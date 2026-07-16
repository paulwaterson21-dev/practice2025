namespace task17;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _server;

    public HardStopCommand(ServerThread server) => _server = server;

    public void Execute()
    {
        if (Thread.CurrentThread != _server.Thread)
        {
            throw new InvalidOperationException("HardStopCommand может быть выполнена только внутри целевого потока.");
        }
        _server.HardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _server;

    public SoftStopCommand(ServerThread server) => _server = server;

    public void Execute()
    {
        if (Thread.CurrentThread !=_server.Thread)
        {
            throw new InvalidOperationException("SoftStopCommand может быть выполнена только внутри целевого потока.");
        }
        _server.SoftStop();
    }
}