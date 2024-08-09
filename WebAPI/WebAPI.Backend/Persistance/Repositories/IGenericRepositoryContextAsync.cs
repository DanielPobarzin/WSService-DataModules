using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistance.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
	public interface IGenericRepositoryWithContext<T, TContext> : IGenericRepositoryAsync<T>
	where T : class
	where TContext : DbContext
	{
		TContext GetDbContext();
	}
}
