namespace TeaTime.Slack
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Abstractions;
    using Common.Features.Runs.Events;
    using MediatR;

    public interface IRunEventListener
    {
        void Trigger<T>(T evt) where T : IEvent;
        Task<TResult> WaitOnceAsync<TIn, TResult>(Func<TIn, Task<TResult>> func, TimeSpan timeout);
    }

    public class RunEndedListener : INotificationHandler<RunEndedEvent>
    {
        private readonly IRunEventListener _eventListener;

        public RunEndedListener(IRunEventListener eventListener)
        {
            _eventListener = eventListener;
        }

        public Task Handle(RunEndedEvent notification, CancellationToken cancellationToken)
        {
            _eventListener.Trigger(notification);

            return Task.CompletedTask;
        }
    }

    public class RunEventListener : IRunEventListener
    {
        private readonly Dictionary<Type, List<TaskCompletionSource<object>>> _waitHandles = new Dictionary<Type, List<TaskCompletionSource<object>>>();
        private readonly object _lock = new object();

        public async Task<TResult> WaitOnceAsync<TIn, TResult>(Func<TIn, Task<TResult>> func, TimeSpan timeout)
        {
            var evtType = typeof(TIn);
            var handle = new TaskCompletionSource<object>();

            lock (_lock)
            {
                if (!_waitHandles.TryGetValue(evtType, out var handles))
                {
                    handles = new List<TaskCompletionSource<object>>();
                    _waitHandles.Add(evtType, handles);
                }

                handles.Add(handle);
            }

            using (var cts = new CancellationTokenSource(timeout))
            using (cts.Token.Register(() => handle.TrySetCanceled(), false))
            {
                var result = await handle.Task.ConfigureAwait(false);
                return (TResult)result;
            }
        }

        public void Trigger<T>(T evt) where T : IEvent
        {
            var evtType = typeof(T);

            List<TaskCompletionSource<object>> callbacks;

            //get all the callbacks to run and remove them from the dictionary (ie run once)
            lock (_lock)
            {
                if (!_waitHandles.TryGetValue(evtType, out callbacks))
                    return;

                _waitHandles.Remove(evtType);
            }

            foreach (var cb in callbacks)
            {
                cb.TrySetResult(evt);
            }
        }
    }
}
