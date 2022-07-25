namespace MystiickCore.Models;

public class Subscription
{
    public Guid ID { get; }
    public EventType Event { get; }
    public Action<object, object> Handler { get; }
    /// <summary>Optional name of the event that can be used to control when the event is called</summary>
    public string EventName { get; }
    public Guid ParentID { get; }

    public Subscription(Guid id, EventType eventType, Action<object, object> handler, Guid parentID)
    {
        this.ID = id;
        this.Event = eventType;
        this.Handler = handler;
        this.ParentID = parentID;
    }

    public Subscription(Guid id, EventType eventType, string eventName, Action<object, object> handler, Guid parentID)
    {
        this.ID = id;
        this.Event = eventType;
        this.Handler = handler;
        this.EventName = eventName;
        this.ParentID = parentID;
    }
}
