using Application.Exceptions;
using Application.Features.Configurations.Server.Queries.GetConfigServerDetails;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Configurations.Client.Queries.GetConfigClientDetails
{
	public class GetClientConfigDetailsQueryHandler : IRequestHandler<GetClientConfigDetailsQuery, Response<ClientConfigDetailsViewModel>>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		private readonly IMapper _mapper;
		public GetClientConfigDetailsQueryHandler(IClientConfigRepositoryAsync repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Response<ClientConfigDetailsViewModel>> Handle(GetClientConfigDetailsQuery query, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(query.Id);
			if (config == null) throw new APIException($"Configuration Not Found.");
			return new Response<ClientConfigDetailsViewModel>(_mapper.Map<ClientConfigDetailsViewModel>(config), true);
		}
	}
}
