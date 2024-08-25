# VeloBrawl.V2.VB24BXIM9CA.Core
This is the core for VeloBrawl V2, which uses the system broker on RabbitMQ.

# To run it you need:
## Dotnet v8.0.401.
## RabbitMQ v3.13.6.
## APK: soon.

# You can use [VeloBrawl.V2.VB24BXIM9CA.Inject](https://github.com/XidMods05/VeloBrawl.V2.VB24BXIM9CA.Inject) as program inject.

You can configure your application in the VeloBrawl.V2.VB24BXIM9CA.Core/SaveBase/config.json file.

logSensitive - the level of how sensitive the console is to logging 

public enum UniqueLogLevels : byte
{
    Hypersensitive = 100,
    MediumSensitive = 80,
    AlmostSensitive = 40,
    AlmostInsensitive = 30,
    Insensitive = 20,
    FatalInsensitive = 10,
    CrazyNoneLogLevel = 1,
    LogAbsent = 0
},

rabbitMqHost - host for brokerage with RabbitMQ.

minutesToGC - number of minutes after which garbage collection will be performed. (minimum number of minutes is 1).
