extern alias UaFx;

using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.Tech_Cab
{
	[OpcDataType("ns=3;s=DT_\"Cab_Prepare\"")]
    [OpcDataTypeEncoding("ns=3;s=TE_\"Cab_Prepare\"")]
    public class CabPrepare
    {
        public UInt16 Stage_0_Cab_prepare_Stage { get; set; }
        public bool Access { get; set; }
        public bool Stage_0_Cab_prepare_Complite { get; set; }
        public bool Return_ERROR { get; set; }

    }
}
