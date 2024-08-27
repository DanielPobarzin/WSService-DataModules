﻿using Communications.DTO;
using Communications.Helpers;
using Communications.UoW;
using Entities.Enums;
using Interactors.Helpers;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Repositories.DO;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Common;
using Shared.Services;
using Shared.Share.KafkaMessage;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Client
{
	public class Program
	{
		private static ProducerService producerService;
		private static UnitOfWorkGetConfig unitOfWorkConfig;
		private static CheckHashHalper checkHashHalper;
		private static CancellationTokenSource cancellationToken;
		private static Thread notificationExchangeThread;
		private static Thread alarmExchangeThread;
		private static Thread kafkaProducerThread;
		private static MemoryCache MemoryCache;

		/// <summary>
		/// The main method defines work threads, creates instances of work units.
		/// </summary>
		/// <remarks>
		/// <see cref="ProducerService"/>
		/// <see cref="UnitOfWorkGetConfig"/>
		/// <see cref="CheckHashHalper"/>
		/// </remarks>
		public static void Main(string[] args)
		{
			GlobalSingletonParameters.Instance.ConnectionCommand = ConnectionCommand.Close;
			GlobalSingletonParameters.Instance.ConnectionCommandChanged += OnConnectionCommandChanged;
			KafkaMessageMetrics.Instance.WorkStatus = WorkStatus.Active;
			// --- Logging --- //
			{
				Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
				.Enrich.FromLogContext()
				.Enrich.WithExceptionData()
				.Enrich.WithThreadName()
				.Enrich.WithDemystifiedStackTraces()
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Information)
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[VERBOSE]_Client_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Verbose)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[ERROR]_Client_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[INFO]_Client_Log_.log",
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 365,
						shared: true,
						restrictedToMinimumLevel: LogEventLevel.Information)
				.CreateLogger();
			}
			// --- CacheInMemory --- // 
			MemoryCache = new MemoryCache(new MemoryCacheOptions
			{
				ExpirationScanFrequency = TimeSpan.FromMinutes(60),	SizeLimit = 1024 * 1024 * 100, CompactionPercentage = 0.4
			});

			// --- Initialize get and check configuration --- // 
			unitOfWorkConfig = new UnitOfWorkGetConfig();
			checkHashHalper = new CheckHashHalper();

			//--------------- Мain threads of the client's work -----------------//
			notificationExchangeThread = new Thread(ExchangeBetweenServerAndClient);
			alarmExchangeThread = new Thread(ExchangeBetweenServerAndClient);

			// --- Initialize Producer (and send current config) & Consumer and run consumer --- //
			producerService = new ProducerService(unitOfWorkConfig.Configuration);
			Task.Run(async () =>
			{
				await producerService.PutMessageProducerProcessAsync("current-config-topic", JsonConvert.SerializeObject
							(unitOfWorkConfig.BracingClientSettings((Microsoft.Extensions.Configuration.IConfigurationRoot)
							unitOfWorkConfig.Configuration), Formatting.Indented), "client-config");
			});
			consumerService = new ConsumerService(unitOfWorkConfig.Configuration);
			consumerService.StartAsync(CancellationToken.None);

			// --- Kafka Producer thread --- //
			kafkaProducerThread = new Thread(StartKafkaProducer);

			//--------------- Defining actions when changing the configuration -----------------//
			OnConfigurationChanged();

			// --- Start adding threads --- //
			notificationExchangeThread.Start(TypeHub.Notify);
			alarmExchangeThread.Start(TypeHub.Alarm);
			kafkaProducerThread.Start();

			Task.Delay(Timeout.Infinite).Wait();
			Log.CloseAndFlush();
		}

		private static ConsumerService consumerService;
		/// <summary>
		/// Start listening to messages from the kafka broker.
		/// </summary>
		public static void StartKafkaProducer()
		{
			cancellationToken = new CancellationTokenSource();
			Task.Run(async () =>
			{
				while (!cancellationToken.Token.IsCancellationRequested)
				{
					if (KafkaMessageMetrics.Instance.ClientId != Guid.Empty)
					{
						KafkaMessageMetrics.Instance.WorkingMemoryUsage = Process.GetCurrentProcess().WorkingSet64;
						KafkaMessageMetrics.Instance.WorkingMemoryUsage = Process.GetCurrentProcess().PrivateMemorySize64;
						await producerService.PutMessageProducerProcessAsync("metric-topic",
						JsonConvert.SerializeObject((KafkaMessageMetrics.Instance), Formatting.Indented), "client-metric");
					}
					await Task.Delay(100, cancellationToken.Token);
				}
			});
		}

		private static Guid clientId;
		/// <summary>
		/// Using basic methods to create a stable connection to the server.
		/// </summary>
		/// <param name="Hub">The input parameter is the type of hub that is used to form a connection to a specific hub.</param>
		/// <remarks>
		/// This is the main method that runs in different threads.
		/// The streams are determined by the type of hub.
		/// Also, the operation of the method depends on the mode -
		/// manual or automatic, i.e. the client either connects himself or on command.
		/// </remarks>
		public static void ExchangeBetweenServerAndClient(object? Hub)
		{
			GlobalSingletonParameters.Instance.ConnectionMode = (ConnectionMode)Enum.
			Parse(typeof(ConnectionMode), unitOfWorkConfig.Configuration["ClientSettings:Mode"]);
			clientId = Guid.Parse(unitOfWorkConfig.Configuration["ClientSettings:ClientId"]);
			KafkaMessageMetrics.Instance.ClientId = clientId;

			Log.Information($"Client started. Id: {clientId}");
			Log.Information($"Mode working by client ({Hub}): {GlobalSingletonParameters.Instance.ConnectionMode}");

			switch (GlobalSingletonParameters.Instance.ConnectionMode)
			{
				case ConnectionMode.Automatic:
					GlobalSingletonParameters.Instance.ConnectionCommand = ConnectionCommand.Open;
				break;
			}

			if (GlobalSingletonParameters.Instance.ConnectionCommand == ConnectionCommand.Open)
			{
				if (Hub is TypeHub type)
				{
					switch (type)
					{
						//--------------- Notify Hub -----------------//
						case TypeHub.Notify:
							string url = unitOfWorkConfig.Configuration["ConnectionSettings:Notify:Url"];
							ConnectionNotify = ConnectionСonfiguration(url); //---- Setting the connection configuration ------// 
							StartHubConnectionAsync(url, ConnectionNotify); //---- Start Hub Connection ----//
							EventWithConnectionHandler(ConnectionNotify); //--- Connection Event Triggers -----//
							GetHubMessages(type, ConnectionNotify); //--- Try get messages ----//
							break;

						//--------------- Alarm Hub -----------------//
						case TypeHub.Alarm:
							url = unitOfWorkConfig.Configuration["ConnectionSettings:Alarm:Url"];
							ConnectionAlarm = ConnectionСonfiguration(url); //---- Setting the connection configuration ------// 
							StartHubConnectionAsync(url, ConnectionAlarm); //---- Start Hub Connection ----//
							EventWithConnectionHandler(ConnectionAlarm); //--- Connection Event Triggers -----//
							GetHubMessages(type, ConnectionAlarm); //--- Try get messages ----//
							break;
					}
				}
			}
		}
		private static HubConnection ConnectionNotify { get; set; }
		private static HubConnection ConnectionAlarm { get; set; }

		/// <summary>
		/// Building a connection object depending on the provided address.
		/// </summary>
		/// <param name="Url">The connection string to a specific hub on the server.</param>
		/// <returns>The connection string object.</returns>
		public static HubConnection ConnectionСonfiguration(string Url)
		{
			var connection = new HubConnectionBuilder()
									.WithUrl(Url, options =>
									{
										options.AccessTokenProvider = null; // TODO: Required!
										options.SkipNegotiation = false;
										options.Cookies = new CookieContainer();
										options.CloseTimeout = TimeSpan.FromSeconds(15);
										options.Headers["Notification"] = "Content";
										options.ClientCertificates = new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
										options.DefaultTransferFormat = TransferFormat.Text;
										options.Credentials = null; // TODO: Required!
										options.UseDefaultCredentials = true;
									})
									.WithKeepAliveInterval(TimeSpan.FromSeconds(80))
									.WithAutomaticReconnect()
									.Build();
			
			connection.ServerTimeout = TimeSpan.FromMinutes(2);
			return connection;
		}

		/// <summary>
		/// Specifying the main methods that are executed depending on the events that occur with the connection.
		/// </summary>
		/// <param name="connection">The input parameter is the built connection object.</param>
		public static void EventWithConnectionHandler(HubConnection connection)
		{
			connection.Closed += (error) =>
			{
				var messageError = (error != null) ? $"An exception has occurred :{error.Message}" : "";
				Log.Information( $"Connection closed. Error message: {messageError}" );
				return Task.CompletedTask;
			};
			connection.Reconnecting += (error) =>
			{
				KafkaMessageMetrics.Instance.ConnectionStatus = ConnectionStatus.Closed;
				var messageError = (error != null) ? $"An exception has occurred :{error.Message}" : "";
				Log.Information($"Connection lost. {messageError}. Reconnecting...");
				return Task.CompletedTask;
			};

			connection.Reconnected += async (connectionId) =>
			{
				Log.Information($"Reconnected. New connection id: {connectionId}");
				try
				{
					await connection.InvokeAsync("OnReconnectedAsync", clientId);
				}
				catch (Exception ex) { Log.Error($"An error occurred when calling the method on the server : {ex.Message}");}
			};
		}

		/// <summary>
		/// The method that determines the operation in case
		/// there was a change in the connect/disconnect command. 
		/// Called when the corresponding event occurs.
		/// </summary>
		/// <param name="newCommand">A new command to disconnect / connect.</param>
		/// <remarks>
		/// It is valid if the connection mode is manual.
		/// </remarks>
		private static async void OnConnectionCommandChanged(object? sender, ConnectionCommand newCommand)
		{
			if (GlobalSingletonParameters.Instance.ConnectionMode == ConnectionMode.Manual)
			{
				switch (newCommand)
				{
					case ConnectionCommand.Close:
						GlobalSingletonParameters.Instance.ConnectionCommand = ConnectionCommand.Close;
						await AllDisconnectAsync();
						break;
					case ConnectionCommand.Open:
						GlobalSingletonParameters.Instance.ConnectionCommand = ConnectionCommand.Open;
						ConnectAsync(TypeHub.Notify, notificationExchangeThread);
						ConnectAsync(TypeHub.Alarm, alarmExchangeThread);
						break;
				}
			}	
		}

		/// <summary>
		/// An attempt to connect to a hub on the server. 
		/// If the attempt is successful, the method is called in the hub to start streaming.
		/// </summary>
		/// <param name="url">The connection address.</param>
		/// <param name="connection">The connection object.</param>
		public static void StartHubConnectionAsync(string url, HubConnection connection)
		{
			Log.Information($"Try to connect to the server by url: {url}");
				try
				{
					connection.StartAsync().GetAwaiter().GetResult();

					Log.Information($"Connection id: {connection.ConnectionId}");
					connection.InvokeAsync("Send", clientId).GetAwaiter().GetResult();
				}
				catch (Exception ex) { Log.Error($"Connection error: {ex.Message}"); }
		}

		/// <summary>
		/// If the connection is established, the method starts receiving all sent messages.
		/// </summary>
		/// <param name="type">The type of hub.</param>
		/// <param name="connection">The connection object.</param>
		/// <remarks>
		/// If a message is received, the method will try to put this message in the database
		/// if it has not arrived before and is not contained in the local cache.
		/// </remarks>
		public static void GetHubMessages(TypeHub type, HubConnection connection)
		{
			TransformToDOHelper TransformToDOHelper = new();
			switch (type)
			{ 
				case TypeHub.Notify:
					connection.On<MessageServerDTO>("ReceiveMessageHandler", async (message) =>
					{
						var compositKey = $"{clientId}_{message.Notification.Id}";
						Log.Information($"Notification {message.Notification.Id} has been recieved."
											+ "\nSender:   " + $" Server - {message.ServerId}"
											+ "\nRecipient:" + $" Client - {clientId}");
						
						var messageNotify = await Communications.Helpers.TransformToDOHelper.TransformToDomainObjectNotification(message, clientId);
						
						{
							KafkaMessageMetrics.Instance.ConnectionStatus = ConnectionStatus.Opened;
							KafkaMessageMetrics.Instance.TotalCountMessages += 1;
							KafkaMessageMetrics.Instance.TotalMessagesSize += Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageNotify)).Length;
							KafkaMessageMetrics.Instance.CountNotifications += 1;
							KafkaMessageMetrics.Instance.Latency = messageNotify.DateAndTimeRecievedDataFromServer - messageNotify.DateAndTimeSendDataByServer;
						}

						if (!MemoryCache.TryGetValue(compositKey, out DomainObjectNotification? cachedNotification))
						{
							try
							{
								using var unitOfWorkPublishNotifications = new UnitOfWorkPublishNotifications(unitOfWorkConfig.Configuration);
								unitOfWorkPublishNotifications.PublishNotifications(messageNotify).Wait();
								MemoryCache.Set($"{clientId}_{messageNotify.Notification.Id}", messageNotify);
							}
							catch (Exception ex) { Log.Error($"Еrror working with the database: {ex.Message}"); }
						}
					});
					break;
				case TypeHub.Alarm:
					connection.On<AlarmServerDTO>("ReceiveMessageHandler", async (message) =>
					{
						var compositKey = $"{clientId}_{message.Alarm.Id}";
						Log.Information($"Alarm {message.Alarm.Id} has been recieved."
											+ "\nSender:   " + $" Server - {message.ServerId}"
											+ "\nRecipient:" + $" Client - {clientId}");

						var messageAlarm = await Communications.Helpers.TransformToDOHelper.TransformToDomainObjectAlarm(message, clientId);

						{
							KafkaMessageMetrics.Instance.ConnectionStatus = ConnectionStatus.Opened;
							KafkaMessageMetrics.Instance.TotalCountMessages += 1;
							KafkaMessageMetrics.Instance.TotalMessagesSize += Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageAlarm)).Length;
							KafkaMessageMetrics.Instance.CountAlarms += 1;
							KafkaMessageMetrics.Instance.Latency = messageAlarm.DateAndTimeRecievedDataFromServer - messageAlarm.DateAndTimeSendDataByServer;
						}

						if (!MemoryCache.TryGetValue(compositKey, out DomainObjectAlarm? cachedAlarm))
						{
							try
							{
								using var unitOfWorkPublishAlarms = new UnitOfWorkPublishAlarms(unitOfWorkConfig.Configuration);
								unitOfWorkPublishAlarms.PublishAlarms(messageAlarm).Wait();
								MemoryCache.Set($"{clientId}_{messageAlarm.Alarm.Id}", messageAlarm);
							}
							catch (Exception ex) { Log.Error($"Еrror working with the database : {ex.Message}"); }
						}
					});
					break;
			}
			connection.On<string>("Notify", (message) =>
			{
				Log.Information($"The message from server: {message}");
			});
		}

		/// <summary>
		/// A method that always expects to receive any changes in the configuration file. 
		/// The action is determined depending on which setting has been changed.
		/// </summary>
		public static async void OnConfigurationChanged()
		{
			await foreach (var hashConfigs in checkHashHalper.CompareHashConfiguration(unitOfWorkConfig.sectionHashes))
			{
				if (hashConfigs.ContainsKey("ClientSettings:ClientId") ||
					hashConfigs.ContainsKey("ClientSettings:Mode"))
				{
					Log.Information($"{clientId}: Changing the settings of client. Reconnect with the new configuration ... ");
					await AllDisconnectAsync();
					RestartAsync(TypeHub.Notify, notificationExchangeThread);
					RestartAsync(TypeHub.Alarm, alarmExchangeThread);
				}
				if(hashConfigs.ContainsKey("DbConnection:DataBase") ||
							hashConfigs.ContainsKey("DbConnection:Notify") ||
							hashConfigs.ContainsKey("ConnectionSettings:Notify"))
				{
					var comment = (hashConfigs.ContainsKey("DbConnection:DataBase") ||
								   hashConfigs.ContainsKey("DbConnection:Notify")) ?
								   ((hashConfigs.ContainsKey("ConnectionSettings:Notify")) ?
								   "database & connection to NotifyHub" : "database")
								   : "NotifyHub";
					Log.Information($"{clientId}: Changing the configuration {comment}. Reconnecting ... ");

					if (ConnectionNotify != null && ConnectionNotify.State == HubConnectionState.Connected)
					{
						await ConnectionNotify.StopAsync();
						await ConnectionNotify.DisposeAsync();
					}
					RestartAsync(TypeHub.Notify, notificationExchangeThread);
				}
				
				else if (hashConfigs.ContainsKey("DbConnection:DataBase") ||
							hashConfigs.ContainsKey("DbConnection:Alarm") ||
							hashConfigs.ContainsKey("ConnectionSettings:Alarm"))
				{
					var comment = (hashConfigs.ContainsKey("DbConnection:DataBase") ||
								   hashConfigs.ContainsKey("DbConnection:AlarmConnection")) ?
								   ((hashConfigs.ContainsKey("ConnectionSettings:Alarm")) ?
								   "database & connection to AlarmHub" : "database")
								   : "AlarmHub";
					Log.Information($"{clientId}: Changing the configuration {comment}. Reconnecting ... ");

					if (ConnectionAlarm != null && ConnectionAlarm.State == HubConnectionState.Connected)
					{
						await ConnectionAlarm.StopAsync();
						await ConnectionAlarm.DisposeAsync();
					}
					RestartAsync(TypeHub.Alarm, alarmExchangeThread);
				}

				if (hashConfigs.ContainsKey("Kafka:Producer"))
				{
					Log.Information($"{clientId}: Changing the settings of connecting to kafka broker (role:Producer). ");
					RebootThread(cancellationToken, kafkaProducerThread, StartKafkaProducer);
				}
				if (hashConfigs.ContainsKey("Kafka:Consumer"))
				{
					Log.Information($"{clientId}: Changing the settings of connecting to kafka broker(role:Consumer). ");
					await consumerService.StopAsync(CancellationToken.None);
					consumerService = new ConsumerService(unitOfWorkConfig.Configuration);
					await consumerService.StartAsync(CancellationToken.None);
				}
				await producerService.PutMessageProducerProcessAsync("current-config-topic", JsonConvert.SerializeObject
							(unitOfWorkConfig.BracingClientSettings((Microsoft.Extensions.Configuration.IConfigurationRoot)
							unitOfWorkConfig.Configuration), Formatting.Indented), "client-config");
			}
		}

		/// <summary>
		/// An attempt to completely disconnect from all hubs if the connection is active.
		/// </summary>
		/// <remarks>
		/// After disconnecting, the connection object is disposed.
		/// </remarks>
		public static async Task AllDisconnectAsync()
		{
			if (ConnectionNotify != null && ConnectionNotify.State == HubConnectionState.Connected)
			{
				await ConnectionNotify.StopAsync();
				await ConnectionNotify.DisposeAsync();
			}
			if (ConnectionAlarm != null && ConnectionAlarm.State == HubConnectionState.Connected)
			{
				await ConnectionAlarm.StopAsync();
				await ConnectionAlarm.DisposeAsync();
			}
		}

		/// <summary>
		/// Restarting the thread to create and establish a connection to the hub.
		/// </summary>
		/// <param name="hub"> The input parameter is the type of hub that is used to form a connection to a specific hub.</param>
		/// <param name="thread"> The thread in which the connection to a specific hub is performed. </param>
		/// <remarks>
		/// It is valid if the connection mode is manual.
		/// </remarks>
		public static void RestartAsync(TypeHub hub, Thread thread)
		{
			thread.Join();
			Thread newThread = new Thread(ExchangeBetweenServerAndClient);
			newThread.Start(hub);
			thread = newThread;
		}

		/// <summary>
		/// The method tries to start a thread to work with a certain type of hub.
		/// </summary>
		/// <param name="hub"> The input parameter is the type of hub that is used to form a connection to a specific hub.</param>
		/// <param name="thread"> The thread in which the connection to a specific hub is performed. </param>
		/// <remarks>
		/// f connection threads have not been started, 
		/// a thread is created and started for a specific hub. 
		/// Otherwise, a restart occurs.
		/// </remarks>
		public static void ConnectAsync(TypeHub hub, Thread thread)
		{
			if (thread.ThreadState == System.Threading.ThreadState.Unstarted)
			{
				Thread newThread = new(ExchangeBetweenServerAndClient);
				newThread.Start(hub);
				thread = newThread;
			} else
			{
				RestartAsync(hub, thread);
			}
		}
		private static void RebootThread(CancellationTokenSource cancellationToken, Thread thread, Action action)
		{
			cancellationToken.Cancel();
			cancellationToken.Dispose();
			thread.Join();
			if (action == StartKafkaProducer)
			{
				producerService.Dispose();
				producerService = new ProducerService(unitOfWorkConfig.Configuration);
			}
			Thread newThread = new Thread(new ThreadStart(action));
			newThread.Start();
			thread = newThread;
		}
	}
}