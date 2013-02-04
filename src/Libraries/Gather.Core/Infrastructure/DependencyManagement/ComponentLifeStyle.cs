namespace Gather.Core.Infrastructure.DependencyManagement
{
    public enum ComponentLifeStyle
    {
        /// <summary>
        /// Singleton
        /// </summary>
        Singleton = 0,
        /// <summary>
        /// Transient
        /// </summary>
        Transient = 1,
        /// <summary>
        /// Lifetime scope
        /// </summary>
        LifetimeScope = 2
    }
}