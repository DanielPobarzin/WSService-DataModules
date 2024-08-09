using Application.Features.Configurations.Client.Commands.CreateConfig;
using Application.Features.Configurations.Client.Commands.DeleteConfig;
using Application.Features.Configurations.Client.Commands.SendConfig;
using Application.Features.Configurations.Client.Commands.UpdateConfig;
using Application.Features.Configurations.Client.Queries.GetConfigClientDetails;
using Application.Features.Configurations.Client.Queries.GetConfigClientList;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Extensions;
using WebAPI.Models.Config;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
	[Route("api/v1/client/configure/[controller]")]
	public class ClientConfigController : BaseController
	{
		private readonly IMapper _mapper;
		public ClientConfigController(IMapper mapper) => _mapper = mapper;

		[HttpGet]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ConfigClientListViewModel>> GetAll()
		{
			var query = new GetConfigClientListQuery();
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}

		[HttpGet("{id}")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ClientConfigDetailsViewModel>> Get([FromRoute] Guid id)
		{
			var query = new GetClientConfigDetailsQuery
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
		public async Task<ActionResult<Guid>> Create([FromBody] CreateClientConfigDTO createConfigDto)
		{
			var command = _mapper.Map<CreateConfigClientCommand>(createConfigDto);
			var configId = await Mediator.Send(command);
			return Ok(configId);
		}
		[HttpPut]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Update([FromBody] UpdateClientConfigDTO updateConfigDto)
		{
			var command = _mapper.Map<UpdateConfigClientCommand>(updateConfigDto);
			await Mediator.Send(command);
			return NoContent();
		}
		[HttpDelete("{id}")]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			var command = new DeleteConfigClientCommand
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
			var command = new SendConfigClientCommand
			{
				Id = id,
			};
			await Mediator.Send(command);
			return NoContent();
		}
	}
}   

