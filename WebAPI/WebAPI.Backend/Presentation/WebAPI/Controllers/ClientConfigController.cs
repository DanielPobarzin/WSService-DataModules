using Application.Features.Configurations.Client.Commands.CreateConfig;
using Application.Features.Configurations.Client.Commands.DeleteConfig;
using Application.Features.Configurations.Client.Commands.SendConfig;
using Application.Features.Configurations.Client.Commands.UpdateConfig;
using Application.Features.Configurations.Client.Queries.GetConfigClientDetails;
using Application.Features.Configurations.Client.Queries.GetConfigClientList;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Models.Config;

namespace WebAPI.Controllers
{
	[Produces("application/json")]
	[Route("api/v1/client/configure/[controller]")]
	public class ClientConfigController : BaseController
	{
		private readonly IMapper _mapper;
		public ClientConfigController(IMapper mapper) => _mapper = mapper;

		/// <summary>
		/// Gets the list of available client configurations.
		/// </summary>
		/// <returns>Returns ConfigClientListViewModel <see cref="ConfigClientListViewModel"/></returns>
		/// <response code="200">Success</response>
		/// <response code="401">If the user is  unauthorized</response>
		[HttpGet]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[SwaggerResponse(200, "Returns the IDs of all available configurations", typeof(Guid))]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<ConfigClientListViewModel>> GetAll()
		{
			var query = new GetConfigClientListQuery();
			var vm = await Mediator.Send(query);
			return Ok(vm);
		}

		/// <summary>
		/// Gets the details of available client configuration by id.
		/// </summary>
		/// <returns>Returns ClientConfigDetailsViewModel <see cref="ClientConfigDetailsViewModel"/></returns>
		/// <response code="200">Success</response>
		/// <response code="401">If the user is  unauthorized</response>
		[HttpGet("{id}")]
		//[Authorize(Policy = Authorization.AdminPolicy)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[SwaggerResponse(200, "Returns the parameters of the found configuration", typeof(ClientConfigDetailsViewModel))]
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

		/// <summary>
		/// Creates the client configuration
		/// </summary>
		/// <param name="createConfigDto">CreateClientConfigDTO object. See <see cref="CreateClientConfigDTO"/> for details.</param>
		/// <returns>Returns id (guid)</returns>
		/// <response code="201">Success</response>
		/// <response code="401">If the user unauthorized</response>
		/// <returns></returns>
		[HttpPost]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[SwaggerResponse(201, "Returns the Id of the created configuration", typeof(Guid))]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<Guid>> Create([FromBody] CreateClientConfigDTO createConfigDto)
		{
			var command = _mapper.Map<CreateConfigClientCommand>(createConfigDto);
			var configId = await Mediator.Send(command);
			return Ok(configId);
		}

		/// <summary>
		/// Updates the client configuration
		/// </summary>
		/// <param name="updateConfigDto"> UpdateClientConfigDTO object. See <see cref="UpdateClientConfigDTO"/> for details.</param>
		/// <returns>Returns NoContent</returns>
		/// <response code="204">Success</response>
		/// <response code="401">If the user is unauthorized</response>
		[HttpPut]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[SwaggerResponse(204, "returns if successfully updated")]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Update([FromBody] UpdateClientConfigDTO updateConfigDto)
		{
			try
			{
				var command = _mapper.Map<UpdateConfigClientCommand>(updateConfigDto);
				await Mediator.Send(command);
			}
			catch (AutoMapperMappingException ex)
			{
				
				Console.WriteLine(ex.Message);
			}
		
			
			return NoContent();
		}

		/// <summary>
		/// Deletes the client configuration by id
		/// </summary>
		/// <param name="id"> Id of the client configuration (guid) </param>
		/// <returns>Returns NoContent</returns>
		/// <response code="204">Success</response>
		/// <response code="401">If the user is unauthorized</response>
		[HttpDelete("{id}")]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[SwaggerResponse(204, "returns if successfully deleted")]
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

		/// <summary>
		/// Sends the configuration to the client by id
		/// </summary>
		/// <param name="id"> Id of the client configuration (guid) </param>
		/// <returns>Returns NoContent</returns>
		/// <response code="204">Success</response>
		/// <response code="401">If the user is unauthorized</response>
		[HttpPut("{id}")]
		//[Authorize(Policy = Authorization.SuperAdminPolicy)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[SwaggerResponse(204, "returns if successfully sent")]
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

