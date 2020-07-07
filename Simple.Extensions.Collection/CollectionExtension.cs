using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Simple.Extensions.Collection
{
    public static class CollectionExtension
    {
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var t in enumerable)
            {
                action.Invoke(t);
            }
        }

        public static async Task EachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action, int maxThreads, CancellationToken token)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var actionBlock = new ActionBlock<T>(
                async i => { await action.Invoke(i); },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = maxThreads, 
                    CancellationToken = token
                }
            );

            foreach (var i in enumerable)
            {
                actionBlock.Post(i);
            }
            actionBlock.Complete();

            await actionBlock.Completion;
        } 
    }
}
