using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
	public static class DbInitializer<TContext> where TContext : DbContext
	{
		public static void Initialize(TContext context)
		{
				context.Database.EnsureCreated();
		}
	}
}
