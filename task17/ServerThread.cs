using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
    private readonly Thread _thread;
    private Action _behavior;

    public Thread Thread => _thread;
    public Action<ICommand, Exception>? ExceptionHandler { get; set; }

    public ServerThread()
    {
        _behavior = DefaultBehavior;
        _thread = new Thread(Run);
    }

    public void Start() => _thread.Start();

    public void AddCommand(ICommand cmd) => _queue.Add(cmd);

    private void Run()
    {
        while (true)
        {
            try
            {
                _behavior();
            }
            catch (StopThreadException)
            {
                break;
            }
        }
    }

    private void DefaultBehavior()
    {
        var cmd = _queue.Take();
        try
        {
            cmd.Execute();
        }
        catch (Exception ex)
        {
            ExceptionHandler?.Invoke(cmd, ex);
        }
    }

    public void SoftStop()
    {
        _behavior = () =>
        {
            if (_queue.TryTake(out var cmd))
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
            else
            {
                throw new StopThreadException();
            }
        }; 
    }

    public void HardStop()
    {
        _behavior = () => throw new StopThreadException();
    }
}

public class StopThreadException : Exception { }