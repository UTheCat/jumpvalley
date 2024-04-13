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
    /// <br/>
    /// It helps to prevent duplicate event connections, and it checks if the event connection is active before disconnecting.
    /// <br/>
    /// The implementation of this class is currently unfinished.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arguments</typeparam>
    public partial class EventSubscriber<TEventArgs>
    {
        /// <summary>
        /// The event that the event subscriber is connected to.
        /// </summary>
        public event EventHandler<TEventArgs> Event;

        /// <summary>
        /// The callback that will get connected to the event.
        /// </summary>
        /// <param name="o">The object that the event call comes from</param>
        /// <param name="e">The event data</param>
        public EventHandler<TEventArgs> Callback;

        private bool _isConnected = false;

        /// <summary>
        /// Whether or not <see cref="Callback"/> is connected to <see cref="Event"/>.
        /// <br/>
        /// Setting this to true will connect the currently specified callback to the currently specified event.
        /// <br/>
        /// Setting this to false will disconnect the currently specified callback from the currently specified event.
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (value == _isConnected) return;

                if (value)
                {
                    Event += Callback;
                }
            }
        }

        public EventSubscriber(EventHandler<TEventArgs> callback)
        {
            Callback = callback;
        }
    }
}
