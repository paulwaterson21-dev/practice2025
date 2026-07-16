using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
    private readonly Thread _thread;
    private readonly IScheduler _scheduler;
    private Action _behavior;
    private bool _isStopped = false;

    public Thread Thread => _thread;
    public Action<ICommand, Exception>? ExceptionHandler { get; set; }

    public ServerThread() : this(new NullScheduler()) { }

    public ServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _behavior = DefaultBehavior;
        _thread = new Thread(Run);
    }

    public void Start() => _thread.Start();

    public void AddCommand(ICommand cmd)
    {
        if (_isStopped) throw new InvalidOperationException("Поток остановлен.");
        _queue.Add(cmd);
    }

    private void Run()
    {
        while (!_isStopped)
        {
            try
            {
                _behavior();
            }
            catch (StopThreadException)
            {
                break;
            }
            catch (InvalidOperationException)
            {
            
                break;
            }
            catch (ThreadInterruptedException)
            {
                break;
            }
        }
    }

    private void DefaultBehavior()
    {
        if (_scheduler.HasCommand())
        {
            if (_queue.TryTake(out var newCmd))
            {
                ExecuteCommand(newCmd);
            }

            if (_scheduler.HasCommand())
            {
                var scheduledCmd = _scheduler.Select();
                ExecuteCommand(scheduledCmd);
            }
        }
        else
        {
            var newCmd = _queue.Take();
            ExecuteCommand(newCmd);
        }
    }

    public void SoftStop()
    {
        _queue.CompleteAdding();
        _behavior = () =>
        {
            if (_queue.TryTake(out var cmd))
            {
                ExecuteCommand(cmd);
            }
            else if (_scheduler.HasCommand())
            {
                var scheduledCmd = _scheduler.Select();
                ExecuteCommand(scheduledCmd);
            }
            else
            {
                throw new StopThreadException();
            }
        };
    }

    public void HardStop()
    {
        _isStopped = true;
        _behavior = () => { throw new StopThreadException(); };
        _queue.CompleteAdding(); 
        _thread.Interrupt();    
    }

    private void ExecuteCommand(ICommand cmd)
    {
        try
        {
            cmd.Execute();
        }
        catch (Exception ex)
        {
            ExceptionHandler?.Invoke(cmd, ex);
        }
    }

    public void Stop()
    {
        HardStop();
    }
}

public class NullScheduler : IScheduler
{
    public bool HasCommand() => false;
    public ICommand Select() => throw new InvalidOperationException("Планировщик пуст.");
    public void Add(ICommand cmd) { }
}

public class StopThreadException : Exception { }