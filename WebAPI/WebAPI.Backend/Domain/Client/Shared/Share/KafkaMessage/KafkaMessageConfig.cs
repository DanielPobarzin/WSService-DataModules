using Interactors.Settings.ClientConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Share.KafkaMessage
{
    public class KafkaMessageConfig : KafkaMessageBase
    {
        public string JsonConfig { get; set; }
    }
}
