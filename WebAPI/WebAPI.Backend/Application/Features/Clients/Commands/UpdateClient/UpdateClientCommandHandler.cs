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
		public UpdateClientCommandHandler(IClientRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
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
