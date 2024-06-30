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
		public async Task<RecievedByClientNotification> TransformToRecievedDataDTO(MessageServerDTO messageServerDTO, Guid clientId)
		{
			RecievedByClientNotification message = new RecievedByClientNotification
			{
				ClientId = clientId,
				ServerId = messageServerDTO.ServerId,
				Notification = messageServerDTO.Notification,
				DateAndTimeSendDataByServer = messageServerDTO.DateAndTimeSendDataByServer,
				DateAndTimeRecievedDataFromServer = DateTime.Now
			};

			return await Task.FromResult(message);
		}
	}
}
