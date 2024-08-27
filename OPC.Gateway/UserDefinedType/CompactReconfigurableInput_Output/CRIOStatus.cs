extern alias UaFx;

using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.cRIO
{
	[OpcDataType("ns=3;s=DT_\"Crio\".\"Status\"")]
    [OpcDataTypeEncoding("ns=3;s=TE_\"Crio\".\"Status\"")]
    public class CRIOStatus
    {
        public bool Power_On { get; set; }
        public bool Blocked { get; set; }
        public bool Turn_On { get; set; }
        public bool Auto_mode { get; set; }
        public bool Error { get; set; }
    }
}
