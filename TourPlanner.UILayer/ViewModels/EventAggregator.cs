using System;
using System.Collections.Generic;

namespace TourPlanner.UILayer.ViewModels
{
    public class EventAggregator
    {
        private static readonly Lazy<EventAggregator> _instance = new Lazy<EventAggregator>(() => new EventAggregator());
        private readonly Dictionary<Type, List<Action<object>>> _handlers = new Dictionary<Type, List<Action<object>>>();

        public static EventAggregator Instance => _instance.Value;

        private EventAggregator() { }

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
            {
                _handlers[type] = new List<Action<object>>();
            }

            _handlers[type].Add(obj => handler((T)obj));
        }

        public void Publish<T>(T message)
        {
            var type = typeof(T);
            if (_handlers.ContainsKey(type))
            {
                foreach (var handler in _handlers[type])
                {
                    handler(message);
                }
            }
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_handlers.ContainsKey(type))
            {
                _handlers[type].Remove(obj => handler((T)obj));
            }
        }
    }
} 