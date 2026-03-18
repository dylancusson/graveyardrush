using System;
using System.Collections.Generic;

public static class Messenger {
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    public static void AddListener(string eventType, Action handler) {
        if (!eventTable.ContainsKey(eventType)) eventTable.Add(eventType, null);
        eventTable[eventType] = (Action)eventTable[eventType] + handler;
    }

    public static void RemoveListener(string eventType, Action handler) {
        if (eventTable.ContainsKey(eventType)) {
            eventTable[eventType] = (Action)eventTable[eventType] - handler;
            if (eventTable[eventType] == null) eventTable.Remove(eventType);
        }
    }

    public static void Broadcast(string eventType) {
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d)) {
            Action callback = d as Action;
            if (callback != null) callback();
        }
    }
}

// Overload for events that pass a parameter (like Speed)
public static class Messenger<T> {
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    public static void AddListener(string eventType, Action<T> handler) {
        if (!eventTable.ContainsKey(eventType)) eventTable.Add(eventType, null);
        eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;
    }

    public static void RemoveListener(string eventType, Action<T> handler) {
        if (eventTable.ContainsKey(eventType)) {
            eventTable[eventType] = (Action<T>)eventTable[eventType] - handler;
            if (eventTable[eventType] == null) eventTable.Remove(eventType);
        }
    }

    public static void Broadcast(string eventType, T arg1) {
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d)) {
            Action<T> callback = d as Action<T>;
            if (callback != null) callback(arg1);
        }
    }
}