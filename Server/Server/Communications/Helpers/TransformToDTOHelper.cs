using Communications.DTO;
using Entities.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communications.Helpers
{
	public class TransformToDTOHelper
	{
		public async Task<MessageServerDTO> TransformToNotificationDTO(Notification notification, Guid serverId)
		{
			MessageServerDTO messageServerDTOs = new MessageServerDTO
			{
				ServerId = serverId,
				Notification = notification,
				DateAndTimeSendDataByServer = DateTime.Now
			};

			return await Task.FromResult(messageServerDTOs);
		}
	}
}
