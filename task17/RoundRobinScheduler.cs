using System;
using System.Collections.Concurrent;

namespace task17;

public class RoundRobinScheduler : IScheduler
{
    private readonly ConcurrentQueue<ICommand> _queue = new ConcurrentQueue<ICommand>();

    public bool HasCommand() => !_queue.IsEmpty;

    public ICommand Select()
    {
        if (_queue.TryDequeue(out var cmd))
        {
            return cmd;
        }
        throw new InvalidOperationException("Планировщик пуст.");
    }

    public void Add(ICommand cmd)
    {
        _queue.Enqueue(cmd);
    }
}