using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.UI.Events
{
    //  Helper folder / system comp folder
    public class EventAggregator
    {
        //  Note: you can add a lock for thread safety

        List<(Type eventType, Delegate methodToCall)> eventRegistrations =
            new List<(Type eventType, Delegate methodToCall)>();

        public void Subscribe<T>(Action<T> action)
        {
            Console.WriteLine($"Subscribing to event of type {typeof(T)}");
            if (action != null)
            {
                this.eventRegistrations.Add((typeof(T),action));
            }
        }
        
        public void Publish<T>(T data)
        {
            Console.WriteLine($"Publishing event of type {typeof(T)}");
            List<(Type type, Delegate methodToCall)> regs = 
                new List<(Type type, Delegate methodToCall)>(eventRegistrations);
            
            foreach(var item in regs)
            {
                if(item.type == typeof(T))
                {
                    ((Action<T>)item.methodToCall)(data);
                }
            }
        }
    }
}
