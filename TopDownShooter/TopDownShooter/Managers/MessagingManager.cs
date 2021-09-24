using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDownShooter.Models;

namespace TopDownShooter.Managers
{
    public class MessagingManager
    {

        private readonly List<Subscription> _subscriptions;

        public MessagingManager()
        {
            _subscriptions = new List<Subscription>();
        }

        #region | Properties |
        public int TotalSubscriptions { get { return _subscriptions.Count; } }
        #endregion

        #region | Public Functions |
        /// <summary>
        /// Subscribe a function to an event type
        /// </summary>
        /// <param name="eventType">Event type to subscribe to</param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public Subscription Subscribe(EventType eventType, Action<object, object> handler, Guid callerID)
        {
            Console.WriteLine($"Subscribing to {eventType} | null | {callerID}");

            var output = new Subscription(Guid.NewGuid(), eventType, handler, callerID);
            _subscriptions.Add(output);

            return output;
        }

        public Subscription Subscribe(EventType eventType, string eventName, Action<object, object> handler, Guid callerID)
        {
            Console.WriteLine($"Subscribing to {eventType} | {eventName} | {callerID}");

            var output = new Subscription(Guid.NewGuid(), eventType, eventName, handler, callerID);
            _subscriptions.Add(output);

            return output;
        }

        /// <summary>
        /// Remove subscription by ID
        /// </summary>
        /// <returns>True if any events were unsubscribed, false if none were unsubscribed</returns>
        public bool Unsubscribe(Guid handlerID)
        {
            return _subscriptions.RemoveAll(x => x.ID == handlerID) > 0;
        }

        /// <summary>
        /// Remove subscription by ID
        /// </summary>
        /// <returns>True if any events were unsubscribed, false if none were unsubscribed</returns>
        public bool Unsubscribe(Guid handlerID, bool throwIfNoneFound)
        {
            if (throwIfNoneFound && !_subscriptions.Any(x => x.ID == handlerID))
            {
                throw new ArgumentException($"ID {handlerID.ToString()} has no subscriptions.");
            }

            return this.Unsubscribe(handlerID);
        }

        /// <summary>
        /// Removes all subscriptions for a parent object
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns>True if any events were unsubscribed, false if none were unsubscribed</returns>
        public bool UnsubscribeParent(Guid parentID)
        {
            return _subscriptions.RemoveAll(x => x.ParentID == parentID) > 0;
        }

        public void SendMessage(EventType eventType, string eventName, object sender, object args)
        {
            Subscription[] subscriptions = _subscriptions.Where(x => x.Event == eventType && (x.EventName == eventName || x.EventName is null)).ToArray();

            foreach (var sub in subscriptions)
            {
                sub.Handler.Invoke(sender, args);
            }
        }
        #endregion
    }
}
