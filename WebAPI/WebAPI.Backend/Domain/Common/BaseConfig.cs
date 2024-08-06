using Domain.Entities;

namespace Domain.Common
{
	public abstract class BaseConfig : Entity
    {
        public virtual Guid SystemId { get; set; }
    }
}
