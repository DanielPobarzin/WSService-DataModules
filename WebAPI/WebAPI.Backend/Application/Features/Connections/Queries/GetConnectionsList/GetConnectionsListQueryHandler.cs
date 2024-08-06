using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Parameters;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Connections.Queries.GetConnectionsList
{
	public class GetConnectionsListQueryHandler : IRequestHandler<GetConnectionsListQuery, ConnectionResponse<IEnumerable<Entity>>>
	{
		private readonly IConnectionRepositoryAsync _connectionsRepository;
		private readonly IModelHelper _modelHelper;
		private readonly IMapper _mapper;
		public GetConnectionsListQueryHandler(IConnectionRepositoryAsync connectionsRepository, IMapper mapper, IModelHelper modelHelper)
		{
			_connectionsRepository = connectionsRepository;
			_mapper = mapper;
			_modelHelper = modelHelper;
		}
		public async Task<ConnectionResponse<IEnumerable<Entity>>> Handle(GetConnectionsListQuery request, CancellationToken cancellationToken)
		{
			var validFilter = request;
			validFilter.Fields = (!string.IsNullOrEmpty(validFilter.Fields))? _modelHelper.ValidateModelFields<ConnectionListViewModel>(validFilter.Fields) 
																			: _modelHelper.GetModelFields<ConnectionListViewModel>();

			var queryResult = await _connectionsRepository.GetStateConnectionsResponseAsync(validFilter);
			var context = queryResult.context;
			ConnectionsCount connectionsCount = queryResult.connectionCount;

			return new ConnectionResponse<IEnumerable<Entity>>(context, connectionsCount);
		}
	}
}