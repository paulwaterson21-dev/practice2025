using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    // === ТЕСТЫ TASK 17 ===

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
        var server = new ServerThread();
        server.Start();

        bool executedAfterStop = false;

        server.AddCommand(new SoftStopCommand(server));
        server.AddCommand(new ActionCommand(() => executedAfterStop = true));

        server.Thread.Join(1000);

        Assert.False(server.Thread.IsAlive);
        Assert.True(executedAfterStop);
    }

    // === ТЕСТ TASK 18 ===

    [Fact]
    public void LongRunningCommands_ShouldInterleave_RoundRobin()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();

        var executionLog = new List<string>();
        var lockObj = new object();

        void LogStep(string name, int step)
        {
            lock (lockObj)
            {
                executionLog.Add($"{name}_{step}");
            }
        }

        var cmdA = new LongRunningCommand("A", 3, scheduler, LogStep);
        var cmdB = new LongRunningCommand("B", 3, scheduler, LogStep);

        server.AddCommand(cmdA);
        server.AddCommand(cmdB);

        Thread.Sleep(500);
        server.Stop();


        Assert.Equal(6, executionLog.Count);
        Assert.Equal("A_1", executionLog[0]);
        Assert.Equal("B_1", executionLog[1]);
        Assert.Equal("A_2", executionLog[2]);
        Assert.Equal("B_2", executionLog[3]);
        Assert.Equal("A_3", executionLog[4]);
        Assert.Equal("B_3", executionLog[5]);
    }

    // === ТЕСТ TASK 19 ===

    [Fact]
    public void Task19_FiveTestCommands_ThreeTimesEach_ThenHardStop()
    {

        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();

        for (int i = 1; i <= 5; i++)
        {
            server.AddCommand(new TestCommand(i, scheduler));
        }

        Thread.Sleep(1000);

        server.HardStop();
        server.Thread.Join(1000);

        Assert.False(server.Thread.IsAlive);
    }
}

public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute() => _action();
}