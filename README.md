# Agency

*Nobody outruns the Agency.*

**Agency** is a .NET lib for Remote Dynamic. 
It's achieved by Trinity Force: [Dynamitey](https://github.com/ekonbenefits/dynamitey) · [LinFu.Delegates](https://github.com/philiplaureano/LinFu.Delegates) · [Serialize.Linq](https://github.com/esskar/Serialize.Linq)

### Remote Dynamic? WTF?
Remote Dynamic means to share objects between processes (usually using .NET Remoting) without the type to be known (using `dynamic` instead). It will be useful when the type is complex or changes frequently.

### Demo
Server:

```
class Agent47
{
    public string Weapon { get; set; } = "Fiber Wire";

    public void PointShooting(int enemy)
    {
        Console.WriteLine($"Target eliminated: {enemy}");
    }
}
```

```
static void Main(string[] args)
{
    var agent = new Agent47();
    Agency.RegisterAgent("47", agent, new IpcHandler());
    Console.ReadLine();
	//Run Client here
    Console.WriteLine($"Current Weapon: {agent.Weapon}"); //weapon changed to `Silverballer`
    Console.ReadLine();
}
```

Client:

```
static void Main(string[] args)
{
    dynamic agent;
    try
    {
        agent = Agency.SpawnAgent("47", new IpcHandler());
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return;
    }
    Console.WriteLine($"Current Weapon: {agent.Weapon}"); //get
    var weapon = "Silverballer";
    agent.Weapon = weapon; //set
    for (int i = 0; i < 5; i++)
    {
        agent.PointShooting(i); //Called on server
    }
    Console.ReadLine();
}
```

### Handler
`Handler` implements the communication way between processes. Currently Agency has implemented `IpcHandler` which uses .NET Remoting IPC. You can implement your handler as well.

### Events
[Example](https://github.com/UlyssesWu/Agency/blob/master/Agency.Test.Client/Program.cs)
You can subscribe a server object's event using a client method. When event is triggered on server, the method executes on client. (The subscription can be slow!)
Even more, you can also subscribe a server object's event using `Expression<T>` and it will be compiled and executed on server!

---
by Ulysses (wdwxy12345@gmail.com)