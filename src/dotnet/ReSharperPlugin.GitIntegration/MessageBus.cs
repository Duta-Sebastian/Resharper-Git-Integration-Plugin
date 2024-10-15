using System;
using System.Collections.Generic;

namespace ReSharperPlugin.GitIntegration;

public static class MessageBus
{
    private static readonly Dictionary<Type, List<Action<object>>> Subscribers = new();

    public static void Publish<T>(T message)
    {
        if (!Subscribers.TryGetValue(typeof(T), out var actions)) return;
        foreach (var action in actions) action(message);
    }

    public static void Subscribe<T>(Action<T> action)
    {
        if (!Subscribers.ContainsKey(typeof(T))) Subscribers[typeof(T)] = [];

        Subscribers[typeof(T)].Add(msg => action((T)msg));
    }
}