using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Features.Servers.Queries.GetServer.GetAll
{
	public class GetServerListQueryHandler : IRequestHandler<GetServerListQuery, Response<IEnumerable<ServerLookupDTO>>>
	{
		private readonly IServerRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public GetServerListQueryHandler(IServerRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<IEnumerable<ServerLookupDTO>>> Handle(GetServerListQuery request, CancellationToken cancellationToken)
		{
			var servers = await _repository.GetAllAsync();
			if (servers.Item2 == 0) throw new APIException($"Servers Not Found.");

			var serverDtos = servers.Item1
				.AsQueryable() 
				.ProjectTo<ServerLookupDTO>(_mapper.ConfigurationProvider)
				.ToList();

			return new Response<IEnumerable<ServerLookupDTO>>(serverDtos, true, servers.Item2);
		}
	}
}
