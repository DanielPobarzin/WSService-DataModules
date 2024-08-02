using Interactors.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Share.KafkaMessage
{
    public class KafkaMessageCommand : KafkaMessageBase
    {
        public ConnectionCommand Command { get; set; }
    }
}
