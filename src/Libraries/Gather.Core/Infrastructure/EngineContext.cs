using System.Runtime.CompilerServices;

namespace Gather.Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the WeWillGather engine.
    /// </summary>
    public class EngineContext
    {

        #region Init Methods

        /// <summary>
        /// Initializes a static instance of the engine
        /// </summary>
        /// <param name="forceRecreate">Creates a new engine instance even if one already exists</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                Singleton<IEngine>.Instance = new Engine();
            }
            return Singleton<IEngine>.Instance;
        }

        #endregion

        /// <summary>Gets the singleton Nop engine used to access Nop services.</summary>
        public static IEngine Current
        {
            get { return Singleton<IEngine>.Instance ?? Initialize(false); }
        }

    }
}