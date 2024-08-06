using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Configurations.Server.Queries.GetConfigServerDetails
{
	public class GetServerConfigDetailsQueryHandler : IRequestHandler<GetServerConfigDetailsQuery, Response<ServerConfigDetailsViewModel>>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public GetServerConfigDetailsQueryHandler(IServerConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<ServerConfigDetailsViewModel>> Handle(GetServerConfigDetailsQuery query, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(query.Id);
			if (config == null) throw new APIException($"Configuration Not Found.");
			return new Response<ServerConfigDetailsViewModel>(_mapper.Map<ServerConfigDetailsViewModel>(config), true);
		}
	}
}
