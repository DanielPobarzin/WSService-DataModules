using Interactors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
	public class GenericRepository<T, TContext> : IRepository<T>
	where T : class
	where TContext : DbContext
	{
		private readonly TContext _dbContext;
		public GenericRepository(TContext dbContext)
		{
			_dbContext = dbContext;
		}
		public virtual async Task<T> FetchByIdMessage(Guid id)
		{
			return await _dbContext.Set<T>().FindAsync(id);
		}
		public async Task<T> PublishMessage(T entity)
		{
			await _dbContext.Set<T>().AddAsync(entity);
			await _dbContext.SaveChangesAsync();
			return entity;
		}

	}
}
