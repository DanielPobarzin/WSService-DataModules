extern alias UaFx;
using UaFx::Opc.UaFx;

namespace OPC.Gateway.UDT.Valves
{
	///<summaray>
	///Класс ValveStatus является представлением 
	///OPC DataType "ns=3;s=DT_\"valve\".\"Status\"
	///</summaray>
	[OpcDataType("ns=3;s=TD_\"valve\".\"Status\"")]
	[OpcDataTypeEncoding("ns=3;s=TE_\"valve\".\"Status\"")]
	public class ValveStatus
    {
        public bool Auto_mode { get; set; }
        public bool Opened { get; set; }
        public bool Closed { get; set; }
        public bool Opening { get; set; }
        public bool Closing { get; set; }
        public bool Blocked { get; set; }
        public bool Serviced { get; set; }
      

    }
}
