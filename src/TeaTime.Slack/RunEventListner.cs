namespace TeaTime.Slack
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Abstractions;

    public interface IRunEventListener
    {
        void Trigger<T>(T evt) where T : IEvent;
        Task<TEvent> WaitOnceAsync<TEvent>(TimeSpan timeout);
    }

    public class RunEventListener : IRunEventListener
    {
        private readonly Dictionary<Type, List<TaskCompletionSource<object>>> _waitHandles = new Dictionary<Type, List<TaskCompletionSource<object>>>();
        private readonly object _lock = new object();

        public async Task<TEvent> WaitOnceAsync<TEvent>(TimeSpan timeout)
        {
            var evtType = typeof(TEvent);
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
            using (cts.Token.Register(() => OnTimeout<TEvent>(handle), false))
            {
                var result = await handle.Task.ConfigureAwait(false);
                return (TEvent) result;
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

        private void OnTimeout<TEvent>(TaskCompletionSource<object> tcs)
        {
            if (!tcs.TrySetCanceled())
                return;

            var evtType = typeof(TEvent);

            lock (_lock)
            {
                if (!_waitHandles.TryGetValue(evtType, out var callbacks))
                    return;

                callbacks.Remove(tcs);

                if (callbacks.Count == 0)
                    _waitHandles.Remove(evtType);
            }
        }
    }
}
