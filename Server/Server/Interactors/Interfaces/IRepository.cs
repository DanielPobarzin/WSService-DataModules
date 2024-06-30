using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactors.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
	{
		IEnumerable<T> GetAllList();
	}
}
