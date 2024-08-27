extern alias UaFx;

using Opc.Ua;
using OPC.Gateway.DB.Entities;
using OPC.Gateway.UDT.AnalogInput;
using OPC.Gateway.UDT.cRIO;
using OPC.Gateway.UDT.DiscreteInput;
using OPC.Gateway.UDT.IntValue;
using OPC.Gateway.UDT.Tech_Cab;
using OPC.Gateway.UDT.Valves;
using UaFx::Opc.UaFx.Client;

namespace OPC.Gateway.DTOs
{
	public class OPCObjects 
	{
		public static OpcClient client;
		public static ValveStatus HPCV_1_status, HPCV_2_status, HPCV_3_status, HPCV_4_status,
								  LPCV_1_status, LPCV_2_status, LPCV_3_status, LPCV_4_status,
								  LPSV_1_status, LPSV_2_status, LPSV_3_status, LPSV_4_status,
								  HPSV_1_status, HPSV_2_status, HPSV_3_status, HPSV_4_status;
		public static ValveInput  HPCV_1_input, HPCV_2_input, HPCV_3_input, HPCV_4_input,
								  LPCV_1_input, LPCV_2_input, LPCV_3_input, LPCV_4_input,
								  LPSV_1_input, LPSV_2_input, LPSV_3_input, LPSV_4_input,
								  HPSV_1_input, HPSV_2_input, HPSV_3_input, HPSV_4_input;
		public static CRIOInput CrioInput;
		public static CabPrepare cabPrepare;
		public static CRIOStatus CrioStatus;
		public static AI AnalogInput;
		public static object SQLLocker;
		public static object OPCLocker;
		public static List<AIValue> AIValues;
		public static List<DIValue> DIValues;
		public static List<IntValue> IntValues;
		public static Dictionary<int, ValveInput> ValvesInput;
		public static Dictionary<int, ValveStatus> ValvesStatus;
		public static AIValue SFT01_FT, SFT02_FT, SFT03_FT, SFT04_FT,
								   SFT05_FT, SFT06_FT, SFT07_FT, SFT08_FT,
								   SFT09_FT, SFT10_FT, FT_TT_1, FT_TT_2, FT_TT_3,
									RRG_9A1_feedback, RRG_9A2_feedback,
									RRG_9A3_feedback, RRG_9A4_feedback,
									TE_1, Pneumatic_Pressure, Crio_Pressure,
									Camera_Pressure, Main_Pressure, Crio_Temperature,
									PreHeat_Temp_SP, HeatAssist_Temp_SP,
									PreHeat_Timer_SP, HeatAssist_Timer_SP,
									ManualSetTemp, BLM_Speed, BLM_Speed_SP,
									K_RRG1, K_RRG2, K_RRG3, K_RRG4, RRG_Pressure_SP, PidHeatMode;
		public static DIValue PreHeat_Done, HeatAssist_Done, PreHeat_Start,
									HeatAssist_Flag, Heat_Done, HeatAssist_TempDone,
									Heat_Assit_On, BLM_Start, BLM_Stop, BLM_Remote_Control_Done,
									BLM_Run, Alarm_Open_door, Alarm_Water_CRIO,
									Alarm_Hight_Pne_Press, Alarm_Low_One_Presse,
									Alarm_Crio_power_failure, Alarm_Qartz_power_failure,
									Alarm_ELI_Power_failure, Alarm_FloatHeater_power_failure,
									Alarm_Ion_power_failure, Alarm_FVP_power_failure,
									Alarm_Indexer_power_failure, Alarm_SSP_power_failure,
									Alarm_TV1_power_failure, Alarm_Water_SECOND, Alarm_Hight_Crio_Temp,
									Crio_start_signal, Alarm_manual_stop, StartProcessSignal, StopProcessSignal,
									ELI_complete, ELI_access;
		public static IntValue PreHeat_Stage;
		public static IntValue HeatAssist_Stage;
		public static IntValue Tech_cam_STAGE;
		public static IntValue FullCycleStage;
		public static User user;

		#region constructor
		private static OPCObjects instance;
		private OPCObjects()
		{
			cabPrepare = new CabPrepare();
			AIValues = new List<AIValue>();
			DIValues = new List<DIValue>();
			ValvesInput = new Dictionary<int, ValveInput>();
			ValvesStatus = new Dictionary<int, ValveStatus>();
			SFT01_FT = new AIValue();
			SFT02_FT = new AIValue();
			SFT03_FT = new AIValue();
			SFT04_FT = new AIValue();
			SFT05_FT = new AIValue();
			SFT06_FT = new AIValue();
			SFT07_FT = new AIValue();
			SFT08_FT = new AIValue();
			SFT09_FT = new AIValue();
			SFT10_FT = new AIValue();
			FT_TT_1 = new AIValue();
			FT_TT_2 = new AIValue();
			FT_TT_3 = new AIValue();
			K_RRG1 = new AIValue();
			K_RRG2 = new AIValue();
			K_RRG3 = new AIValue();
			K_RRG4 = new AIValue();
			PidHeatMode = new AIValue();
			RRG_Pressure_SP = new AIValue();
			RRG_9A1_feedback = new AIValue();
			RRG_9A2_feedback = new AIValue();
			RRG_9A3_feedback = new AIValue();
			RRG_9A4_feedback = new AIValue();
			TE_1 = new AIValue();
			Pneumatic_Pressure = new AIValue();
			Crio_Pressure = new AIValue();
			Camera_Pressure = new AIValue();
			Main_Pressure = new AIValue();
			Crio_Temperature = new AIValue();
			PreHeat_Temp_SP = new AIValue();
			HeatAssist_Temp_SP = new AIValue();
			PreHeat_Timer_SP = new AIValue();
			HeatAssist_Timer_SP = new AIValue();
			ManualSetTemp = new AIValue();
			BLM_Speed = new AIValue();
			BLM_Speed_SP = new AIValue();
			PreHeat_Done = new DIValue();
			HeatAssist_Done = new DIValue();
			PreHeat_Start = new DIValue();
			HeatAssist_Flag = new DIValue();
			Heat_Done = new DIValue();
			HeatAssist_TempDone = new DIValue();
			Heat_Assit_On = new DIValue();
			BLM_Start = new DIValue();
			BLM_Stop = new DIValue();
			BLM_Remote_Control_Done = new DIValue();
			BLM_Run = new DIValue();
			Alarm_Open_door = new DIValue();
			Alarm_Water_CRIO = new DIValue();
			Alarm_Hight_Pne_Press = new DIValue();
			Alarm_Low_One_Presse = new DIValue();
			Alarm_Crio_power_failure = new DIValue();
			Alarm_Qartz_power_failure = new DIValue();
			Alarm_ELI_Power_failure = new DIValue();
			Alarm_FloatHeater_power_failure = new DIValue();
			Alarm_Ion_power_failure = new DIValue();
			Alarm_FVP_power_failure = new DIValue();
			Alarm_Indexer_power_failure = new DIValue();
			Alarm_SSP_power_failure = new DIValue();
			Alarm_TV1_power_failure = new DIValue();
			Alarm_Water_SECOND = new DIValue();
			Alarm_Hight_Crio_Temp = new DIValue();
			Crio_start_signal = new DIValue();
			Alarm_manual_stop = new DIValue();
			StartProcessSignal = new DIValue();
			StopProcessSignal = new DIValue();
			ELI_complete = new DIValue();
			ELI_access = new DIValue();
			PreHeat_Stage = new IntValue();
			HeatAssist_Stage = new IntValue();
			Tech_cam_STAGE = new IntValue();
			FullCycleStage = new IntValue();
			IntValues = new List<IntValue>();
			OPCLocker = new object();
			SQLLocker = new object();
			HPCV_1_status = new ValveStatus();
			HPCV_2_status = new ValveStatus();
			HPCV_3_status = new ValveStatus();
			HPCV_4_status = new ValveStatus();
			LPCV_1_status = new ValveStatus();
			LPCV_2_status = new ValveStatus();
			LPCV_3_status = new ValveStatus();
			LPCV_4_status = new ValveStatus();
			LPSV_1_status = new ValveStatus();
			LPSV_2_status = new ValveStatus();
			LPSV_3_status = new ValveStatus();
			LPSV_4_status = new ValveStatus();
			HPSV_1_status = new ValveStatus();
			HPSV_2_status = new ValveStatus();
			HPSV_3_status = new ValveStatus();
			HPSV_4_status = new ValveStatus();
			HPCV_1_input = new ValveInput();
			HPCV_2_input = new ValveInput();
			HPCV_3_input = new ValveInput();
			HPCV_4_input = new ValveInput();
			LPCV_1_input = new ValveInput();
			LPCV_2_input = new ValveInput();
			LPCV_3_input = new ValveInput();
			LPCV_4_input = new ValveInput();
			LPSV_1_input = new ValveInput();
			LPSV_2_input = new ValveInput();
			LPSV_3_input = new ValveInput();
			LPSV_4_input = new ValveInput();
			HPSV_1_input = new ValveInput();
			HPSV_2_input = new ValveInput();
			HPSV_3_input = new ValveInput();
			HPSV_4_input = new ValveInput();
			cabPrepare = new CabPrepare();
			CrioPumpStart = new CrioPumpStart();
			openCam = new OpenCam();
			StopCrio = new StopCrio();
			StopFVP = new StopFVP();
			IonInputCommnd = new IonInputCommand();
			IonInputSetPoint = new IonInputSetPoint();
			IonOutputFeedBack = new IonOutputFeedBack();
			IonStatus = new IonStatus();
			FVPStatus = new FVPStatus();
			CrioInput = new CrioInput();
			CrioStatus = new CrioStatus();
			user = new User();
		}

		public static OPCObjects createObjects()
		{
			if (instance == null)
			{
				instance = new OPCObjects();
			}
			return instance;
		}
		#endregion

		#region Tech_cam
		public void set_camPrepare(CamPrepare obj)
		{
			camPrepare = obj;
		}
		public CamPrepare get_camPrepare()
		{
			return camPrepare;
		}

		public void set_CrioPumpStart(CrioPumpStart obj)
		{
			CrioPumpStart = obj;
		}
		public CrioPumpStart GetCrioPumpStart()
		{
			return CrioPumpStart;
		}

		public void SetOpenCam(OpenCam obj)
		{
			openCam = obj;
		}
		public OpenCam GetOpenCam()
		{
			return openCam;
		}

		public void SetStopCrio(StopCrio obj)
		{
			StopCrio = obj;
		}
		public StopCrio GetStopCrio()
		{
			return StopCrio;
		}

		public void SetStopFvp(StopFVP obj)
		{
			StopFVP = obj;
		}
		public StopFVP GetStopFVP()
		{
			return StopFVP;
		}
		#endregion
		#region ION
		public void SetIonInputCommand(IonInputCommand obj)
		{
			IonInputCommnd = obj;
		}
		public IonInputCommand GetIonInputCommand()
		{
			return IonInputCommnd;
		}
		public void SetIonInputSetPoint(IonInputSetPoint obj)
		{
			IonInputSetPoint = obj;
		}
		public IonInputSetPoint GetIonInputSetPoint()
		{
			return IonInputSetPoint;
		}
		public void SetIonOutputFeedBack(IonOutputFeedBack obj)
		{
			IonOutputFeedBack = obj;
		}
		public IonOutputFeedBack GetIonOutputFeedBack()
		{
			return IonOutputFeedBack;
		}
		public void SetIonStatus(IonStatus obj)
		{
			IonStatus = obj;
		}
		public IonStatus GetIonStatus()
		{
			return IonStatus;
		}
		#endregion
		public void setSQLLocker(object obj)
		{
			SQLLocker = obj;
		}
		public object getSQLLocker()
		{
			return SQLLocker;
		}
		public void setOPCLocker(object obj)
		{
			OPCLocker = obj;
		}
		public object getOPCLocker()
		{
			return OPCLocker;
		}

		public void SetAnalogInput(AnalogInput obj)
		{
			AnalogInput = obj;
		}
		public AnalogInput GetAnalogInput()
		{
			return AnalogInput;
		}

		public void SetFVPStatus(FVPStatus obj)
		{
			FVPStatus = obj;
		}
		public FVPStatus GetFVPStatus()
		{
			return FVPStatus;
		}

		public void SetCrioInput(CrioInput obj)
		{
			CrioInput = obj;
		}
		public CrioInput GetCrioInput()
		{
			return CrioInput;
		}

		public void SetCrioStatus(CrioStatus obj)
		{
			CrioStatus = obj;
		}
		public CrioStatus GetCrioStatus()
		{
			return CrioStatus;
		}

		public void set_OpcClient(OpcClient client)
		{
			client = client;
		}
		public OpcClient get_OpcClietn()
		{
			return client;
		}
		public void setBAV_3_Status(ValveStatus obj)
		{
			BAV_3_status = obj;
		}
		public ValveStatus getBAV_3_Status()
		{
			return BAV_3_status;
		}

		public void setBAV_3_Input(ValveInput obj)
		{
			BAV_3_input = obj;
		}
		public ValveInput getBAV_3_Input()
		{
			return BAV_3_input;
		}

		public void set_FVV_S_Status(ValveStatus obj)
		{
			FVV_S_Status = obj;
		}
		public ValveStatus get_FVV_S_Status()
		{
			return FVV_S_Status;
		}
		public void set_FVV_S_Input(ValveInput obj)
		{
			FVV_S_Input = obj;
		}
		public ValveInput get_FVV_S_Input()
		{
			return FVV_S_Input;
		}


		public void set_FVV_B_Status(ValveStatus obj)
		{
			FVV_B_Status = obj;
		}
		public ValveStatus get_FVV_B_Status()
		{
			return FVV_B_Status;
		}
		public void set_FVV_B_Input(ValveInput obj)
		{
			FVV_B_Input = obj;
		}
		public ValveInput get_FVV_B_Input()
		{
			return FVV_B_Input;
		}

		public void set_CVP_Status(ValveStatus obj)
		{
			CPV_Status = obj;
		}
		public ValveStatus get_CVP_Status()
		{
			return CPV_Status;
		}
		public void set_CVP_Input(ValveInput obj)
		{
			CPV_Input = obj;
		}
		public ValveInput get_CVP_Input()
		{
			return CPV_Input;
		}


		public void set_SHV_Status(ValveStatus obj)
		{
			SHV_Status = obj;
		}
		public ValveStatus get_SHV_Status()
		{
			return SHV_Status;
		}
		public void set_SHV_Input(ValveInput obj)
		{
			SHV_Input = obj;
		}
		public ValveInput get_SHV_Input()
		{
			return SHV_Input;
		}
		public void SetFT_TT_1(AIValue obj)
		{
			FT_TT_1 = obj;
		}
		public AIValue GetFT_TT_1()
		{
			return FT_TT_1;
		}
		public void SetFT_TT_2(AIValue obj)
		{
			FT_TT_2 = obj;
		}
		public AIValue GetFT_TT_2()
		{
			return FT_TT_2;
		}
		public void SetFT_TT_3(AIValue obj)
		{
			FT_TT_3 = obj;
		}
		public AIValue GetFT_TT_3()
		{
			return FT_TT_3;
		}
		public void SetHeatAssist_Temp_SP(AIValue obj)
		{
			HeatAssist_Temp_SP = obj;
		}
		public AIValue GetHeatAssist_Temp_SP()
		{
			return HeatAssist_Temp_SP;
		}
		public void SetHeatAssist_Timer_SP(AIValue obj)
		{
			HeatAssist_Timer_SP = obj;
		}
		public AIValue GetHeatAssist_Timer_SP()
		{
			return HeatAssist_Timer_SP;
		}
		public void SetMainPres(AIValue obj)
		{
			Main_Pressure = obj;
		}
		public AIValue GetMainPres()
		{
			return Main_Pressure;
		}
		public void SetManualSetTemp(AIValue obj)
		{
			ManualSetTemp = obj;
		}
		public AIValue GetManualSetTemp()
		{
			return ManualSetTemp;
		}
		public void SetPneumaticPres(AIValue obj)
		{
			Pneumatic_Pressure = obj;
		}
		public AIValue GetPneumaticPres()
		{
			return Pneumatic_Pressure;
		}
		public void SetPreHeat_Temp_SP(AIValue obj)
		{
			PreHeat_Temp_SP = obj;
		}
		public AIValue GetPreHeat_Temp_SP()
		{
			return PreHeat_Temp_SP;
		}
		public void SetPreHeat_Timer_SP(AIValue obj)
		{
			PreHeat_Timer_SP = obj;
		}
		public AIValue GetPreHeat_Timer_SP()
		{
			return PreHeat_Timer_SP;
		}



	}
}


