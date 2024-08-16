using Application.Features.Clients.Commands.AddClient;
using Application.Features.Clients.Commands.DeleteClient;
using Application.Features.Clients.Commands.UpdateClient;
using Application.Features.Clients.Queries.GetClient.GetAll;
using Application.Features.CLients.Queries.GetClient.GetDetails;
using Application.Features.Connections.Commands.AddConnection;
using Application.Features.Connections.Commands.CloseConnection;
using Application.Features.Connections.Commands.DeleteConnection;
using Application.Features.Connections.Commands.OpenConnection;
using Application.Features.Connections.Commands.UpdateConnection;
using Application.Features.Connections.Queries.GetConnectionDetails;
using Application.Features.Connections.Queries.GetConnectionsList;
using Application.Features.Servers.Commands.DeleteServer;
using Application.Features.Servers.Queries.GetServer.GetAll;
using Application.Features.Servers.Queries.GetServer.GetDetails;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Client;
using Shared.Models.Connection;
using Shared.Models.Server;

namespace WebAPI.Controllers
{
	[Produces("application/json")]
	[Route("api/v1/connections/[controller]")]
	public class ConnectionController : BaseController
	{
		private readonly IMapper _mapper;
		public ConnectionController(IMapper mapper) => _mapper = mapper;

		[HttpGet("allConnections")]
		//[Authorize(Policy = Authorization.UserPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ConnectionListViewModel>> GetAllConnections()
		{
			var query = new GetConnectionsListQuery();
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}
		[HttpGet("{connectionId}")]
		//[Authorize(Policy = Authorization.UserPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ConnectionDetailsViewModel>> GetConnection([FromRoute] string connectionId)
		{
			var query = new GetConnectionDetalisQuery
			{
				ConnectionId = connectionId
			};
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}
		[HttpGet("allServers")]
		//[Authorize(Policy = Authorization.UserPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ServerListViewModel>> GetAllServers()
		{
			var query = new GetServerListQuery();
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}
		[HttpGet("server::{serverId}")]
		//[Authorize(Policy = Authorization.UserPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ServerDetailsViewModel>> GetServer([FromRoute] Guid serverId)
		{
			var query = new ServerDetailsQuery
			{
				Id = serverId
			};
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}
		[HttpGet("allClients")]
		//[Authorize(Policy = Authorization.UserPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ClientListViewModel>> GetAllClients()
		{
			var query = new GetClientListQuery();
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}
		[HttpGet("client::{clientId}")]
		//[Authorize(Policy = Authorization.UserPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ClientDetailsViewModel>> Get([FromRoute] Guid clientId)
		{
			var query = new ClientDetailsQuery
			{
				Id = clientId
			};
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}
		[HttpPost("connection/add")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<Guid>> Create([FromBody] AddConnectionDTO addConnectionDto)
		{
			var command = _mapper.Map<AddConnectionCommand>(addConnectionDto);
			var serverId = await Mediator.Send(command);
			return Ok(serverId);
		}
		[HttpPost("server/add")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<Guid>> Create([FromBody] AddServerDTO addServerDto)
		{
			var command = _mapper.Map<Application.Features.Servers.Commands.AddServer.AddServerCommand>(addServerDto);
			var serverId = await Mediator.Send(command);
			return Ok(serverId);
		}
		[HttpPost("client/add")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<Guid>> Create([FromBody] AddClientDTO addClientDto)
		{
			var command = _mapper.Map<AddClientCommand>(addClientDto);
			var clientId = await Mediator.Send(command);
			return Ok(clientId);
		}
		[HttpPut("connection/update")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Update([FromBody] UpdateConnectionDTO updateConnectionDto)
		{
			var command = _mapper.Map<UpdateConnectionCommand>(updateConnectionDto);
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpPut("server/update")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Update([FromBody] UpdateServerDTO updateServerDto)
		{
			var command = _mapper.Map<Application.Features.Servers.Commands.UpdateServer.UpdateServerCommand>(updateServerDto);
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpPut("client/update")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Update([FromBody] UpdateClientDTO updateClientDto)
		{
			var command = _mapper.Map<UpdateClientCommand>(updateClientDto);
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpDelete("{connectionId}")]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Delete([FromRoute] string connectionId)
		{
			var command = new DeleteConnectionCommand
			{
				ConnectionId = connectionId,
			};
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpDelete("server::{Id}")]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> DeleteServer([FromRoute] Guid id)
		{
			var command = new DeleteServerCommand
			{
				ServerId = id,
			};
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpDelete("client::{Id}")]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> DeleteClient([FromRoute] Guid id)
		{
			var command = new DeleteClientCommand
			{
				ClientId = id,
			};
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpPut("connection/close/client::{id}")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Close([FromRoute] Guid id)
		{
			var command = new CloseConnectionCommand
			{
				Id = id,
			};
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpPut("connection/open/client::{id}")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Open([FromRoute] Guid id)
		{
			var command = new OpenConnectionCommand
			{
				Id = id,
			};
			await Mediator.Send(command);
			return NoContent();
		}
	}
	
}
