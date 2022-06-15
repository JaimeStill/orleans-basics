using GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace Grains;
public class HelloGrain : Orleans.Grain, IHello
{
    private readonly ILogger logger;

    public HelloGrain(ILogger<HelloGrain> logger)
    {
        this.logger = logger;
    }

    Task<string> IHello.SayHello(string greeting)
    {
        logger.LogInformation($"\nSayHello message received: greeting = '{greeting}'");
        return Task.FromResult($"\n Client said: '{greeting}', so HelloGrain says: Hello!");
    }
}
