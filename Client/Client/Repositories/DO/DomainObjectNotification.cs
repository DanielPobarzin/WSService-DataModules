using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DO
{
	public class DomainObjectNotification : DomainObject
	{
		public Notification Notification { get; set; }
	}
}
