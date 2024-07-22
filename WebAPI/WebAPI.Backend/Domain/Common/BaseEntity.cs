using System;

namespace WebAPI.Backend.Core.Domain.Common
{
	public abstract class BaseEntity
    {
        public virtual Guid Id { get; set; }
    }
}