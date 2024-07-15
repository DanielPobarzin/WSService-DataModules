using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Connections;

public class Initializer
{
	public static void Initialize(ConnectionDbContext context)
	{
		context.Database.EnsureCreated();
	}
}
