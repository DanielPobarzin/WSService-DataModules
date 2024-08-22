namespace Entities.Entities
{
	public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
