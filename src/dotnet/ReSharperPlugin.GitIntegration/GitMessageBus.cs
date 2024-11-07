using System;
using System.Collections.Immutable;
using System.Threading;

namespace ReSharperPlugin.GitIntegration;

public static class GitMessageBus
{
    private static volatile ImmutableDictionary<Type, ImmutableList<Action<object>>> _subscribers = ImmutableDictionary<Type, ImmutableList<Action<object>>>.Empty;

    public static void Publish<T>(T message)
    {
        if (!_subscribers.TryGetValue(typeof(T), out var actions)) return;
        
        foreach (var action in actions)
        {
            action(message);
        }
    }

    public static void Subscribe<T>(Action<T> action)
    {
        ImmutableDictionary<Type, ImmutableList<Action<object>>> originalDict, updatedDict;

        do
        {
            originalDict = _subscribers;
            
            var currentList = originalDict.GetValueOrDefault(typeof(T), ImmutableList<Action<object>>.Empty);
            
            var updatedList = currentList.Add(msg => action((T)msg));
            
            updatedDict = originalDict.SetItem(typeof(T), updatedList);
        }
        while (Interlocked.CompareExchange(ref _subscribers, updatedDict, originalDict) != originalDict);
    }
}