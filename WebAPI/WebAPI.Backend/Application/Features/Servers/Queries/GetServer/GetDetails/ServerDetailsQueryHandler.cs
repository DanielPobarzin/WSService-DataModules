using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Servers.Queries.GetServer.GetDetails
{
	public class ServerDetailsQueryHandler :  IRequestHandler<ServerDetailsQuery, Response<ServerDetailsViewModel>>
	{
		private readonly IServerRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public ServerDetailsQueryHandler(IServerRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<ServerDetailsViewModel>> Handle(ServerDetailsQuery query, CancellationToken cancellationToken)
		{
			var server = await _repository.GetByIdAsync(query.Id);
			if (server == null) throw new APIException($"Server Not Found.");
			return new Response<ServerDetailsViewModel>(_mapper.Map<ServerDetailsViewModel>(server), true);
		}
	}

}
