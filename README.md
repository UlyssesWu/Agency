# Agency

**Agency** is a .NET lib for Remote Dynamic, based on [dynamitey](https://github.com/ekonbenefits/dynamitey).

### Remote Dynamic? WTF?
Remote Dynamic means to share a object between processes (usually using .NET Remoting) without the type to be known (using `dynamic` instead). It will be useful when the type is complex or changes frequently.

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
    var weapon = "Silverballer";
    agent.Weapon = weapon; //set
    for (int i = 0; i < 5; i++)
    {
        agent.PointShooting(i); //Called on server
    }
    Console.WriteLine(agent.Name + ": " + agent.IsDianaDead()); //get
    Console.ReadLine();
}
```

### Handler
`Handler` implement the communication way between processes. Currently Agency has implemented `IpcHandler` which uses .NET Remoting IPC. You can implement your handler as well.

---
by Ulysses (wdwxy12345@gmail.com)