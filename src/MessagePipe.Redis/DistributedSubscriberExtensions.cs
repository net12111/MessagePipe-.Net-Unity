﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePipe
{
    public static class DistributedSubscriberExtensions
    {
        // sync handler

        public static ValueTask<IAsyncDisposable> SubscribeAsync<TKey, TMessage>(this IDistributedSubscriber<TKey, TMessage> subscriber, TKey key, Action<TMessage> handler, CancellationToken cancellationToken = default)
        {
            return SubscribeAsync(subscriber, key, handler, Array.Empty<MessageHandlerFilter<TMessage>>(), cancellationToken);
        }

        public static ValueTask<IAsyncDisposable> SubscribeAsync<TKey, TMessage>(this IDistributedSubscriber<TKey, TMessage> subscriber, TKey key, Action<TMessage> handler, MessageHandlerFilter<TMessage>[] filters, CancellationToken cancellationToken = default)
        {
            return subscriber.SubscribeAsync(key, new AnonymousMessageHandler<TMessage>(handler), filters, cancellationToken);
        }

        public static ValueTask<IAsyncDisposable> SubscribeAsync<TKey, TMessage>(this IDistributedSubscriber<TKey, TMessage> subscriber, TKey key, Action<TMessage> handler, Func<TMessage, bool> predicate, CancellationToken cancellationToken = default)
        {
            return SubscribeAsync(subscriber, key, handler, predicate, Array.Empty<MessageHandlerFilter<TMessage>>(), cancellationToken);
        }

        public static ValueTask<IAsyncDisposable> SubscribeAsync<TKey, TMessage>(this IDistributedSubscriber<TKey, TMessage> subscriber, TKey key, Action<TMessage> handler, Func<TMessage, bool> predicate, MessageHandlerFilter<TMessage>[] filters, CancellationToken cancellationToken = default)
        {
            var predicateFilter = new PredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : Append(filters, predicateFilter);

            return subscriber.SubscribeAsync(key, new AnonymousMessageHandler<TMessage>(handler), filters, cancellationToken);
        }

        // async handler

        public static ValueTask<IAsyncDisposable> SubscribeAsync<TKey, TMessage>(this IDistributedSubscriber<TKey, TMessage> subscriber, TKey key, Func<TMessage, CancellationToken, ValueTask> handler, CancellationToken cancellationToken = default)
        {
            return SubscribeAsync(subscriber, key, handler, Array.Empty<AsyncMessageHandlerFilter<TMessage>>(), cancellationToken);
        }

        public static ValueTask<IAsyncDisposable> SubscribeAsync<TKey, TMessage>(this IDistributedSubscriber<TKey, TMessage> subscriber, TKey key, Func<TMessage, CancellationToken, ValueTask> handler, AsyncMessageHandlerFilter<TMessage>[] filters, CancellationToken cancellationToken = default)
        {
            return subscriber.SubscribeAsync(key, new AnonymousAsyncMessageHandler<TMessage>(handler), filters, cancellationToken);
        }

        public static ValueTask<IAsyncDisposable> SubscribeAsync<TKey, TMessage>(this IDistributedSubscriber<TKey, TMessage> subscriber, TKey key, Func<TMessage, CancellationToken, ValueTask> handler, Func<TMessage, bool> predicate, CancellationToken cancellationToken = default)
        {
            return SubscribeAsync(subscriber, key, handler, predicate, Array.Empty<AsyncMessageHandlerFilter<TMessage>>(), cancellationToken);
        }

        public static ValueTask<IAsyncDisposable> SubscribeAsync<TKey, TMessage>(this IDistributedSubscriber<TKey, TMessage> subscriber, TKey key, Func<TMessage, CancellationToken, ValueTask> handler, Func<TMessage, bool> predicate, AsyncMessageHandlerFilter<TMessage>[] filters, CancellationToken cancellationToken = default)
        {
            var predicateFilter = new AsyncPredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : Append(filters, predicateFilter);

            return subscriber.SubscribeAsync(key, new AnonymousAsyncMessageHandler<TMessage>(handler), filters, cancellationToken);
        }

        static T[] Append<T>(T[] source, T item)
        {
            var dest = new T[source.Length + 1];
            Array.Copy(source, 0, dest, 0, source.Length);
            dest[dest.Length - 1] = item;
            return dest;
        }
    }

    // utils

    internal sealed class AnonymousMessageHandler<TMessage> : IMessageHandler<TMessage>
    {
        readonly Action<TMessage> handler;

        public AnonymousMessageHandler(Action<TMessage> handler)
        {
            this.handler = handler;
        }

        public void Handle(TMessage message)
        {
            handler.Invoke(message);
        }
    }

    internal sealed class AnonymousAsyncMessageHandler<TMessage> : IAsyncMessageHandler<TMessage>
    {
        readonly Func<TMessage, CancellationToken, ValueTask> handler;

        public AnonymousAsyncMessageHandler(Func<TMessage, CancellationToken, ValueTask> handler)
        {
            this.handler = handler;
        }

        public ValueTask HandleAsync(TMessage message, CancellationToken cancellationToken)
        {
            return handler.Invoke(message, cancellationToken);
        }
    }

    internal sealed class PredicateFilter<T> : MessageHandlerFilter<T>
    {
        readonly Func<T, bool> predicate;

        public PredicateFilter(Func<T, bool> predicate)
        {
            this.predicate = predicate;
            this.Order = int.MinValue; // filter first.
        }

        // T and T2 should be same.
        public override void Handle(T message, Action<T> next)
        {
            if (predicate(message))
            {
                next(message);
            }
        }
    }

    internal sealed class AsyncPredicateFilter<T> : AsyncMessageHandlerFilter<T>
    {
        readonly Func<T, bool> predicate;

        public AsyncPredicateFilter(Func<T, bool> predicate)
        {
            this.predicate = predicate;
            this.Order = int.MinValue; // filter first.
        }

        public override ValueTask HandleAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next)
        {
            if (predicate(message))
            {
                return next(message, cancellationToken);
            }
            return default(ValueTask);
        }
    }
}