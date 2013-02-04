using System;
using System.Collections.Generic;

namespace Gather.Core.Infrastructure
{
    /// <summary>
    /// A statically compiled singleton used to store objects throughout the lifetime of the application.
    /// </summary>
    /// <typeparam name="T">The type of object to store</typeparam>
    public class Singleton<T> : Singleton
    {
        static T _instance;

        public static T Instance
        {
            get { return _instance; }
            set
            {
                _instance = value;
                AllSingletons[typeof(T)] = value;
            }
        }
    }

    /// <summary>
    /// Provides a singleton list for a certain type
    /// </summary>
    /// <typeparam name="T">The type of list to store</typeparam>
    public class SingletonList<T> : Singleton<IList<T>>
    {
        static SingletonList()
        {
            Singleton<IList<T>>.Instance = new List<T>();
        }

        public new static IList<T> Instance
        {
            get { return Singleton<IList<T>>.Instance; }
        }
    }

    /// <summary>
    /// Provides a singleton dictionary for a certain key and vlaue type
    /// </summary>
    /// <typeparam name="TKey">The type of key</typeparam>
    /// <typeparam name="TValue">The type of value</typeparam>
    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        public new static IDictionary<TKey, TValue> Instance
        {
            get { return Singleton<Dictionary<TKey, TValue>>.Instance; }
        }
    }

    /// <summary>
    /// Provides access to all "singletons" stored by <see cref="Singleton{T}"/>.
    /// </summary>
    public class Singleton
    {
        static readonly IDictionary<Type, object> _allSingletons;

        static Singleton()
        {
            _allSingletons = new Dictionary<Type, object>();
        }

        public static IDictionary<Type, object> AllSingletons
        {
            get { return _allSingletons; }
        }
    }
}