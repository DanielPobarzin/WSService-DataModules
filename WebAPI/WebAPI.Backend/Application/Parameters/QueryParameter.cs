using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Backend.Core.Application.Parameters
{
	public class QueryParameter
	{
		public virtual string OrderBy { get; set; }
		public virtual string Fields { get; set; }
	}
}
