using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
	public class Alarm : BaseEntity
	{
		public double Value { get; set; }
		public char Quality { get; set; }
	}
}
