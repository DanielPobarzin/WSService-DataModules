﻿using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.DO;
using Repositories.Notifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communications.UoW
{
	public class UnitOfWorkPublishNotifications : IDisposable
	{
		private RecievedNotificationsDbContext db;
		private RecievedNotificationsRepository notificationRepository;
		private IConfiguration configuration;
		private bool disposed = false;
		public RecievedNotificationsRepository Notifications
		{
			get
			{
				notificationRepository ??= new RecievedNotificationsRepository(db);
				return notificationRepository;
			}
		}
		public UnitOfWorkPublishNotifications(IConfiguration configuration)
		{
			this.configuration = configuration;
			db = new RecievedNotificationsDbContext(this.configuration);
		}
		public async Task PublishNotifications(DomainObjectNotification message)
		{
			await Notifications.PublishMessage(message);
		}
		public void Save()
		{
			db.SaveChanges();
		}
		public virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
					db.Dispose();
				this.disposed = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}


