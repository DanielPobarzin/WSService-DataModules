﻿namespace Application.DTOs.Message
{
	public class MessageRequest
	{
		public Guid To { get; set; }
		public string Body { get; set; }
	}
}
