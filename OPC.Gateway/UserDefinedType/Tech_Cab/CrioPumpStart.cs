extern alias UaFx;
using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.Tech_Cab
{
	[OpcDataType("ns=3;s=DT_\"CP_check\"")]
    [OpcDataTypeEncoding("ns=3;s=TE_\"CP_check\"")]
    public class CrioPumpStart
    {
        public float Crio_pressure_check_SP { get; set; }
        public float Crio_pressure_calc_value { get; set; }
        public float Crio_pressure_diff { get; set; }
        public float Temperature_SP { get; set; }
        public UInt16 Stage_0_Stage { get; set; }
        public bool Access { get; set; }
        public bool Return_error { get; set; }
        public bool Stage_0_CompliteP { get; set; }

    }
}
