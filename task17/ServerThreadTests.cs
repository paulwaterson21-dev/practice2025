using System.Threading;
using Xunit;
using task17;

namespace task17tests;


public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute() => _action();
}

public class ServerThreadTests
{
    [Fact]
    public void HardStop_StopsImmediately_IgnoresQueue()
    {
        var server = new ServerThread();
        server.Start();

        bool executedAfterStop = false;

        server.AddCommand(new HardStopCommand(server));
        server.AddCommand(new ActionCommand(() => executedAfterStop = true));

        server.Thread.Join(1000);

        Assert.False(server.Thread.IsAlive);
        Assert.False(executedAfterStop);
    }

    [Fact]
    public void SoftStop_FinishesQueue_ThenStops()
    {
        var server= new ServerThread();
        server.Start();

        bool executedAfterStop = false;

        server.AddCommand(new SoftStopCommand(server));
        server.AddCommand(new ActionCommand(() => executedAfterStop = true));

        server.Thread.Join(1000);

        Assert.False(server.Thread.IsAlive);
        Assert.True(executedAfterStop);
    }

    [Fact]
    public void StopCommands_ThrowException_IfExecutedFromWrongThread()
    {
        var server =new ServerThread();
        var hardCommand =new HardStopCommand(server);
        var softCommand = new SoftStopCommand(server);

        Assert.Throws<InvalidOperationException>(() => hardCommand.Execute());
        Assert.Throws<InvalidOperationException>(() => softCommand.Execute());
    }
}