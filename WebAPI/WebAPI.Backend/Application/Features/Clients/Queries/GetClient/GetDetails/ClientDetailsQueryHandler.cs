using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.CLients.Queries.GetClient.GetDetails
{
	public class ClientDetailsQueryHandler : IRequestHandler<ClientDetailsQuery, Response<ClientDetailsViewModel>>
	{
		private readonly IClientRepositoryAsync _repository;
		private readonly IMapper _mapper;

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientDetailsQueryHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository for working with clients.</param>
		/// <param name="mapper">An object for converting entities to view models.</param>
		public ClientDetailsQueryHandler(IClientRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		/// <summary>
		/// Handles the request to retrieve details of a specific client.
		/// </summary>
		/// <param name="query">The request containing the client ID.</param>
		/// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
		/// <returns>
		/// An object of type <see cref="Response{T}"/> containing the client details view model.
		/// An <see cref="APIException"/> is thrown if the client is not found.
		/// </returns>
		/// <exception cref="APIException">Thrown when the client is not found.</exception>
		public async Task<Response<ClientDetailsViewModel>> Handle(ClientDetailsQuery query, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(query.Id);
			if (client == null) throw new APIException($"Client Not Found.");
			return new Response<ClientDetailsViewModel>(_mapper.Map<ClientDetailsViewModel>(client), true);
		}
	}
}
