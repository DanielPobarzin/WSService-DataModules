using Application.Features.Configurations.Server.Commands.CreateConfig;
using Application.Features.Configurations.Server.Commands.DeleteConfig;
using Application.Features.Configurations.Server.Commands.SendConfig;
using Application.Features.Configurations.Server.Commands.UpdateConfig;
using Application.Features.Configurations.Server.Queries.GetConfigServerDetails;
using Application.Features.Configurations.Server.Queries.GetConfigServerList;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Config;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
	[Route("api/v1/server/configure/[controller]")]
	public class ServerConfigController : BaseController
	{
		private readonly IMapper _mapper;
		public ServerConfigController(IMapper mapper) => _mapper = mapper;
		[HttpGet]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ConfigServerListViewModel>> GetAll()
		{
			var query = new GetConfigServerListQuery();
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}

		[HttpGet("{id}")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ServerConfigDetailsViewModel>> Get([FromRoute] Guid id)
		{
			var query = new GetServerConfigDetailsQuery
			{
				Id = id
			};
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}
		[HttpPost]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<Guid>> Create([FromBody] CreateServerConfigDTO createConfigDto)
		{
			var command = _mapper.Map<CreateConfigServerCommand>(createConfigDto);
			var configId = await Mediator.Send(command);
			return Ok(configId);
		}
		[HttpPut]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Update([FromBody] UpdateServerConfigDTO updateConfigDto)
		{
			var command = _mapper.Map<UpdateConfigServerCommand>(updateConfigDto);
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpDelete("{id}")]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			var command = new DeleteConfigServerCommand
			{
				Id = id,
			};
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpPut("{id}")]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Send([FromRoute] Guid id)
		{
			var command = new SendConfigServerCommand
			{
				Id = id,
			};
			await Mediator.Send(command);
			return NoContent();
		}
	}
}
