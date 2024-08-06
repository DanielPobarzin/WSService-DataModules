using Application.Exceptions;
using Application.Features.Connections.Queries.GetConnectionDetails;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CLients.Queries.GetClient.GetDetails
{
	public class ClientDetailsQueryHandler :  IRequestHandler<ClientDetailsQuery, Response<ClientDetailsViewModel>>
	{
		private readonly IClientRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public ClientDetailsQueryHandler(IClientRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<ClientDetailsViewModel>> Handle(ClientDetailsQuery query, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(query.Id);
			if (client == null) throw new APIException($"Client Not Found.");
			return new Response<ClientDetailsViewModel>(_mapper.Map<ClientDetailsViewModel>(client), true);
		}
	}

}
