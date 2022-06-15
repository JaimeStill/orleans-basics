using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;

try
{
    var message = args.First() ?? "Good morning!";
    using var client = await ConnectClient();
    await DoClientWork(client, message);
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