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

		/// <summary>
		/// Initializes a new instance of the <see cref="GetConfigClientListQueryHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository to access client configuration data.</param>
		/// <param name="mapper">The AutoMapper instance for mapping entities to DTOs.</param>
		public GetConfigClientListQueryHandler(IClientConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		/// <summary>
		/// Handles the specified <see cref="GetConfigClientListQuery"/> and returns a list of client configuration lookups.
		/// </summary>
		/// <param name="request">The query containing the request to retrieve client configuration lookups.</param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A task that represents the asynchronous operation, containing a <see cref="Response{T}"/> with a list of client configuration lookups.</returns>
		/// <exception cref="APIException">Thrown when no client configurations are found.</exception>
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
