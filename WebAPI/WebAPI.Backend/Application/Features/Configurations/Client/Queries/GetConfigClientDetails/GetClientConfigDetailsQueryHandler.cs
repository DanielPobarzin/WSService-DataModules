using Application.Exceptions;
using Application.Features.Configurations.Server.Queries.GetConfigServerDetails;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientDetails
{
	/// <summary>
	/// Handles the query to retrieve client configuration details.
	/// Implements <see cref="IRequestHandler{TRequest, TResponse}"/> for <see cref="GetClientConfigDetailsQuery"/>.
	/// </summary>
	public class GetClientConfigDetailsQueryHandler : IRequestHandler<GetClientConfigDetailsQuery, Response<ClientConfigDetailsViewModel>>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		/// <summary>
		/// Initializes a new instance of the <see cref="GetClientConfigDetailsQueryHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository to access client configuration data.</param>
		/// <param name="mapper">The AutoMapper instance for mapping entities to view models.</param>
		public GetClientConfigDetailsQueryHandler(IClientConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		/// <summary>
		/// Handles the specified <see cref="GetClientConfigDetailsQuery"/> and returns the client configuration details.
		/// </summary>
		/// <param name="query">The query containing the identifier of the client configuration to retrieve.</param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A task that represents the asynchronous operation, containing a <see cref="Response{T}"/> with the client configuration details.</returns>
		/// <exception cref="APIException">Thrown when the configuration is not found.</exception>
		public async Task<Response<ClientConfigDetailsViewModel>> Handle(GetClientConfigDetailsQuery query, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(query.Id);
			if (config == null) throw new APIException($"Configuration Not Found.");
			return new Response<ClientConfigDetailsViewModel>(_mapper.Map<ClientConfigDetailsViewModel>(config), true);
		}
	}
}
