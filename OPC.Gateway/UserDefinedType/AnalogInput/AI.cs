extern alias UaFx;
using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.AnalogInput
{
	public class AI
    {
        public OpcValue SFT01_FT { get; set; }
        public OpcValue SFT02_FT { get; set; }
        public OpcValue SFT03_FT { get; set; }
        public OpcValue SFT04_FT { get; set; }
        public OpcValue SFT05_FT { get; set; }
        public OpcValue SFT06_FT { get; set; }
        public OpcValue SFT07_FT { get; set; }
        public OpcValue SFT08_FT { get; set; }
        public OpcValue SFT09_FT { get; set; }
        public OpcValue SFT10_FT { get; set; }
        public OpcValue FT_TT_1 { get; set; }
        public OpcValue FT_TT_2 { get; set; }
        public OpcValue FT_TT_3 { get; set; }
        public OpcValue CVG_9HP1_feedback { get; set; }
        public OpcValue CVG_9LP2_feedback { get; set; }
        public OpcValue SVG_9HP3_feedback { get; set; }
        public OpcValue SVG_9LP4_feedback { get; set; }
		public OpcValue OilPressure { get; set; }
		public OpcValue PressureWorkingChamber { get; set; }
        public OpcValue SuctionPressure { get; set; }
        public OpcValue PositionIndicatorAxleBox { get; set; }
		public OpcValue PositionIndicator { get; set; }
		public OpcValue CoolantTemperature { get; set; }

        public OpcValue ReciprocatingFull_Timer { get; set; }
        public OpcValue RreciprocatingPart_Timer { get; set; }

       
        public OpcValue ReciprocatingFull_Stage { get; set; }
        public OpcValue RreciprocatingPart_Stage { get; set; }
        

        public OpcValue ReciprocatingFull_Done { get; set; }
        public OpcValue RreciprocatingPart_Done { get; set; }
        public OpcValue CalcInsensitivity_Start { get; set; }
        public OpcValue Reciprocating_Flag { get; set; }

        public OpcValue ManualSetTemp { get; set; }

        public OpcValue CV_Speed { get; set; }
        
        public OpcValue CV_Start { get; set; }
        public OpcValue CV_Stop { get; set; }
        public OpcValue CV_Remote_Control_Done { get; set; }
		public OpcValue Alarm_Diag { get; set; }
		public OpcValue Alarm_Open_door { get; set; }
        public OpcValue Alarm_Temperature_conditioner { get; set; }
        public OpcValue Alarm_Hight_Pressure { get; set; }
        public OpcValue Alarm_Low_Pressure { get; set; }
        public OpcValue Alarm_power_failure { get; set; }

        public OpcValue Alarm_High_Humidity { get; set; }
        public OpcValue Alarm_fire { get; set; }
        public OpcValue Notify_Checking_Pacing { get; set; }
		public OpcValue Notify_Performing_Insensitivity { get; set; }
		public OpcValue Notify_Position_Tracking { get; set; }
		public OpcValue Notify_Check_Open_State { get; set; }
		public OpcValue Notify_Perform_Diagnostic_Data_Analysis { get; set; }


		public OpcValue Tech_cam_STAGE { get; set; }
        
        public OpcValue Alarm_manual_Stop { get; set; }



    }
}
