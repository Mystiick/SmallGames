using System;
using System.Collections.Generic;
using System.Text;

using TopDownShooter.Managers;
using TopDownShooter.Models;

namespace TopDownShooter.Services
{
    public static class MessagingService
    {
        /// <summary>
        /// The concrete the MessagingService is built on
        /// </summary>
        public static MessagingManager Instance { get; private set; }

        /// <summary>
        /// Required one-time setup for the static class
        /// </summary>
        public static void Init()
        {
            Instance = new MessagingManager();
        }

        /// <summary>
        /// Subscribe a function to an event type
        /// </summary>
        public static Subscription Subscribe(EventType eventType, Action<object, object> handler, Guid callerID)
        {
            return Instance.Subscribe(eventType, handler, callerID);
        }

        /// <summary>
        /// Subscribe a function to an event type
        /// </summary>
        public static Subscription Subscribe(EventType eventType, string eventName, Action<object, object> handler, Guid callerID)
        {
            return Instance.Subscribe(eventType, eventName, handler, callerID);
        }

        /// <summary>
        /// Remove subscription by ID
        /// </summary>
        /// <returns>True if any events were unsubscribed, false if none were unsubscribed</returns>
        public static bool Unsubscribe(Guid handlerID)
        {
            return Instance.Unsubscribe(handlerID);
        }

        /// <summary>
        /// Remove subscription by ID
        /// </summary>
        /// <returns>True if any events were unsubscribed, false if none were unsubscribed</returns>
        public static bool Unsubscribe(Guid handlerID, bool throwIfNoneFound)
        {
            return Instance.Unsubscribe(handlerID, throwIfNoneFound);
        }

        /// <summary>
        /// Removes all subscriptions for a parent object
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns>True if any events were unsubscribed, false if none were unsubscribed</returns>
        public static bool UnsubscribeParent(Guid parentID)
        {
            return Instance.UnsubscribeParent(parentID);
        }

        public static void SendMessage(EventType eventType, string eventName, object sender, object args)
        {
#if DEBUG
            Console.WriteLine($"Sending message: {eventType} | {eventName} | {args}");
#endif
            Instance.SendMessage(eventType, eventName, sender, args);
        }
    }
}
