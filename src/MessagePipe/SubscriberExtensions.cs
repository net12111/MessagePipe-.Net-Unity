﻿using MessagePipe.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace MessagePipe
{
    public static partial class SubscriberExtensions
    {
        // pubsub-keyless-sync

        public static IDisposable Subscribe<TMessage>(this ISubscriber<TMessage> subscriber, Action<TMessage> handler, params MessageHandlerFilter<TMessage>[] filters)
        {
            return subscriber.Subscribe(new AnonymousMessageHandler<TMessage>(handler), filters);
        }

        public static IDisposable Subscribe<TMessage>(this ISubscriber<TMessage> subscriber, Action<TMessage> handler, Func<TMessage, bool> predicate, params MessageHandlerFilter<TMessage>[] filters)
        {
            var predicateFilter = new PredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return subscriber.Subscribe(new AnonymousMessageHandler<TMessage>(handler), filters);
        }

        public static ValueTask<TMessage> FirstAsync<TMessage>(this ISubscriber<TMessage> subscriber, CancellationToken cancellationToken, params MessageHandlerFilter<TMessage>[] filters)
        {
            return new ValueTask<TMessage>(new FirstAsyncMessageHandler<TMessage>(subscriber, cancellationToken, filters), 0);
        }

        public static ValueTask<TMessage> FirstAsync<TMessage>(this ISubscriber<TMessage> subscriber, CancellationToken cancellationToken, Func<TMessage, bool> predicate, params MessageHandlerFilter<TMessage>[] filters)
        {
            var predicateFilter = new PredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return new ValueTask<TMessage>(new FirstAsyncMessageHandler<TMessage>(subscriber, cancellationToken, filters), 0);
        }

        public static IObservable<TMessage> AsObservable<TMessage>(this ISubscriber<TMessage> subscriber, params MessageHandlerFilter<TMessage>[] filters)
        {
            return new ObservableSubscriber<TMessage>(subscriber, filters);
        }

        public static IDisposable Subscribe<TMessage>(this IBufferedSubscriber<TMessage> subscriber, Action<TMessage> handler, params MessageHandlerFilter<TMessage>[] filters)
        {
            return subscriber.Subscribe(new AnonymousMessageHandler<TMessage>(handler), filters);
        }

        public static IDisposable Subscribe<TMessage>(this IBufferedSubscriber<TMessage> subscriber, Action<TMessage> handler, Func<TMessage, bool> predicate, params MessageHandlerFilter<TMessage>[] filters)
        {
            var predicateFilter = new PredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return subscriber.Subscribe(new AnonymousMessageHandler<TMessage>(handler), filters);
        }

        public static ValueTask<TMessage> FirstAsync<TMessage>(this IBufferedSubscriber<TMessage> subscriber, CancellationToken cancellationToken, params MessageHandlerFilter<TMessage>[] filters)
        {
            return new ValueTask<TMessage>(new FirstAsyncBufferedMessageHandler<TMessage>(subscriber, cancellationToken, filters), 0);
        }

        public static ValueTask<TMessage> FirstAsync<TMessage>(this IBufferedSubscriber<TMessage> subscriber, CancellationToken cancellationToken, Func<TMessage, bool> predicate, params MessageHandlerFilter<TMessage>[] filters)
        {
            var predicateFilter = new PredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return new ValueTask<TMessage>(new FirstAsyncBufferedMessageHandler<TMessage>(subscriber, cancellationToken, filters), 0);
        }

        public static IObservable<TMessage> AsObservable<TMessage>(this IBufferedSubscriber<TMessage> subscriber, params MessageHandlerFilter<TMessage>[] filters)
        {
            return new ObservableBufferedSubscriber<TMessage>(subscriber, filters);
        }

        // pubsub-keyless-async

        public static IDisposable Subscribe<TMessage>(this IAsyncSubscriber<TMessage> subscriber, Func<TMessage, CancellationToken, ValueTask> handler, params AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            return subscriber.Subscribe(new AnonymousAsyncMessageHandler<TMessage>(handler), filters);
        }

        public static IDisposable Subscribe<TMessage>(this IAsyncSubscriber<TMessage> subscriber, Func<TMessage, CancellationToken, ValueTask> handler, Func<TMessage, bool> predicate, params AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            var predicateFilter = new AsyncPredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return subscriber.Subscribe(new AnonymousAsyncMessageHandler<TMessage>(handler), filters);
        }

        public static ValueTask<TMessage> FirstAsync<TMessage>(this IAsyncSubscriber<TMessage> subscriber, CancellationToken cancellationToken, params AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            return new ValueTask<TMessage>(new FirstAsyncAsyncMessageHandler<TMessage>(subscriber, cancellationToken, filters), 0);
        }

        public static ValueTask<TMessage> FirstAsync<TMessage>(this IAsyncSubscriber<TMessage> subscriber, CancellationToken cancellationToken, Func<TMessage, bool> predicate, params AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            var predicateFilter = new AsyncPredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return new ValueTask<TMessage>(new FirstAsyncAsyncMessageHandler<TMessage>(subscriber, cancellationToken, filters), 0);
        }

        public static ValueTask<IDisposable> SubscribeAsync<TMessage>(this IBufferedAsyncSubscriber<TMessage> subscriber, Func<TMessage, CancellationToken, ValueTask> handler, CancellationToken cancellationToken = default)
        {
            return SubscribeAsync(subscriber, handler, Array.Empty<AsyncMessageHandlerFilter<TMessage>>(), cancellationToken);
        }

        public static ValueTask<IDisposable> SubscribeAsync<TMessage>(this IBufferedAsyncSubscriber<TMessage> subscriber, Func<TMessage, CancellationToken, ValueTask> handler, AsyncMessageHandlerFilter<TMessage>[] filters, CancellationToken cancellationToken = default)
        {
            return subscriber.SubscribeAsync(new AnonymousAsyncMessageHandler<TMessage>(handler), filters, cancellationToken);
        }

        public static ValueTask<IDisposable> SubscribeAsync<TMessage>(this IBufferedAsyncSubscriber<TMessage> subscriber, Func<TMessage, CancellationToken, ValueTask> handler, Func<TMessage, bool> predicate, CancellationToken cancellationToken = default)
        {
            return SubscribeAsync(subscriber, handler, predicate, Array.Empty<AsyncMessageHandlerFilter<TMessage>>(), cancellationToken);
        }

        public static ValueTask<IDisposable> SubscribeAsync<TMessage>(this IBufferedAsyncSubscriber<TMessage> subscriber, Func<TMessage, CancellationToken, ValueTask> handler, Func<TMessage, bool> predicate, AsyncMessageHandlerFilter<TMessage>[] filters, CancellationToken cancellationToken = default)
        {
            var predicateFilter = new AsyncPredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return subscriber.SubscribeAsync(new AnonymousAsyncMessageHandler<TMessage>(handler), filters, cancellationToken);
        }

        public static async ValueTask<TMessage> FirstAsync<TMessage>(this IBufferedAsyncSubscriber<TMessage> subscriber, CancellationToken cancellationToken, params AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            return await new ValueTask<TMessage>(await FirstAsyncAsyncBufferedMessageHandler<TMessage>.CreateAsync(subscriber, cancellationToken, filters), 0);
        }

        public static async ValueTask<TMessage> FirstAsync<TMessage>(this IBufferedAsyncSubscriber<TMessage> subscriber, CancellationToken cancellationToken, Func<TMessage, bool> predicate, params AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            var predicateFilter = new AsyncPredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return await new ValueTask<TMessage>(await FirstAsyncAsyncBufferedMessageHandler<TMessage>.CreateAsync(subscriber, cancellationToken, filters), 0);
        }

        // pubsub-key-sync

        public static IDisposable Subscribe<TKey, TMessage>(this ISubscriber<TKey, TMessage> subscriber, TKey key, Action<TMessage> handler, params MessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            return subscriber.Subscribe(key, new AnonymousMessageHandler<TMessage>(handler), filters);
        }

        public static IDisposable Subscribe<TKey, TMessage>(this ISubscriber<TKey, TMessage> subscriber, TKey key, Action<TMessage> handler, Func<TMessage, bool> predicate, params MessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            var predicateFilter = new PredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return subscriber.Subscribe(key, new AnonymousMessageHandler<TMessage>(handler), filters);
        }

        public static ValueTask<TMessage> FirstAsync<TKey, TMessage>(this ISubscriber<TKey, TMessage> subscriber, TKey key, CancellationToken cancellationToken, params MessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            return new ValueTask<TMessage>(new FirstAsyncMessageHandler<TKey, TMessage>(subscriber, key, cancellationToken, filters), 0);
        }

        public static ValueTask<TMessage> FirstAsync<TKey, TMessage>(this ISubscriber<TKey, TMessage> subscriber, TKey key, CancellationToken cancellationToken, Func<TMessage, bool> predicate, params MessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            var predicateFilter = new PredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return new ValueTask<TMessage>(new FirstAsyncMessageHandler<TKey, TMessage>(subscriber, key, cancellationToken, filters), 0);
        }

        public static IObservable<TMessage> AsObservable<TKey, TMessage>(this ISubscriber<TKey, TMessage> subscriber, TKey key, params MessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            return new ObservableSubscriber<TKey, TMessage>(key, subscriber, filters);
        }

        // pubsub-key-async

        public static IDisposable Subscribe<TKey, TMessage>(this IAsyncSubscriber<TKey, TMessage> subscriber, TKey key, Func<TMessage, CancellationToken, ValueTask> handler, params AsyncMessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            return subscriber.Subscribe(key, new AnonymousAsyncMessageHandler<TMessage>(handler), filters);
        }

        public static IDisposable Subscribe<TKey, TMessage>(this IAsyncSubscriber<TKey, TMessage> subscriber, TKey key, Func<TMessage, CancellationToken, ValueTask> handler, Func<TMessage, bool> predicate, params AsyncMessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            var predicateFilter = new AsyncPredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return subscriber.Subscribe(key, new AnonymousAsyncMessageHandler<TMessage>(handler), filters);
        }

        public static ValueTask<TMessage> FirstAsync<TKey, TMessage>(this IAsyncSubscriber<TKey, TMessage> subscriber, TKey key, CancellationToken cancellationToken, params AsyncMessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            return new ValueTask<TMessage>(new FirstAsyncAsyncMessageHandler<TKey, TMessage>(subscriber, key, cancellationToken, filters), 0);
        }

        public static ValueTask<TMessage> FirstAsync<TKey, TMessage>(this IAsyncSubscriber<TKey, TMessage> subscriber, TKey key, CancellationToken cancellationToken, Func<TMessage, bool> predicate, params AsyncMessageHandlerFilter<TMessage>[] filters)
            where TKey : notnull
        {
            var predicateFilter = new AsyncPredicateFilter<TMessage>(predicate);
            filters = (filters.Length == 0)
                ? new[] { predicateFilter }
                : ArrayUtil.ImmutableAdd(filters, predicateFilter);

            return new ValueTask<TMessage>(new FirstAsyncAsyncMessageHandler<TKey, TMessage>(subscriber, key, cancellationToken, filters), 0);
        }
    }

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

    internal sealed class FirstAsyncMessageHandler<TKey, TMessage> : IMessageHandler<TMessage>, IValueTaskSource<TMessage>
        where TKey : notnull
    {
        int handleCalled = 0;
        IDisposable? subscription;
        CancellationToken cancellationToken;
        CancellationTokenRegistration cancellationTokenRegistration;
        ManualResetValueTaskSourceCore<TMessage> core;

        static readonly Action<object?> cancelCallback = Cancel;

        public FirstAsyncMessageHandler(ISubscriber<TKey, TMessage> subscriber, TKey key, CancellationToken cancellationToken, MessageHandlerFilter<TMessage>[] filters)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                this.core.SetException(new OperationCanceledException(cancellationToken));
                return;
            }

            try
            {
                this.subscription = subscriber.Subscribe(key, this, filters);
            }
            catch (Exception ex)
            {
                this.core.SetException(ex);
                return;
            }

            if (handleCalled != 0)
            {
                this.subscription?.Dispose();
                return;
            }

            if (cancellationToken.CanBeCanceled)
            {
                this.cancellationToken = cancellationToken;
                this.cancellationTokenRegistration = cancellationToken.Register(cancelCallback, this, false);
            }
        }

        static void Cancel(object? state)
        {
            var self = (FirstAsyncMessageHandler<TKey, TMessage>)state!;
            self.subscription?.Dispose();
            self.core.SetException(new OperationCanceledException(self.cancellationToken));
        }

        public void Handle(TMessage message)
        {
            if (Interlocked.Increment(ref handleCalled) == 1)
            {
                try
                {
                    core.SetResult(message);
                }
                finally
                {
                    subscription?.Dispose();
                    cancellationTokenRegistration.Dispose();
                }
            }
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            core.OnCompleted(continuation, state, token, flags);
        }

        public TMessage GetResult(short token)
        {
            return core.GetResult(token);
        }
    }
    internal sealed class FirstAsyncMessageHandler<TMessage> : IMessageHandler<TMessage>, IValueTaskSource<TMessage>
    {
        int handleCalled = 0;
        IDisposable? subscription;
        CancellationToken cancellationToken;
        CancellationTokenRegistration cancellationTokenRegistration;
        ManualResetValueTaskSourceCore<TMessage> core;

        static readonly Action<object?> cancelCallback = Cancel;

        public FirstAsyncMessageHandler(ISubscriber<TMessage> subscriber, CancellationToken cancellationToken, MessageHandlerFilter<TMessage>[] filters)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                this.core.SetException(new OperationCanceledException(cancellationToken));
                return;
            }

            try
            {
                this.subscription = subscriber.Subscribe(this, filters);
            }
            catch (Exception ex)
            {
                this.core.SetException(ex);
                return;
            }

            if (handleCalled != 0)
            {
                this.subscription?.Dispose();
                return;
            }

            if (cancellationToken.CanBeCanceled)
            {
                this.cancellationToken = cancellationToken;
                this.cancellationTokenRegistration = cancellationToken.Register(cancelCallback, this, false);
            }
        }

        static void Cancel(object? state)
        {
            var self = (FirstAsyncMessageHandler<TMessage>)state!;
            self.subscription?.Dispose();
            self.core.SetException(new OperationCanceledException(self.cancellationToken));
        }

        public void Handle(TMessage message)
        {
            if (Interlocked.Increment(ref handleCalled) == 1)
            {
                try
                {
                    core.SetResult(message);
                }
                finally
                {
                    subscription?.Dispose();
                    cancellationTokenRegistration.Dispose();
                }
            }
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            core.OnCompleted(continuation, state, token, flags);
        }

        public TMessage GetResult(short token)
        {
            return core.GetResult(token);
        }
    }

    internal sealed class FirstAsyncBufferedMessageHandler<TMessage> : IMessageHandler<TMessage>, IValueTaskSource<TMessage>
    {
        int handleCalled = 0;
        IDisposable? subscription;
        CancellationToken cancellationToken;
        CancellationTokenRegistration cancellationTokenRegistration;
        ManualResetValueTaskSourceCore<TMessage> core;

        static readonly Action<object?> cancelCallback = Cancel;

        public FirstAsyncBufferedMessageHandler(IBufferedSubscriber<TMessage> subscriber, CancellationToken cancellationToken, MessageHandlerFilter<TMessage>[] filters)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                this.core.SetException(new OperationCanceledException(cancellationToken));
                return;
            }

            try
            {
                this.subscription = subscriber.Subscribe(this, filters);
            }
            catch (Exception ex)
            {
                this.core.SetException(ex);
                return;
            }

            if (handleCalled != 0)
            {
                this.subscription?.Dispose();
                return;
            }

            if (cancellationToken.CanBeCanceled)
            {
                this.cancellationToken = cancellationToken;
                this.cancellationTokenRegistration = cancellationToken.Register(cancelCallback, this, false);
            }
        }

        static void Cancel(object? state)
        {
            var self = (FirstAsyncBufferedMessageHandler<TMessage>)state!;
            self.subscription?.Dispose();
            self.core.SetException(new OperationCanceledException(self.cancellationToken));
        }

        public void Handle(TMessage message)
        {
            if (Interlocked.Increment(ref handleCalled) == 1)
            {
                try
                {
                    core.SetResult(message);
                }
                finally
                {
                    subscription?.Dispose();
                    cancellationTokenRegistration.Dispose();
                }
            }
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            core.OnCompleted(continuation, state, token, flags);
        }

        public TMessage GetResult(short token)
        {
            return core.GetResult(token);
        }
    }

    internal sealed class FirstAsyncAsyncMessageHandler<TKey, TMessage> : IAsyncMessageHandler<TMessage>, IValueTaskSource<TMessage>
        where TKey : notnull
    {
        int handleCalled = 0;
        IDisposable? subscription;
        CancellationToken cancellationToken;
        CancellationTokenRegistration cancellationTokenRegistration;
        ManualResetValueTaskSourceCore<TMessage> core;

        static readonly Action<object?> cancelCallback = Cancel;

        public FirstAsyncAsyncMessageHandler(IAsyncSubscriber<TKey, TMessage> subscriber, TKey key, CancellationToken cancellationToken, AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                this.core.SetException(new OperationCanceledException(cancellationToken));
                return;
            }

            try
            {
                this.subscription = subscriber.Subscribe(key, this, filters);
            }
            catch (Exception ex)
            {
                this.core.SetException(ex);
                return;
            }

            if (handleCalled != 0)
            {
                this.subscription?.Dispose();
                return;
            }

            if (cancellationToken.CanBeCanceled)
            {
                this.cancellationToken = cancellationToken;
                this.cancellationTokenRegistration = cancellationToken.Register(cancelCallback, this, false);
            }
        }

        static void Cancel(object? state)
        {
            var self = (FirstAsyncAsyncMessageHandler<TKey, TMessage>)state!;
            self.subscription?.Dispose();
            self.core.SetException(new OperationCanceledException(self.cancellationToken));
        }

        public ValueTask HandleAsync(TMessage message, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref handleCalled) == 1)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        core.SetException(new OperationCanceledException(cancellationToken));
                    }
                    else
                    {
                        core.SetResult(message);
                    }
                }
                finally
                {
                    subscription?.Dispose();
                    cancellationTokenRegistration.Dispose();
                }
            }
            return default;
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            core.OnCompleted(continuation, state, token, flags);
        }

        public TMessage GetResult(short token)
        {
            return core.GetResult(token);
        }
    }

    internal sealed class FirstAsyncAsyncMessageHandler<TMessage> : IAsyncMessageHandler<TMessage>, IValueTaskSource<TMessage>
    {
        int handleCalled = 0;
        IDisposable? subscription;
        CancellationToken cancellationToken;
        CancellationTokenRegistration cancellationTokenRegistration;
        ManualResetValueTaskSourceCore<TMessage> core;

        static readonly Action<object?> cancelCallback = Cancel;

        public FirstAsyncAsyncMessageHandler(IAsyncSubscriber<TMessage> subscriber, CancellationToken cancellationToken, AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                this.core.SetException(new OperationCanceledException(cancellationToken));
                return;
            }

            try
            {
                this.subscription = subscriber.Subscribe(this, filters);
            }
            catch (Exception ex)
            {
                this.core.SetException(ex);
                return;
            }

            if (handleCalled != 0)
            {
                this.subscription?.Dispose();
                return;
            }

            if (cancellationToken.CanBeCanceled)
            {
                this.cancellationToken = cancellationToken;
                this.cancellationTokenRegistration = cancellationToken.Register(cancelCallback, this, false);
            }
        }

        static void Cancel(object? state)
        {
            var self = (FirstAsyncAsyncMessageHandler<TMessage>)state!;
            self.subscription?.Dispose();
            self.core.SetException(new OperationCanceledException(self.cancellationToken));
        }

        public ValueTask HandleAsync(TMessage message, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref handleCalled) == 1)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        core.SetException(new OperationCanceledException(cancellationToken));
                    }
                    else
                    {
                        core.SetResult(message);
                    }
                }
                finally
                {
                    subscription?.Dispose();
                    cancellationTokenRegistration.Dispose();
                }
            }
            return default;
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            core.OnCompleted(continuation, state, token, flags);
        }

        public TMessage GetResult(short token)
        {
            return core.GetResult(token);
        }
    }

    internal sealed class FirstAsyncAsyncBufferedMessageHandler<TMessage> : IAsyncMessageHandler<TMessage>, IValueTaskSource<TMessage>
    {
        int handleCalled = 0;
        IDisposable? subscription;
        CancellationToken cancellationToken;
        CancellationTokenRegistration cancellationTokenRegistration;
        ManualResetValueTaskSourceCore<TMessage> core;

        static readonly Action<object?> cancelCallback = Cancel;

        public static async ValueTask<FirstAsyncAsyncBufferedMessageHandler<TMessage>> CreateAsync(IBufferedAsyncSubscriber<TMessage> subscriber, CancellationToken cancellationToken, AsyncMessageHandlerFilter<TMessage>[] filters)
        {
            var self = new FirstAsyncAsyncBufferedMessageHandler<TMessage>();
            if (cancellationToken.IsCancellationRequested)
            {
                self.core.SetException(new OperationCanceledException(cancellationToken));
                return self;
            }

            try
            {
                self.subscription = await subscriber.SubscribeAsync(self, filters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                self.core.SetException(ex);
                return self;
            }

            if (self.handleCalled != 0)
            {
                self.subscription?.Dispose();
                return self;
            }

            if (cancellationToken.CanBeCanceled)
            {
                self.cancellationToken = cancellationToken;
                self.cancellationTokenRegistration = cancellationToken.Register(cancelCallback, self, false);
            }
            return self;
        }

        static void Cancel(object? state)
        {
            var self = (FirstAsyncAsyncBufferedMessageHandler<TMessage>)state!;
            self.subscription?.Dispose();
            self.core.SetException(new OperationCanceledException(self.cancellationToken));
        }

        public ValueTask HandleAsync(TMessage message, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref handleCalled) == 1)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        core.SetException(new OperationCanceledException(cancellationToken));
                    }
                    else
                    {
                        core.SetResult(message);
                    }
                }
                finally
                {
                    subscription?.Dispose();
                    cancellationTokenRegistration.Dispose();
                }
            }
            return default;
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            core.OnCompleted(continuation, state, token, flags);
        }

        public TMessage GetResult(short token)
        {
            return core.GetResult(token);
        }
    }

    internal sealed class ObservableSubscriber<TKey, TMessage> : IObservable<TMessage>
        where TKey : notnull
    {
        readonly TKey key;
        readonly ISubscriber<TKey, TMessage> subscriber;
        readonly MessageHandlerFilter<TMessage>[] filters;

        public ObservableSubscriber(TKey key, ISubscriber<TKey, TMessage> subscriber, MessageHandlerFilter<TMessage>[] filters)
        {
            this.key = key;
            this.subscriber = subscriber;
            this.filters = filters;
        }

        public IDisposable Subscribe(IObserver<TMessage> observer)
        {
            return subscriber.Subscribe(key, new ObserverMessageHandler<TMessage>(observer), filters);
        }
    }

    internal sealed class ObservableSubscriber<TMessage> : IObservable<TMessage>
    {
        readonly ISubscriber<TMessage> subscriber;
        readonly MessageHandlerFilter<TMessage>[] filters;

        public ObservableSubscriber(ISubscriber<TMessage> subscriber, MessageHandlerFilter<TMessage>[] filters)
        {
            this.subscriber = subscriber;
            this.filters = filters;
        }

        public IDisposable Subscribe(IObserver<TMessage> observer)
        {
            return subscriber.Subscribe(new ObserverMessageHandler<TMessage>(observer), filters);
        }
    }

    internal sealed class ObservableBufferedSubscriber<TMessage> : IObservable<TMessage>
    {
        readonly IBufferedSubscriber<TMessage> subscriber;
        readonly MessageHandlerFilter<TMessage>[] filters;

        public ObservableBufferedSubscriber(IBufferedSubscriber<TMessage> subscriber, MessageHandlerFilter<TMessage>[] filters)
        {
            this.subscriber = subscriber;
            this.filters = filters;
        }

        public IDisposable Subscribe(IObserver<TMessage> observer)
        {
            return subscriber.Subscribe(new ObserverMessageHandler<TMessage>(observer), filters);
        }
    }

    internal sealed class ObserverMessageHandler<TMessage> : IMessageHandler<TMessage>
    {
        readonly IObserver<TMessage> observer;

        public ObserverMessageHandler(IObserver<TMessage> observer)
        {
            this.observer = observer;
        }

        public void Handle(TMessage message)
        {
            observer.OnNext(message);
        }
    }
}