using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Servers.Commands.AddServer
{
	public class AddServerCommandHandler : IRequestHandler<AddServerCommand, Response<Server>>
	{
		private readonly IServerRepositoryAsync _repository;
		private readonly IMapper _mapper;

		public AddServerCommandHandler(IServerRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<Server>> Handle(AddServerCommand command, CancellationToken cancellationToken)
		{
			var server = await _repository.GetByIdAsync(command.ServerId);
			if (server != null) throw new APIException($"Server has already been added.");
			server = _mapper.Map<Server>(command);
			await _repository.AddAsync(server);
			return new Response<Server>(server, true);
		}
	}
}
