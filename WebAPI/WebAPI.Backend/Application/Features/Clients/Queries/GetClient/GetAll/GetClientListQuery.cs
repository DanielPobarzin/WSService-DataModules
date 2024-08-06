using Application.Parameters;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clients.Queries.GetClient.GetAll
{
	public class GetClientListQuery : IRequest<Response<IEnumerable<ClientLookupDTO>>>
	{

	}
}
