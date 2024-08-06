using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CLients.Queries.GetClient.GetDetails
{
	public class ClientDetailsQuery : IRequest<Response<ClientDetailsViewModel>>
	{
		public Guid Id { get; set; }
	}
}
