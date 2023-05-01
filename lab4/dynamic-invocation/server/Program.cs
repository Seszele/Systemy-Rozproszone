using Demo;
using Ice;
using System;

public class OperationsImpl : OperationsDisp_
{
    public override void simpleOperation(Current current) => Console.WriteLine("Simple operation called.");

    public override string operationWithArgs(string arg1, int arg2, Current current) => $"Received: {arg1} and {arg2}";

    public override byte[] processData(byte[] data, Current current) => data;
}

public static class Program
{
    public static int Main(string[] args)
    {
        Console.WriteLine($"Running {nameof(Program)}");

        try
        {
            using var communicator = Ice.Util.initialize(ref args);
            var adapter = communicator.createObjectAdapterWithEndpoints("OperationsAdapter", "default -p 10000");
            var servant = new OperationsImpl();
            adapter.add(servant, Ice.Util.stringToIdentity("Operations"));
            adapter.activate();
            communicator.waitForShutdown();
        }
        catch (Ice.Exception e)
        {
            Console.Error.WriteLine(e);
            return 1;
        }
        catch (System.Exception e)
        {
            Console.Error.WriteLine(e);
            return 1;
        }
        return 0;
    }
}
