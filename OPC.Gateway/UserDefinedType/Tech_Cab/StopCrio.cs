extern alias UaFx;
using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.Tech_Cab
{
    [OpcDataType("ns=3;s=DT_\"Stopcr\"")]
    [OpcDataTypeEncoding("ns=3;s=TE_\"Stopcr\"")]
    public class StopCrio
    {
        public UInt16 Stage_2_Stage { get; set; }
        public bool Access { get; set; }
        public bool Stage_2_Return { get; set; }
        public bool Stage_2_done { get; set; }

    }
}
