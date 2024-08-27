extern alias UaFx;

using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.Valves
{
	///<summaray>
	///Класс ValveInput является представлением 
	///OPC DataType "ns=3;s=DT_\"valve\".\"Input\"
	///</summaray>
	[OpcDataType("ns=3;s=DT_\"ValveInput\"")]
    [OpcDataTypeEncoding("ns=3;s=TE_\"ValveInput\"")]
    public class ValveInput
    {
        public bool Service_mode { get; set; }
        public bool Auto_mode { get; set; }
        public bool Man_command { get; set; }
        public bool Open_close { get; set; } 
        public bool Block { get; set; }
        public bool Opened_signal { get; set; }
        public bool Closed_signal { get; set; }

}

}
