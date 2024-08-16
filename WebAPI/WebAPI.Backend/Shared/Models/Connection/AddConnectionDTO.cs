using Application.Features.Connections.Commands.AddConnection;
using Application.Features.Connections.Queries.GetConnectionDetails;
using Application.Mappings;
using AutoMapper;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Connection
{
    public class AddConnectionDTO : IMapWith<AddConnectionCommand>
	{
		[Required]
		public Guid ClientId { get; set; }
		[Required]
		public Guid ServerId { get; set; }
		[Required]
		public string ConnectionId { get; set; }
		[Required]
		public DateTime TimeStampOpenConnection { get; set; }
		[Required]
		public ConnectionStatus Status { get; set; }
		public void Mapping(Profile profile)
		{

			profile.CreateMap<AddConnectionDTO, AddConnectionCommand>()
					.ForMember(connectionVm => connectionVm.ClientId,
							   opt => opt.MapFrom(connection => connection.ClientId))
					.ForMember(connectionVm => connectionVm.ServerId,
							   opt => opt.MapFrom(connection => connection.ServerId))
					.ForMember(connectionVm => connectionVm.ConnectionId,
							   opt => opt.MapFrom(connection => connection.ConnectionId))
					.ForMember(connectionVm => connectionVm.TimeStampOpenConnection,
							   opt => opt.MapFrom(connection => connection.TimeStampOpenConnection))
					.ForMember(connectionVm => connectionVm.Status,
							   opt => opt.MapFrom(connection => connection.Status));
		}
	}
}