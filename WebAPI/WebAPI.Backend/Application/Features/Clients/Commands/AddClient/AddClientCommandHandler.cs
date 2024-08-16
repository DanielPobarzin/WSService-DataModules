using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Clients.Commands.AddClient
{
	public class AddClientCommandHandler : IRequestHandler<AddClientCommand, Response<Client>>
	{
		private readonly IClientRepositoryAsync _repository;
		private readonly IMapper _mapper;

		/// <summary>
		/// Initializes a new instance of the <see cref="AddClientCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository interface for working with clients.</param>
		/// <param name="mapper">The interface for object mapping.</param>
		public AddClientCommandHandler(IClientRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		/// <summary>
		/// Handles the command to add a client.
		/// </summary>
		/// <param name="command">The command containing client data to be added.</param>
		/// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
		/// <returns>
		/// An instance of <see cref="Response{Client}"/> containing the added client and the operation status.
		/// </returns>
		/// <exception cref="APIException">Thrown if a client with the specified ID already exists.</exception>
		public async Task<Response<Client>> Handle(AddClientCommand command, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(command.ClientId);
			if (client != null) throw new APIException($"Client has already been added.");

			client = _mapper.Map<Client>(command);
			await _repository.AddAsync(client);

			return new Response<Client>(client, true);
		}
	}
}
