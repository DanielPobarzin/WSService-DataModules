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

		public AddClientCommandHandler(IClientRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
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
