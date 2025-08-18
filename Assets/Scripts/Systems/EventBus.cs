using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EventBus
{
    static readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        if(!_handlers.ContainsKey(typeof(T)))
            _handlers[typeof(T)] = new List<Delegate>();

        _handlers[typeof(T)].Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        if(_handlers.TryGetValue(typeof(T), out List<Delegate> list))
            _handlers[typeof(T)].Remove(handler);
    }

    public static void Publish<T>(T eventData)
    {
        if (_handlers.TryGetValue(typeof(T),out var handlers))
        {
            foreach(var handler in handlers)
            {
                ((Action<T>)handler)?.Invoke(eventData);
            }
        }
    }
}

