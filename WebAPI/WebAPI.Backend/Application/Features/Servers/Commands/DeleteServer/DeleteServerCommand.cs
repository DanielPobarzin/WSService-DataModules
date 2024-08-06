using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Servers.Commands.DeleteServer
{
	public class DeleteServerCommand : IRequest<Response<Guid>>
	{
		public Guid ServerId { get; set; }
	}
}
