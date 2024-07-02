using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DO
{
		public class DomainObjectNotification
		{
			public Guid ClientId { get; set; }
			public Guid ServerId { get; set; }
			public Notification Notification { get; set; }
			public DateTime DateAndTimeSendDataByServer { get; set; }
			public DateTime DateAndTimeRecievedDataFromServer { get; set; }
		}
}
