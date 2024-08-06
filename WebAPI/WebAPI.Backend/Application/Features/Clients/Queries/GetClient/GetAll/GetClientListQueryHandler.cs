using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Features.Clients.Queries.GetClient.GetAll
{
	public class GetClientListQueryHandler : IRequestHandler<GetClientListQuery, Response<IEnumerable<ClientLookupDTO>>>
	{
		private readonly IClientRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public GetClientListQueryHandler(IClientRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<IEnumerable<ClientLookupDTO>>> Handle(GetClientListQuery request, CancellationToken cancellationToken)
		{
			var clients = await _repository.GetAllAsync();
			if (clients.Item2 == 0) throw new APIException($"Clients Not Found.");

			var clientDtos = clients.Item1
				.AsQueryable() 
				.ProjectTo<ClientLookupDTO>(_mapper.ConfigurationProvider)
				.ToList();

			return new Response<IEnumerable<ClientLookupDTO>>(clientDtos, true, clients.Item2);
		}
	}
}
