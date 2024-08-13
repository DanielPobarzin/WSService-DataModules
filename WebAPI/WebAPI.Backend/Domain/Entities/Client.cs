using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class Client : Entity
	{
		[Required]
		public Guid Id { get; set; }
		[Required]
		public WorkStatus WorkStatus { get; set; }
		[Required]
		public ConnectionStatus ConnectionStatus { get; set; }
		public string? ConnectionId { get; set; }

		public ICollection<Connection> Connections { get; set; }
	}
}
