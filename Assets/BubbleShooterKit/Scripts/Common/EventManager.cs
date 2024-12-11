// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;

namespace BubbleShooterKit
{
    /// <summary>
    /// Basic event manager class. We mainly use it to notify several parts of the
    /// code base about a gameplay event happening (e.g., bubbles exploding).
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<Type, List<IBaseEventListener>> RegisteredListeners =
            new Dictionary<Type, List<IBaseEventListener>>();

        public static void RegisterListener<T>(IEventListener<T> listener) where T : struct
        {
            var type = typeof(T);
            
            if (!RegisteredListeners.ContainsKey(type))
                RegisteredListeners.Add(type, new List<IBaseEventListener>());
            
            RegisteredListeners[type].Add(listener);
        }

        public static void UnregisterListener<T>(IEventListener<T> listener) where T : struct
        {
            var type = typeof(T);

            RegisteredListeners[type].Remove(listener);
        }

        public static void RaiseEvent<T>(T evt)
        {
            var type = typeof(T);
            
            foreach (var listener in RegisteredListeners[type])
                ((IEventListener<T>)listener).HandleEvent(evt);
        }
    }
    
    public interface IBaseEventListener
    {
    }
    
    public interface IEventListener<in T> : IBaseEventListener
    {
        void HandleEvent(T evt);
    }
}
