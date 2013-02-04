namespace Gather.Core
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the user active flag
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Gets or sets the user deleted flag
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}