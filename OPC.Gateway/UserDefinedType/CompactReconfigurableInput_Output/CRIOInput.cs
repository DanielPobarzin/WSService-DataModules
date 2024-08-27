using Opc.UaFx;

namespace OPC.Gateway.UDT.cRIO
{
	[OpcDataType("ns=3;s=DT_\"CRIOInput\"")]
    [OpcDataTypeEncoding("ns=3;s=TE_\"CRIOInput\"")]
    public class CRIOInput
    {
        public bool Auto_mode { get; set; }
        public bool Command_manual { get; set; }
        public bool Command_auto { get; set; }
        public bool Block { get; set; }
        public bool Power_on { get; set; }
        public bool Switch_on { get; set; }

    }
}
