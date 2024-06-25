using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
