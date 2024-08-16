using Application.Features.Connections.Queries.GetConnectionsList;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clients.Queries.GetClient.GetAll
{
	/// <summary>
	/// Data Transfer Object (DTO) for looking up client information.
	/// </summary>
	public class ClientLookupDTO
	{
		public Guid ClientId { get; set; }

		public void Mapping(Profile profile)
		{
			profile.CreateMap<Client, ClientLookupDTO>()
				.ForMember(clientDto => clientDto.ClientId,
					opt => opt.MapFrom(client => client.Id));
		}
	}
}
