using Application.Exceptions;
using Application.Features.Servers.Queries.GetServer.GetAll;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Features.Configurations.Server.Queries.GetConfigServerList
{
	public class GetConfigServerListQueryHandler : IRequestHandler<GetConfigServerListQuery, Response<IEnumerable<ConfigServerLookupDTO>>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public GetConfigServerListQueryHandler(IServerConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<IEnumerable<ConfigServerLookupDTO>>> Handle(GetConfigServerListQuery request, CancellationToken cancellationToken)
		{
			var configs = await _repository.GetAllAsync();
			if (configs.Item2 == 0) throw new APIException($"Configs Servers Not Found.");

			var configDtos = configs.Item1
				.AsQueryable()
				.ProjectTo<ConfigServerLookupDTO>(_mapper.ConfigurationProvider)
				.ToList();

			return new Response<IEnumerable<ConfigServerLookupDTO>>(configDtos, true, configs.Item2);
		}
	}
}
