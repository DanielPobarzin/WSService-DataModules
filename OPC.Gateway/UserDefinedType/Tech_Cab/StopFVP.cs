extern alias UaFx;
using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.Tech_Cab
{
	[OpcDataType("ns=3;s=DT_\"Stop_FVP\"")]
    [OpcDataTypeEncoding("ns=3;s=TE_\"Stop_FVP\"")]
    public class StopFVP
    {
        public bool Access { get; set; }
        public bool Stage_3_Return { get; set; }
        public bool Stage_3_Done { get; set; }

    }
}
