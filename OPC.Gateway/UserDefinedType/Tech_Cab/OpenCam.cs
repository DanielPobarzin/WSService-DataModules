extern alias UaFx;
using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.Tech_Cab
{
    [OpcDataType("ns=3;s=DT_\"Open_Cam\"")]
    [OpcDataTypeEncoding("ns=3;s=TE_\"Open_Cam\"")]
    public class OpenCam
    {
        public UInt16 Stage_1_stage { get; set; }
        public bool Access { get; set; }
        public bool Stage_1_Return { get; set; }
        public bool Stage_1_done { get; set; }
        public bool Heat_cam { get; set; }

    }
}
