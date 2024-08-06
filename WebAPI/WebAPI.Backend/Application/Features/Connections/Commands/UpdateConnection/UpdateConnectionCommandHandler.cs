using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Connections.Commands.UpdateConnection
{
	public class UpdateConnectionCommandHandler : IRequestHandler<UpdateConnectionCommand, Response<string>>
	{
		private readonly IConnectionRepositoryAsync _connectionRepository;
		private readonly IMapper _mapper;

		public UpdateConnectionCommandHandler(IConnectionRepositoryAsync Repository, IMapper mapper)
		{
			_connectionRepository = Repository;
			_mapper = mapper;
		}
		public async Task<Response<string>> Handle(UpdateConnectionCommand command, CancellationToken cancellationToken)
		{
			var connection = await _connectionRepository.GetByConnectionIdAsync(command.ConnectionId);

				if (connection == null)
				{
					throw new APIException($"Connection Not Found.");
				}
				else
				{
			        connection = _mapper.Map<Connection>(command);
					await _connectionRepository.UpdateAsync(connection);

					return new Response<string>(connection.ConnectionId, true);
				}
		}
	}
}
