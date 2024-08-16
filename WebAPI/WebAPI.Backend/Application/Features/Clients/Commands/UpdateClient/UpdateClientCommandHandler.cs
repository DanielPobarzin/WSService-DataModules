using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Clients.Commands.UpdateClient
{
	public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Response<Client>>
	{
		private readonly IClientRepositoryAsync _repository;
		private readonly IMapper _mapper;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateClientCommandHandler"/> class.
		/// </summary>
		/// <param name="repository">The repository for working with clients.</param>
		/// <param name="mapper">The object for mapping data.</param>
		public UpdateClientCommandHandler(IClientRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		/// <summary>
		/// Handles the client update command.
		/// </summary>
		/// <param name="command">The update command containing the data to be updated.</param>
		/// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
		/// <returns>
		/// An object of type <see cref="Response{Client}"/> containing the updated client and the status of the operation.
		/// </returns>
		/// <exception cref="APIException">
		/// Thrown if a client with the specified identifier is not found.
		/// </exception>
		public async Task<Response<Client>> Handle(UpdateClientCommand command, CancellationToken cancellationToken)
		{
			var client = await _repository.GetByIdAsync(command.ClientId);
			if (client == null) throw new APIException($"Client Not Found.");

			client = _mapper.Map<Client>(command);

			client.ConnectionId = command.ConnectionId;
			client.ConnectionStatus = command.ConnectionStatus;
			client.WorkStatus = command.WorkStatus;

			await _repository.UpdateAsync(client);
			return new Response<Client>(client, true);
		}
	}
}