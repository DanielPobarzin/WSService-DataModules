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

		/// <summary>
		/// Initializes a new instance of the <see cref="GetClientListQueryHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository for working with clients.</param>
		/// <param name="mapper">An object for converting entities to DTOs.</param>
		public GetClientListQueryHandler(IClientRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		/// <summary>
		/// Handles the request to retrieve a list of clients.
		/// </summary>
		/// <param name="request">The request for retrieving the list of clients.</param>
		/// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
		/// <returns>
		/// An object of type <see cref="Response{T}"/> containing a list of client DTOs.
		/// An <see cref="APIException"/> is thrown if no clients are found.
		/// </returns>
		/// <exception cref="APIException">Thrown when no clients are found.</exception>
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
