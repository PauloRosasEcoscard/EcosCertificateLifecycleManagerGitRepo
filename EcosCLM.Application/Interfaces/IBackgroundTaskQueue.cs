using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcosCLM.Application.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        public ValueTask QueueBackgroundWorkItemAsync(
            Func<CancellationToken, ValueTask> workItem);

        public ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
