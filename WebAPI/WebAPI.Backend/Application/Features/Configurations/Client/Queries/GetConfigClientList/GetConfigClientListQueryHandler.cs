using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientList
{
	public class GetConfigClientListQueryHandler : IRequestHandler<GetConfigClientListQuery, Response<IEnumerable<ConfigClientLookupDTO>>>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public GetConfigClientListQueryHandler(IClientConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<IEnumerable<ConfigClientLookupDTO>>> Handle(GetConfigClientListQuery request, CancellationToken cancellationToken)
		{
			var configs = await _repository.GetAllAsync();
			if (configs.Item2 == 0) throw new APIException($"Configs Clients Not Found.");

			var configDtos = configs.Item1
				.AsQueryable()
				.ProjectTo<ConfigClientLookupDTO>(_mapper.ConfigurationProvider)
				.ToList();

			return new Response<IEnumerable<ConfigClientLookupDTO>>(configDtos, true, configs.Item2);
		}
	}
}
