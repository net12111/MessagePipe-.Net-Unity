﻿#if !UNITY_2018_3_OR_NEWER

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePipe
{
    public sealed class InMemoryDistributedPublisher<TKey, TMessage> : IDistributedPublisher<TKey, TMessage>
        where TKey : notnull
    {
        readonly IAsyncPublisher<TKey, TMessage> publisher;

        public InMemoryDistributedPublisher(IAsyncPublisher<TKey, TMessage> publisher)
        {
            this.publisher = publisher;
        }

        public ValueTask PublishAsync(TKey key, TMessage message, CancellationToken cancellationToken = default)
        {
            return publisher.PublishAsync(key, message, cancellationToken);
        }
    }

    public sealed class InMemoryDistributedSubscriber<TKey, TMessage> : IDistributedSubscriber<TKey, TMessage>
        where TKey : notnull
    {
        readonly IAsyncSubscriber<TKey, TMessage> subscriber;

        public InMemoryDistributedSubscriber(IAsyncSubscriber<TKey, TMessage> subscriber)
        {
            this.subscriber = subscriber;
        }

        public ValueTask<IAsyncDisposable> SubscribeAsync(TKey key, IMessageHandler<TMessage> handler, CancellationToken cancellationToken = default)
        {
            var d = subscriber.Subscribe(key, new AsyncMessageHandlerBridge<TMessage>(handler));
            return new ValueTask<IAsyncDisposable>(new AsyncDisposableBridge(d));
        }

        public ValueTask<IAsyncDisposable> SubscribeAsync(TKey key, IMessageHandler<TMessage> handler, MessageHandlerFilter<TMessage>[] filters, CancellationToken cancellationToken = default)
        {
            var d = subscriber.Subscribe(key, new AsyncMessageHandlerBridge<TMessage>(handler), filters.Select(x => new AsyncMessageHandlerFilterBridge<TMessage>(x)).ToArray());
            return new ValueTask<IAsyncDisposable>(new AsyncDisposableBridge(d));
        }

        public ValueTask<IAsyncDisposable> SubscribeAsync(TKey key, IAsyncMessageHandler<TMessage> handler, CancellationToken cancellationToken = default)
        {
            var d = subscriber.Subscribe(key, handler);
            return new ValueTask<IAsyncDisposable>(new AsyncDisposableBridge(d));
        }

        public ValueTask<IAsyncDisposable> SubscribeAsync(TKey key, IAsyncMessageHandler<TMessage> handler, AsyncMessageHandlerFilter<TMessage>[] filters, CancellationToken cancellationToken = default)
        {

            var d = subscriber.Subscribe(key, handler, filters);
            return new ValueTask<IAsyncDisposable>(new AsyncDisposableBridge(d));
        }
    }

    internal sealed class AsyncDisposableBridge : IAsyncDisposable
    {
        readonly IDisposable disposable;

        public AsyncDisposableBridge(IDisposable disposable)
        {
            this.disposable = disposable;
        }

        public ValueTask DisposeAsync()
        {
            disposable.Dispose();
            return default;
        }
    }

    internal sealed class AsyncMessageHandlerBridge<T> : IAsyncMessageHandler<T>
    {
        readonly IMessageHandler<T> handler;

        public AsyncMessageHandlerBridge(IMessageHandler<T> handler)
        {
            this.handler = handler;
        }

        public ValueTask HandleAsync(T message, CancellationToken cancellationToken)
        {
            handler.Handle(message);
            return default;
        }
    }

    internal sealed class AsyncMessageHandlerFilterBridge<T> : AsyncMessageHandlerFilter<T>
    {
        readonly MessageHandlerFilter<T> filter;

        public AsyncMessageHandlerFilterBridge(MessageHandlerFilter<T> filter)
        {
            this.filter = filter;
            this.Order = filter.Order;
        }

        public override ValueTask HandleAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next)
        {
            filter.Handle(message, async x => await next(x, cancellationToken));
            return default;
        }
    }
}

#endif