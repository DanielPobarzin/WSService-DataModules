using Application.Features.Connections.Commands.UpdateConnection;
using Application.Mappings;
using AutoMapper;
using Domain.Enums;

namespace WebAPI.Models.Connection
{
	public class UpdateConnectionDTO : IMapWith<UpdateConnectionCommand>
	{
		public string ConnectionId { get; set; }
		public TimeSpan? Session { get; set; }
		public DateTime? TimeStampCloseConnection { get; set; }
		public ConnectionStatus Status { get; set; }
		public void Mapping(Profile profile)
		{

			profile.CreateMap<UpdateConnectionDTO, UpdateConnectionCommand>()
					.ForMember(connectionVm => connectionVm.ConnectionId,
							   opt => opt.MapFrom(connection => connection.ConnectionId))
					.ForMember(connectionVm => connectionVm.TimeStampCloseConnection,
							   opt => opt.MapFrom(connection => connection.TimeStampCloseConnection))
					.ForMember(connectionVm => connectionVm.Status,
							   opt => opt.MapFrom(connection => connection.Status))
					.ForMember(connectionVm => connectionVm.Session,
							   opt => opt.MapFrom(connection => connection.Session));
		}
	}
}
