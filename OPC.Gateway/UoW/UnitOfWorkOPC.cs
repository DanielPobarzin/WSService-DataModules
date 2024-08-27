extern alias UaFx;

using Opc.Ua;
using OPC.Gateway.DTOs;
using OPC.Gateway.UDT.AnalogInput;
using OPC.Gateway.UDT.cRIO;
using OPC.Gateway.UDT.DiscreteInput;
using OPC.Gateway.UDT.IntValue;
using OPC.Gateway.UDT.Tech_Cab;
using OPC.Gateway.UDT.Valves;
using Serilog;
using UaFx::Opc.UaFx;
using UaFx::Opc.UaFx.Client;

namespace OPC.Gateway.UoW
{

	public class UnitOfWorkOPC
	{
		Timer timer;
		object OPCLocker;
		TimerCallback OpcInnerTimer;
		OpcClient client;
		public delegate void OPCHandler(string text);


		OPCHandler _opcHandler;
		public event OPCHandler OPCNotify;
		public OPCObjects opcobjects;

		public UnitOfWorkOPC()
		{
			opcobjects = OPCObjects.createObjects();
		}
		public void RegisterHandler(OPCHandler opcHandler)
		{
			_opcHandler = opcHandler;
		}

		private static void ReadAnalogValue(ref AIValue analogValue, string Path, OpcClient client)
		{
			OpcValue var = client.ReadNode(Path);
			analogValue.Path = Path;
			analogValue.Value = float.Parse(var.ToString());

		}


		private static void ReadDiscreteValue(ref DIValue discreteValue, string Path, OpcClient client)
		{
			OpcValue var = client.ReadNode(Path);
			discreteValue.Path = Path;
			discreteValue.Value = (bool)var.Value;
		}

		private static void ReadIntegerValue(ref IntValue intValue, string Path, OpcClient client)
		{
			OpcValue var = client.ReadNode(Path);
			intValue.Path = Path;
			intValue.Value = int.Parse(var.ToString());
		}

		public static void ReadAIValues(OpcClient client)
		{
			ReadAnalogValue(ref OPCObjects.BLM_Speed_SP, UnitOfWorkOPCPath.BLM_Speed_SP_path, client);
			ReadAnalogValue(ref OPCObjects.Camera_Pressure, UnitOfWorkOPCPath.CameraPressure_path, client);
			ReadAnalogValue(ref OPCObjects.Crio_Pressure, UnitOfWorkOPCPath.CrioPressure_path, client);
			ReadAnalogValue(ref OPCObjects.Crio_Temperature, UnitOfWorkOPCPath.CrioTemperature_path, client);
			ReadAnalogValue(ref OPCObjects.FT_TT_1, UnitOfWorkOPCPath.FT_TT_1_path, client);
			ReadAnalogValue(ref OPCObjects.FT_TT_2, UnitOfWorkOPCPath.FT_TT_2_path, client);
			ReadAnalogValue(ref OPCObjects.FT_TT_3, UnitOfWorkOPCPath.FT_TT_3_path, client);
			ReadAnalogValue(ref OPCObjects.HeatAssist_Temp_SP, UnitOfWorkOPCPath.HeatAssist_Temp_SP_path, client);
			ReadAnalogValue(ref OPCObjects.HeatAssist_Timer_SP, UnitOfWorkOPCPath.Heat_Assist_Timer_SP_path, client);
			ReadAnalogValue(ref OPCObjects.Main_Pressure, UnitOfWorkOPCPath.MainPressure_path, client);
			ReadAnalogValue(ref OPCObjects.ManualSetTemp, UnitOfWorkOPCPath.ManualSetTemp_path, client);
			ReadAnalogValue(ref OPCObjects.Pneumatic_Pressure, UnitOfWorkOPCPath.PneumaticPressure_path, client);
			ReadAnalogValue(ref OPCObjects.PreHeat_Temp_SP, UnitOfWorkOPCPath.PreHeat_Temp_SP_path, client);
			ReadAnalogValue(ref OPCObjects.PreHeat_Timer_SP, UnitOfWorkOPCPath.PreHeat_Timer_SP_path, client);
			ReadAnalogValue(ref OPCObjects.BLM_Speed, UnitOfWorkOPCPath.BLM_Speed_path, client);
			ReadAnalogValue(ref OPCObjects.RRG_9A1_feedback, UnitOfWorkOPCPath.RRG_9A1_feedback_path, client);
			ReadAnalogValue(ref OPCObjects.RRG_9A2_feedback, UnitOfWorkOPCPath.RRG_9A2_feedback_path, client);
			ReadAnalogValue(ref OPCObjects.RRG_9A3_feedback, UnitOfWorkOPCPath.RRG_9A3_feedback_path, client);
			ReadAnalogValue(ref OPCObjects.RRG_9A4_feedback, UnitOfWorkOPCPath.RRG_9A4_feedback_path, client);
			ReadAnalogValue(ref OPCObjects.SFT01_FT, UnitOfWorkOPCPath.SFT01_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT02_FT, UnitOfWorkOPCPath.SFT02_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT03_FT, UnitOfWorkOPCPath.SFT03_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT04_FT, UnitOfWorkOPCPath.SFT04_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT05_FT, UnitOfWorkOPCPath.SFT05_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT06_FT, UnitOfWorkOPCPath.SFT06_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT07_FT, UnitOfWorkOPCPath.SFT07_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT08_FT, UnitOfWorkOPCPath.SFT08_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT09_FT, UnitOfWorkOPCPath.SFT09_FT_path, client);
			ReadAnalogValue(ref OPCObjects.SFT10_FT, UnitOfWorkOPCPath.SFT10_FT_path, client);
			ReadAnalogValue(ref OPCObjects.TE_1, UnitOfWorkOPCPath.TE_1_path, client);
			ReadAnalogValue(ref OPCObjects.K_RRG1, UnitOfWorkOPCPath.K_RRG1_path, client);
			ReadAnalogValue(ref OPCObjects.K_RRG2, UnitOfWorkOPCPath.K_RRG2_path, client);
			ReadAnalogValue(ref OPCObjects.K_RRG3, UnitOfWorkOPCPath.K_RRG3_path, client);
			ReadAnalogValue(ref OPCObjects.K_RRG4, UnitOfWorkOPCPath.K_RRG4_path, client);
			ReadAnalogValue(ref OPCObjects.RRG_Pressure_SP, UnitOfWorkOPCPath.RRG_Pressure_SP, client);
			ReadAnalogValue(ref OPCObjects.PidHeatMode, UnitOfWorkOPCPath.PidHeatMode_path, client);

		}
		public static void ReadDiscretValues(OpcClient client)
		{
			ReadDiscreteValue(ref OPCObjects.Alarm_Crio_power_failure, UnitOfWorkOPCPath.Alarm_Crio_power_failure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_ELI_Power_failure, UnitOfWorkOPCPath.Alarm_Crio_power_failure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_FloatHeater_power_failure, UnitOfWorkOPCPath.Alarm_FloatHeater_power_failure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_FVP_power_failure, UnitOfWorkOPCPath.Alarm_FVP_power_failure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Hight_Crio_Temp, UnitOfWorkOPCPath.Alarm_Hight_Crio_Temp_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Hight_Pne_Press, UnitOfWorkOPCPath.Alarm_Hight_Pne_Presse_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Indexer_power_failure, UnitOfWorkOPCPath.Alarm_Indexer_power_failure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Ion_power_failure, UnitOfWorkOPCPath.Alarm_Ion_power_failure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Low_One_Presse, UnitOfWorkOPCPath.Alarm_Low_One_Presse_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_manual_stop, UnitOfWorkOPCPath.Alarm_manual_Stop_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Open_door, UnitOfWorkOPCPath.Alarm_Open_door_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Qartz_power_failure, UnitOfWorkOPCPath.Alarm_Qartz_power_filure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_SSP_power_failure, UnitOfWorkOPCPath.Alarm_SSP_power_failure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_TV1_power_failure, UnitOfWorkOPCPath.Alarm_TV1_power_failure_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Water_CRIO, UnitOfWorkOPCPath.Alarm_Water_CRIO_path, client);
			ReadDiscreteValue(ref OPCObjects.Alarm_Water_SECOND, UnitOfWorkOPCPath.Alarm_Water_SECOND_path, client);
			ReadDiscreteValue(ref OPCObjects.BLM_Remote_Control_Done, UnitOfWorkOPCPath.BLM_Remote_Control_Done_path, client);
			ReadDiscreteValue(ref OPCObjects.BLM_Run, UnitOfWorkOPCPath.BLM_Run_path, client);
			ReadDiscreteValue(ref OPCObjects.BLM_Start, UnitOfWorkOPCPath.BLM_Start_path, client);
			ReadDiscreteValue(ref OPCObjects.BLM_Stop, UnitOfWorkOPCPath.BLM_Stop_path, client);
			ReadDiscreteValue(ref OPCObjects.Crio_start_signal, UnitOfWorkOPCPath.Crio_start_signal_path, client);
			ReadDiscreteValue(ref OPCObjects.HeatAssist_Done, UnitOfWorkOPCPath.HeatAssist_Done_path, client);
			ReadDiscreteValue(ref OPCObjects.HeatAssist_Flag, UnitOfWorkOPCPath.HeatAssist_Flag_path, client);
			ReadDiscreteValue(ref OPCObjects.HeatAssist_TempDone, UnitOfWorkOPCPath.HeatAssist_TempDone_path, client);
			ReadDiscreteValue(ref OPCObjects.Heat_Assit_On, UnitOfWorkOPCPath.Heat_Assist_ON_path, client);
			ReadDiscreteValue(ref OPCObjects.Heat_Done, UnitOfWorkOPCPath.Heat_Done_path, client);
			ReadDiscreteValue(ref OPCObjects.PreHeat_Done, UnitOfWorkOPCPath.PreHeat_Done_path, client);
			ReadDiscreteValue(ref OPCObjects.PreHeat_Start, UnitOfWorkOPCPath.PreHeat_Start_path, client);
			ReadDiscreteValue(ref OPCObjects.StartProcessSignal, UnitOfWorkOPCPath.StartProcessSignal_path, client);
			ReadDiscreteValue(ref OPCObjects.StopProcessSignal, UnitOfWorkOPCPath.StopProcessSignal_path, client);
			ReadDiscreteValue(ref OPCObjects.ELI_complete, UnitOfWorkOPCPath.ELI_complete_path, client);
			ReadDiscreteValue(ref OPCObjects.ELI_access, UnitOfWorkOPCPath.ELI_access_path, client);

		}
		public static void ReadIntegerValues(OpcClient client)
		{

			//variable.HeatAssist_Stage = client.ReadNode(UnitOfWorkOPCPath.HeatAssist_Stage_path);
			//variable.Tech_cam_STAGE = client.ReadNode(UnitOfWorkOPCPath.Tech_cam_STAGE_path);
			//variable.PreHeat_Stage = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Stage_path);
			ReadIntegerValue(ref OPCObjects.HeatAssist_Stage, UnitOfWorkOPCPath.HeatAssist_Stage_path, client);
			ReadIntegerValue(ref OPCObjects.Tech_cam_STAGE, UnitOfWorkOPCPath.Tech_cam_STAGE_path, client);
			ReadIntegerValue(ref OPCObjects.PreHeat_Stage, UnitOfWorkOPCPath.PreHeat_Stage_path, client);
			//ReadIntegerValue(ref OPCObjects.FullCycleStage, UnitOfWorkOPCPath.FullCycleStage_path, client);

		}

		public void StartOPCUAClient()
		{
			AI variable = new AI();
			OPCLocker = new object();
			client = new OpcClient("opc.tcp://192.168.0.10:4840/");
			opcobjects = OPCObjects.createObjects();

			OPCObjects.client = client;
			OPCObjects.OPCLocker = OPCLocker;
			OPCObjects.client.Connect();

			lock (OPCObjects.OPCLocker)
			{
				//client.Connect();
				#region BAV_3
				ValveStatus BAV_3_Status = client.ReadNode(UnitOfWorkOPCPath.BAV_3_Status_path).As<ValveStatus>();
				ValveInput BAV_3_Input = client.ReadNode(UnitOfWorkOPCPath.BAV_3_Input_path).As<ValveInput>();
				#endregion
				#region FVV_S
				ValveStatus FVV_S_Status = client.ReadNode(UnitOfWorkOPCPath.FVV_S_Status_path).As<ValveStatus>();
				ValveInput FVV_S_Input = client.ReadNode(UnitOfWorkOPCPath.FVV_S_Input_path).As<ValveInput>();
				#endregion
				#region FVV_B
				ValveStatus FVV_B_Status = client.ReadNode(UnitOfWorkOPCPath.FVV_B_Status_path).As<ValveStatus>();
				ValveInput FVV_B_Input = client.ReadNode(UnitOfWorkOPCPath.FVV_B_Input_path).As<ValveInput>();
				#endregion
				#region CPV
				ValveStatus CPV_Status = client.ReadNode(UnitOfWorkOPCPath.CPV_Status_path).As<ValveStatus>();
				ValveInput CPV_Input = client.ReadNode(UnitOfWorkOPCPath.CPV_Input_path).As<ValveInput>();
				#endregion
				#region SHV
				ValveStatus SHV_Status = client.ReadNode(UnitOfWorkOPCPath.SHV_Status_path).As<ValveStatus>();
				ValveInput SHV_Input = client.ReadNode(UnitOfWorkOPCPath.SHV_Input_path).As<ValveInput>();
				#endregion

				#region Crio_pump
				CRIOInput crioInput = client.ReadNode(UnitOfWorkOPCPath.Crio_pump_Input_path).As<CRIOInput>();
				CRIOStatus crioStatus = client.ReadNode(UnitOfWorkOPCPath.Crio_pump_Status_path).As<CRIOStatus>();
				#endregion
				StopFVP StopFVP = client.ReadNode(UnitOfWorkOPCPath.StopFVP_path).As<StopFVP>();
				StopCrio StopCrio = client.ReadNode(UnitOfWorkOPCPath.StopCrio_path).As<StopCrio>();


				OpenCam OpenCam = client.ReadNode(UnitOfWorkOPCPath.OpenCam_path).As<OpenCam>();
				CrioPumpStart CrioPumpStart = client.ReadNode(UnitOfWorkOPCPath.CrioPumpStart_path).As<CrioPumpStart>();
				CabPrepare camPrepare = client.ReadNode(UnitOfWorkOPCPath.CamPrepare_path).As<CabPrepare>();
				ReadAIValues(client);

				ReadDiscretValues(client);

				ReadIntegerValues(client);
				//variable.HeatAssist_Timer = client.ReadNode(UnitOfWorkOPCPath.HeatAssist_Timer_path);

				//variable.Heat_Assist_Timer_SP = client.ReadNode(UnitOfWorkOPCPath.Heat_Assist_Timer_SP_path);


				//variable.MainPressure = client.ReadNode(UnitOfWorkOPCPath.MainPressure_path);

				//variable.ManualSetTemp = client.ReadNode(UnitOfWorkOPCPath.ManualSetTemp_path);

				//variable.PneumaticPressure = client.ReadNode(UnitOfWorkOPCPath.PneumaticPressure_path);




				//variable.PreHeat_Temp_SP = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Temp_SP_path);

				//variable.PreHeat_Timer = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Timer_path);

				//variable.PreHeat_Timer_SP = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Timer_SP_path);

				//variable.RRG_9A1_feedback = client.ReadNode(UnitOfWorkOPCPath.RRG_9A1_feedback_path);
				//variable.RRG_9A2_feedback = client.ReadNode(UnitOfWorkOPCPath.RRG_9A2_feedback_path);
				//variable.RRG_9A3_feedback = client.ReadNode(UnitOfWorkOPCPath.RRG_9A3_feedback_path);
				//variable.RRG_9A4_feedback = client.ReadNode(UnitOfWorkOPCPath.RRG_9A4_feedback_path);
				//variable.SFT01_FT = client.ReadNode(UnitOfWorkOPCPath.SFT01_FT_path);
				//variable.SFT02_FT = client.ReadNode(UnitOfWorkOPCPath.SFT02_FT_path);
				//variable.SFT03_FT = client.ReadNode(UnitOfWorkOPCPath.SFT03_FT_path);
				//variable.SFT04_FT = client.ReadNode(UnitOfWorkOPCPath.SFT04_FT_path);
				//variable.SFT05_FT = client.ReadNode(UnitOfWorkOPCPath.SFT05_FT_path);
				//variable.SFT06_FT = client.ReadNode(UnitOfWorkOPCPath.SFT06_FT_path);
				//variable.SFT07_FT = client.ReadNode(UnitOfWorkOPCPath.SFT07_FT_path);
				//variable.SFT08_FT = client.ReadNode(UnitOfWorkOPCPath.SFT08_FT_path);
				//variable.SFT09_FT = client.ReadNode(UnitOfWorkOPCPath.SFT09_FT_path);
				//variable.SFT10_FT = client.ReadNode(UnitOfWorkOPCPath.SFT10_FT_path);
				//variable.TE_1 = client.ReadNode(UnitOfWorkOPCPath.TE_1_path);




				//client.Disconnect();

				Log.Information("SDASDASDASDASDSA {0}", opcobjects.GetIonInputSetPoint().Heat_U_SP);


				OPCObjects.AIValues.Add(OPCObjects.BLM_Speed_SP);
				OPCObjects.AIValues.Add(OPCObjects.Camera_Pressure);
				OPCObjects.AIValues.Add(OPCObjects.Crio_Pressure);
				OPCObjects.AIValues.Add(OPCObjects.Crio_Temperature);
				OPCObjects.AIValues.Add(OPCObjects.FT_TT_1);
				OPCObjects.AIValues.Add(OPCObjects.FT_TT_2);
				OPCObjects.AIValues.Add(OPCObjects.FT_TT_3);
				OPCObjects.AIValues.Add(OPCObjects.HeatAssist_Temp_SP);
				OPCObjects.AIValues.Add(OPCObjects.HeatAssist_Timer_SP);
				OPCObjects.AIValues.Add(OPCObjects.Main_Pressure);
				OPCObjects.AIValues.Add(OPCObjects.ManualSetTemp);
				OPCObjects.AIValues.Add(OPCObjects.Pneumatic_Pressure);
				OPCObjects.AIValues.Add(OPCObjects.PreHeat_Temp_SP);
				OPCObjects.AIValues.Add(OPCObjects.PreHeat_Timer_SP);
				OPCObjects.AIValues.Add(OPCObjects.BLM_Speed);
				OPCObjects.AIValues.Add(OPCObjects.RRG_9A1_feedback);
				OPCObjects.AIValues.Add(OPCObjects.RRG_9A2_feedback);
				OPCObjects.AIValues.Add(OPCObjects.RRG_9A3_feedback);
				OPCObjects.AIValues.Add(OPCObjects.RRG_9A4_feedback);
				OPCObjects.AIValues.Add(OPCObjects.SFT01_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT02_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT03_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT04_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT05_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT06_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT07_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT08_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT09_FT);
				OPCObjects.AIValues.Add(OPCObjects.SFT10_FT);
				OPCObjects.AIValues.Add(OPCObjects.TE_1);
				OPCObjects.AIValues.Add(OPCObjects.K_RRG1);
				OPCObjects.AIValues.Add(OPCObjects.K_RRG2);
				OPCObjects.AIValues.Add(OPCObjects.K_RRG3);
				OPCObjects.AIValues.Add(OPCObjects.K_RRG4);
				OPCObjects.AIValues.Add(OPCObjects.RRG_Pressure_SP);
				OPCObjects.AIValues.Add(OPCObjects.PidHeatMode);

				OPCObjects.DIValues.Add(OPCObjects.Alarm_Crio_power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_ELI_Power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_FloatHeater_power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_FVP_power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Hight_Crio_Temp);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Hight_Pne_Press);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Indexer_power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Ion_power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Low_One_Presse);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_manual_stop);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Open_door);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Qartz_power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_SSP_power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_TV1_power_failure);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Water_CRIO);
				OPCObjects.DIValues.Add(OPCObjects.Alarm_Water_SECOND);
				OPCObjects.DIValues.Add(OPCObjects.BLM_Remote_Control_Done);
				OPCObjects.DIValues.Add(OPCObjects.BLM_Run);
				OPCObjects.DIValues.Add(OPCObjects.BLM_Start);
				OPCObjects.DIValues.Add(OPCObjects.BLM_Stop);
				OPCObjects.DIValues.Add(OPCObjects.Crio_start_signal);
				OPCObjects.DIValues.Add(OPCObjects.HeatAssist_Done);
				OPCObjects.DIValues.Add(OPCObjects.HeatAssist_Flag);
				OPCObjects.DIValues.Add(OPCObjects.HeatAssist_TempDone);
				OPCObjects.DIValues.Add(OPCObjects.Heat_Assit_On);
				OPCObjects.DIValues.Add(OPCObjects.Heat_Done);
				OPCObjects.DIValues.Add(OPCObjects.PreHeat_Done);
				OPCObjects.DIValues.Add(OPCObjects.PreHeat_Start);
				OPCObjects.DIValues.Add(OPCObjects.StopProcessSignal);
				OPCObjects.DIValues.Add(OPCObjects.StartProcessSignal);
				OPCObjects.DIValues.Add(OPCObjects.ELI_access);
				OPCObjects.DIValues.Add(OPCObjects.ELI_complete);

				OPCObjects.IntValues.Add(OPCObjects.HeatAssist_Stage);
				OPCObjects.IntValues.Add(OPCObjects.Tech_cam_STAGE);
				OPCObjects.IntValues.Add(OPCObjects.PreHeat_Stage);

				opcobjects.setBAV_3_Status(BAV_3_Status);
				opcobjects.setBAV_3_Input(BAV_3_Input);
				opcobjects.set_CVP_Input(CPV_Input);
				opcobjects.set_CVP_Status(CPV_Status);
				opcobjects.set_FVV_B_Input(FVV_B_Input);
				opcobjects.set_FVV_B_Status(FVV_B_Status);
				opcobjects.set_FVV_S_Input(FVV_S_Input);
				opcobjects.set_FVV_S_Status(FVV_S_Status);
				opcobjects.set_SHV_Input(SHV_Input);
				opcobjects.set_SHV_Status(SHV_Status);
				opcobjects.SetStopFvp(StopFVP);
				opcobjects.SetStopCrio(StopCrio);
				opcobjects.set_OpcClient(client);
				opcobjects.SetOpenCam(OpenCam);
				opcobjects.set_CrioPumpStart(CrioPumpStart);
				//opcobjects.SetIonInputSetPoint(IonInputSetPoint);
				// opcobjects.SetIonInputCommand(IonInputCommand);
				//opcobjects.SetFVPStatus(FVPStatus);
				opcobjects.setOPCLocker(OPCLocker);


				OPCObjects.ValvesInput.Add(1, OPCObjects.BAV_3_input);
				OPCObjects.ValvesStatus.Add(1, OPCObjects.BAV_3_status);
				OPCObjects.ValvesInput.Add(2, OPCObjects.SHV_Input);
				OPCObjects.ValvesStatus.Add(2, OPCObjects.SHV_Status);
				OPCObjects.ValvesInput.Add(3, OPCObjects.FVV_S_Input);
				OPCObjects.ValvesStatus.Add(3, OPCObjects.FVV_S_Status);
				OPCObjects.ValvesStatus.Add(4, OPCObjects.FVV_B_Status);
				OPCObjects.ValvesInput.Add(4, OPCObjects.FVV_B_Input);
				OPCObjects.ValvesInput.Add(5, OPCObjects.CPV_Input);
				OPCObjects.ValvesStatus.Add(5, OPCObjects.CPV_Status);

			}
			//this.RegisterSubscribe();

			OpcInnerTimer = new TimerCallback(TimerRead);
			timer = new Timer(OpcInnerTimer, client, 0, 2000);

			_opcHandler("OPC DONE");




		}


		public void RegisterSubscribe()
		{
			OpcSubscribeDataChange[] commands = new OpcSubscribeDataChange[]
			{
				new OpcSubscribeDataChange(UnitOfWorkOPCPath.MainPressure_path,HandleAnalogDataChange),
				new OpcSubscribeDataChange(UnitOfWorkOPCPath.CrioPressure_path,HandleAnalogDataChange)
			};
			var client = opcobjects.get_OpcClietn();
			OpcSubscription subscription = opcobjects.get_OpcClietn().SubscribeNodes(commands);
		}

		private static void HandleAnalogDataChange(object sender, OpcDataChangeReceivedEventArgs e)
		{
			OpcMonitoredItem item = (OpcMonitoredItem)sender;

			DIValues(
					"Data Change from NodeId '{0}': {1}",
					item.RelativePath,
					e.Item.Value);
			var objects = Objects.OPCObjects.createObjects();
			using (var context = new MyDBContext())
			{
				lock (objects.getSQLLocker())
				{
					var analogs = context.AnalogValue.Where(x => x.Path == item.NodeId);
					foreach (var analog in analogs)
					{
						var input = e.Item.Value.ToString();
						analog.Value = float.Parse(input);
						context.Update(analog);
						context.SaveChanges();
					}


				}

			}

		}
		public static void Write<T>(string path, T obj)
		{

			var client = OPCObjects.client;
			lock (OPCObjects.OPCLocker)
			{
				//client.Connect();
				client.WriteNode(path, obj);
			}


		}
		public static void WriteDi(string path, bool obj)
		{

			var client = OPCObjects.client;
			lock (OPCObjects.OPCLocker)
			{
				//client.Connect();
				client.WriteNode(path, obj);
			}

		}
		private static void ReadOPCData(object objclient)
		{
			var objects = OPCObjects.createObjects();
			lock (OPCObjects.OPCLocker)
			{
				OpcClient client = (OpcClient)objclient;
				var BAV_3_Status = objects.getBAV_3_Status();
				var BAV_3_Input = objects.getBAV_3_Input();
				var FVV_S_Status = objects.get_FVV_S_Status();
				var FVV_S_Input = objects.get_FVV_S_Input();
				var FVV_B_Status = objects.get_FVV_B_Status();
				var FVV_B_Input = objects.get_FVV_B_Input();
				var CPV_Status = objects.get_CVP_Status();
				var CPV_Input = objects.get_CVP_Input();
				var SHV_Status = objects.get_SHV_Status();
				var SHV_Input = objects.get_SHV_Input();
				var crioInput = objects.GetCrioInput();
				var crioStatus = objects.GetCrioStatus();
				var StopFVP = objects.GetStopFVP();
				var StopCrio = objects.GetStopCrio();
				var OpenCam = objects.GetOpenCam();
				var CrioPumpStart = objects.GetCrioPumpStart();
				var CamPrepare = objects.get_camPrepare();
				var IonStatus = objects.GetIonStatus();
				var IonOutputFeedBack = objects.GetIonOutputFeedBack();
				//var IonInputSetPoint = objects.GetIonInputSetPoint();
				var IonInputCommand = objects.GetIonInputCommand();
				var FVPStatus = objects.GetFVPStatus();
				var variable = objects.GetAnalogInput();



				//client.Connect();
				BAV_3_Status = client.ReadNode(UnitOfWorkOPCPath.BAV_3_Status_path).As<ValveStatus>();
				BAV_3_Input = client.ReadNode(UnitOfWorkOPCPath.BAV_3_Input_path).As<ValveInput>();
				FVV_S_Status = client.ReadNode(UnitOfWorkOPCPath.FVV_S_Status_path).As<ValveStatus>();
				FVV_S_Input = client.ReadNode(UnitOfWorkOPCPath.FVV_S_Input_path).As<ValveInput>();
				FVV_B_Status = client.ReadNode(UnitOfWorkOPCPath.FVV_B_Status_path).As<ValveStatus>();
				FVV_B_Input = client.ReadNode(UnitOfWorkOPCPath.FVV_B_Input_path).As<ValveInput>();
				CPV_Status = client.ReadNode(UnitOfWorkOPCPath.CPV_Status_path).As<ValveStatus>();
				CPV_Input = client.ReadNode(UnitOfWorkOPCPath.CPV_Input_path).As<ValveInput>();
				SHV_Status = client.ReadNode(UnitOfWorkOPCPath.SHV_Status_path).As<ValveStatus>();
				//variable.Alarm_Crio_power_failure = client.ReadNode(UnitOfWorkOPCPath.Alarm_Crio_power_failure_path);

				//variable.Alarm_ELI_power_failure = client.ReadNode(UnitOfWorkOPCPath.Alarm_ELI_power_failure_path);
				//variable.Alarm_FloatHeater_power_failure = client.ReadNode(UnitOfWorkOPCPath.Alarm_FloatHeater_power_failure_path);
				//variable.Alarm_FVP_power_failure = client.ReadNode(UnitOfWorkOPCPath.Alarm_FVP_power_failure_path);
				//variable.Alarm_Hight_Crio_Temp = client.ReadNode(UnitOfWorkOPCPath.Alarm_Hight_Crio_Temp_path);
				//variable.Alarm_Hight_Pne_Presse = client.ReadNode(UnitOfWorkOPCPath.Alarm_Hight_Pne_Presse_path);
				//variable.Alarm_Indexer_power_failure = client.ReadNode(UnitOfWorkOPCPath.Alarm_Indexer_power_failure_path);
				//variable.Alarm_Ion_power_failure = client.ReadNode(UnitOfWorkOPCPath.Alarm_Ion_power_failure_path);
				//variable.Alarm_Low_One_Presse = client.ReadNode(UnitOfWorkOPCPath.Alarm_Low_One_Presse_path);
				//variable.Alarm_manual_Stop = client.ReadNode(UnitOfWorkOPCPath.Alarm_manual_Stop_path);
				//variable.Alarm_Open_door = client.ReadNode(UnitOfWorkOPCPath.Alarm_Open_door_path);
				//variable.Alarm_Qartz_power_filure = client.ReadNode(UnitOfWorkOPCPath.Alarm_Qartz_power_filure_path);
				//variable.Alarm_SSP_power_failure = client.ReadNode(UnitOfWorkOPCPath.Alarm_SSP_power_failure_path);
				//variable.Alarm_TV1_power_failure = client.ReadNode(UnitOfWorkOPCPath.Alarm_TV1_power_failure_path);
				//variable.Alarm_Water_CRIO = client.ReadNode(UnitOfWorkOPCPath.Alarm_Water_CRIO_path);
				//variable.Alarm_Water_SECOND = client.ReadNode(UnitOfWorkOPCPath.Alarm_Water_SECOND_path);
				//variable.BLM_Remote_Control_Done = client.ReadNode(UnitOfWorkOPCPath.BLM_Remote_Control_Done_path);
				//variable.BLM_Run = client.ReadNode(UnitOfWorkOPCPath.BLM_Run_path);
				//variable.BLM_Speed = client.ReadNode(UnitOfWorkOPCPath.BLM_Speed_path);
				//variable.BLM_Speed_SP = client.ReadNode(UnitOfWorkOPCPath.BLM_Speed_SP_path);
				//variable.BLM_Start = client.ReadNode(UnitOfWorkOPCPath.BLM_Start_path);
				//variable.BLM_Stop = client.ReadNode(UnitOfWorkOPCPath.BLM_Stop_path);
				//variable.CameraPressure = client.ReadNode(UnitOfWorkOPCPath.CameraPressure_path);
				//variable.CrioPressure = client.ReadNode(UnitOfWorkOPCPath.CrioPressure_path);
				//variable.CrioTemperature = client.ReadNode(UnitOfWorkOPCPath.CrioTemperature_path);
				//variable.Crio_start_signal = client.ReadNode(UnitOfWorkOPCPath.Crio_start_signal_path);
				//variable.FT_TT_1 = client.ReadNode(UnitOfWorkOPCPath.FT_TT_1_path);
				//variable.FT_TT_2 = client.ReadNode(UnitOfWorkOPCPath.FT_TT_2_path);
				//variable.FT_TT_3 = client.ReadNode(UnitOfWorkOPCPath.FT_TT_3_path);
				//variable.HeatAssist_Done = client.ReadNode(UnitOfWorkOPCPath.HeatAssist_Done_path);
				//variable.HeatAssist_Flag = client.ReadNode(UnitOfWorkOPCPath.HeatAssist_Flag_path);
				//variable.HeatAssist_Stage = client.ReadNode(UnitOfWorkOPCPath.HeatAssist_Stage_path);
				//variable.HeatAssist_TempDone = client.ReadNode(UnitOfWorkOPCPath.HeatAssist_TempDone_path);
				//variable.HeatAssist_Temp_SP = client.ReadNode(UnitOfWorkOPCPath.HeatAssist_Temp_SP_path);
				variable.HeatAssist_Timer = client.ReadNode(UnitOfWorkOPCPath.HeatAssist_Timer_path);
				//variable.Heat_Assist_ON = client.ReadNode(UnitOfWorkOPCPath.Heat_Assist_ON_path);
				//variable.Heat_Assist_Timer_SP = client.ReadNode(UnitOfWorkOPCPath.Heat_Assist_Timer_SP_path);
				//variable.Heat_Done = client.ReadNode(UnitOfWorkOPCPath.Heat_Done_path);
				//variable.MainPressure = client.ReadNode(UnitOfWorkOPCPath.MainPressure_path);
				//variable.ManualSetTemp = client.ReadNode(UnitOfWorkOPCPath.ManualSetTemp_path);
				//variable.PneumaticPressure = client.ReadNode(UnitOfWorkOPCPath.PneumaticPressure_path);
				//variable.PreHeat_Done = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Done_path);
				//variable.PreHeat_Stage = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Stage_path);
				//variable.PreHeat_Start = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Start_path);
				//variable.PreHeat_Temp_SP = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Temp_SP_path);
				variable.PreHeat_Timer = client.ReadNode(UnitOfWorkOPCPath.PreHeat_Timer_path);

				ReadAIValues(client);
				ReadDiscretValues(client);
				ReadIntegerValues(client);
				objects.setBAV_3_Status(BAV_3_Status);
				objects.setBAV_3_Input(BAV_3_Input);
				objects.set_CVP_Input(CPV_Input);
				objects.set_CVP_Status(CPV_Status);
				objects.set_FVV_B_Input(FVV_B_Input);
				objects.set_FVV_B_Status(FVV_B_Status);
				objects.set_FVV_S_Input(FVV_S_Input);
				objects.set_FVV_S_Status(FVV_S_Status);
				objects.set_SHV_Input(SHV_Input);
				objects.set_SHV_Status(SHV_Status);
				objects.SetCrioInput(crioInput);
				objects.SetCrioStatus(crioStatus);
				objects.SetStopFvp(StopFVP);
				objects.SetStopCrio(StopCrio);
				objects.set_OpcClient(client);
				objects.SetOpenCam(OpenCam);
				objects.set_CrioPumpStart(CrioPumpStart);
				objects.set_camPrepare(CamPrepare);
				objects.SetIonStatus(IonStatus);
				objects.SetIonOutputFeedBack(IonOutputFeedBack);
				objects.SetIonInputCommand(IonInputCommand);
				objects.SetAnalogInput(variable);
			}


		}
		public void TimerRead(object obj)
		{
			Log.Information("OPCUpdate");
			ReadOPCData(obj);
			_opcHandler("OPC Server data update");
		}
		public void Dispose()
		{

		}
	}
}
