using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;

try
{
    var message = args.Any() ? args.First() : "Good morning!";
    var messages = GenerateAsyncMessages();

    using var client = await ConnectClient();
    await DoClientWork(client, message);

    await foreach (var m in messages)
        await DoClientWork(client, m);

    Console.ReadKey();

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"\nException while trying to run client: {ex.Message}");
    Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
    Console.WriteLine("\nPress any key to exit.");
    Console.ReadKey();

    return 1;
}

static IAsyncEnumerable<string> GenerateAsyncMessages() =>
    RangeAsync(1, 20);

static async IAsyncEnumerable<string> RangeAsync(int start, int count)
{
    for (var i = 0; i < count; i++)
        yield return await Task.FromResult($"message {start + i}");
}

static async Task<IClusterClient> ConnectClient()
{
    IClusterClient client = new ClientBuilder()
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "OrleansBasics";
        })
        .ConfigureLogging(logging => logging.AddConsole())
        .Build();

    await client.Connect();
    Console.WriteLine("Client successfully connected to silo host \n");
    return client;
}

static async Task DoClientWork(IClusterClient client, string message)
{
    var friend = client.GetGrain<IHello>(0);
    var response = await friend.SayHello(message);
    Console.WriteLine($"\n\n{response}\n\n");
}