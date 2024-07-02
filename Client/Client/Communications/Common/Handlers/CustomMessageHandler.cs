using Communications.DTO;
using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.Common.Handlers
{
	public class CustomMessageHandler : DelegatingHandler
	{
		private readonly IConfiguration _configuration;
		public CustomMessageHandler(IConfiguration configuration) => _configuration = configuration;
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				MessageServerDTO message = JsonConvert.DeserializeObject<MessageServerDTO>(content);

				DomainObjectNotification notification = new DomainObjectNotification
				{
					ClientId = Guid.Parse(_configuration["ClientSetting:ClientId"]),
					ServerId = message.ServerId,
					Notification = message.Notification,
					DateAndTimeSendDataByServer = message.DateAndTimeSendDataByServer,
					DateAndTimeRecievedDataFromServer = DateTime.Now
				};

				string updatedContent = JsonConvert.SerializeObject(notification);
				response.Content = new StringContent(updatedContent, Encoding.UTF8, "application/json");
			}

			return response;
		}
	}
}
