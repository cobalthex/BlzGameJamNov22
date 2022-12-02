/// <summary>
/// Basic Event Manager
/// 
/// Usage:
///
///     EventManager.AddListener<SomeEvent>(OnSomeEvent);
///
///     EventManager.Fire(new SomeEvent(Data));
///
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TEventBase
{
}

public delegate void EventHandlerDelegate<TEvent>(TEvent evt) where TEvent : TEventBase;

static internal class EventManager
{
    static public Dictionary<Type, Delegate> m_eventTable = new Dictionary<Type, Delegate>();

    public class FireEventException : Exception
    {
        public FireEventException(string msg)
            : base(msg)
        {
        }
    }

    static public void AddListener<TEvent>(EventHandlerDelegate<TEvent> handler) where TEvent : TEventBase
    {
        var eventType = typeof(TEvent);
        if (!m_eventTable.ContainsKey(eventType))
        {
            m_eventTable.Add(eventType, null);
        }

        Delegate theDelegate = m_eventTable[eventType];
        if (m_eventTable.TryGetValue(typeof(TEvent), out theDelegate))
            m_eventTable[eventType] = Delegate.Combine(theDelegate, handler);
        else
            m_eventTable[eventType] = handler;
    }

    static public void RemoveListener<TEvent>(EventHandlerDelegate<TEvent> handler) where TEvent : TEventBase
    {
        var eventType = typeof(TEvent);

        Delegate theDelegate;
        if (m_eventTable.TryGetValue(eventType, out theDelegate))
        {
            Delegate currentDelegate = Delegate.Remove(theDelegate, handler);
            if (currentDelegate == null)
                m_eventTable.Remove(typeof(TEvent));
            else
                m_eventTable[eventType] = currentDelegate;
        }
    }

    static public void Fire<TEvent>(TEvent evt) where TEvent : TEventBase
    {
        if (evt == null)
            return;

        var eventType = typeof(TEvent);
        if (!m_eventTable.ContainsKey(eventType))
        {
            Debug.LogWarning($"No event for {eventType.ToString()}");
            return;
        }

        Delegate theDelegate;
        if (m_eventTable.TryGetValue(eventType, out theDelegate))
        {
            EventHandlerDelegate<TEvent> callback = theDelegate as EventHandlerDelegate<TEvent>;
            if (callback == null)
                throw new FireEventException($"Fire message \"{eventType.ToString()}\" failed. Assigned delegate is null.");
            else
                callback(evt);
        }
        else
        {
            throw new FireEventException($"Fire message \"{eventType.ToString()}\" failed. Could not find associated delegate.");
        }
    }
}