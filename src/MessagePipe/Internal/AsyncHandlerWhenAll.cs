﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePipe.Internal
{
    internal static class ContinuationSentinel
    {
        public static readonly Action AvailableContinuation = () => { };
        public static readonly Action CompletedContinuation = () => { };
    }

    internal class AsyncHandlerWhenAll<T> : ICriticalNotifyCompletion
    {
        readonly int taskCount = 0;

        int completedCount = 0;
        ExceptionDispatchInfo? exception;
        Action continuation = ContinuationSentinel.AvailableContinuation;

        public AsyncHandlerWhenAll(IAsyncMessageHandler<T>?[] handlers, T message, CancellationToken cancellationtoken)
        {
            taskCount = handlers.Length;

            foreach (var item in handlers)
            {
                if (item == null)
                {
                    IncrementSuccessfully();
                }
                else
                {
                    ValueTask task;
                    try
                    {
                        task = item.HandleAsync(message, cancellationtoken);
                    }
                    catch (Exception ex)
                    {
                        exception = ExceptionDispatchInfo.Capture(ex);
                        TryInvokeContinuation();
                        return;
                    }

                    HandleTask(task);
                }
            }
        }

        async void HandleTask(ValueTask task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exception = ExceptionDispatchInfo.Capture(ex);
                TryInvokeContinuation();
                return;
            }

            IncrementSuccessfully();
        }

        void IncrementSuccessfully()
        {
            if (Interlocked.Increment(ref completedCount) == taskCount)
            {
                TryInvokeContinuation();
            }
        }

        void TryInvokeContinuation()
        {
            var c = Interlocked.Exchange(ref continuation, ContinuationSentinel.CompletedContinuation); // register completed.
            if (c != ContinuationSentinel.AvailableContinuation && c != ContinuationSentinel.CompletedContinuation)
            {
                c();
            }
        }

        // Awaiter

        public AsyncHandlerWhenAll<T> GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted => exception != null || completedCount == taskCount;

        public void GetResult()
        {
            if (exception != null)
            {
                exception.Throw();
            }
            // Complete, OK.
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            var c = Interlocked.CompareExchange(ref this.continuation, continuation, ContinuationSentinel.AvailableContinuation);
            if (c == ContinuationSentinel.CompletedContinuation) // registered TryInvokeContinuation first.
            {
                continuation();
                return;
            }
        }
    }
}
