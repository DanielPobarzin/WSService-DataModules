using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class BaseConfig
    {
        public virtual Guid SystemId { get; set; }
    }
}
