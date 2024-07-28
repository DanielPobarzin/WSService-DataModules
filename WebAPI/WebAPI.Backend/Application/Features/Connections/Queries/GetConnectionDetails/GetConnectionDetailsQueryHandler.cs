using Application.Exceptions;
using Application.Features.Connections.Queries.GetConnectionsList;
using Application.Interfaces.Repositories;
using Application.Parameters;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Connections.Queries.GetConnectionDetails
{
	public class GetConnectionDetailsQueryHandler : IRequestHandler<GetConnectionDetalisQuery, Response<ConnectionDetailsViewModel>>
	{
		private readonly IConnectionRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public GetConnectionDetailsQueryHandler(IConnectionRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<Response<ConnectionDetailsViewModel>> Handle(GetConnectionDetalisQuery query, CancellationToken cancellationToken)
		{
			var entity = await _repository.GetByConnectionIdAsync(query.ConnectionId);
			if (entity == null) throw new APIException($"Connection Not Found.");
			return new Response<ConnectionDetailsViewModel>(_mapper.Map<ConnectionDetailsViewModel>(entity), true);
		}
	}
}
