using System;

namespace task17;

public class LongRunningCommand : ICommand
{
    private readonly IScheduler _scheduler;
    private readonly string _name;
    private readonly int _totalSteps;
    private int _currentStep = 0;
    private readonly Action<string, int> _onStepExecute;

    public bool IsCompleted => _currentStep >= _totalSteps;

    public LongRunningCommand(string name, int totalSteps, IScheduler scheduler, Action<string, int> onStepExecute)
    {
        _name = name;
        _totalSteps = totalSteps;
        _scheduler = scheduler;
        _onStepExecute = onStepExecute;
    }

    public void Execute()
    {
        if (IsCompleted) return;

        _currentStep++;
        _onStepExecute(_name, _currentStep);

        if (!IsCompleted)
        {

            _scheduler.Add(this);
        }
    }
}