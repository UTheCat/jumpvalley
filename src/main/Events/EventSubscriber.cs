using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumpvalley.Events
{
    /// <summary>
    /// Utility class for safely connecting and disconnecting functions from events.
    /// The implementation of this class is currently not finished.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arguments</typeparam>
    public partial class EventSubscriber<TEventArgs>
    {
        public event EventHandler<TEventArgs> Event;

        public EventSubscriber(EventHandler<TEventArgs> @event, Callback callback)
        {
            Event = @event;
        }

        /// <summary>
        /// The callback to connect the event to.
        /// </summary>
        /// <param name="o">The object that the event call comes from</param>
        /// <param name="e">The event data</param>
        public delegate void Callback(object o, TEventArgs e);
    }
}
