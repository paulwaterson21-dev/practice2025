using System;

namespace task17;

public class TestCommand(int id, IScheduler scheduler) : ICommand
{
    private int counter = 0;

    public void Execute()
    {
        Console.WriteLine($"Поток {id} вызов {++counter}");


        if (counter < 3)
        {
            scheduler.Add(this);
        }
    }
}