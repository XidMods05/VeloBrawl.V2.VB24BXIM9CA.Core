using System.Collections.Concurrent;

namespace VeloBrawl.V2.VB24BXIM9CA.GameProxy.ProtectionsLayer;

public static class ProtectionStatic
{
    public static readonly ConcurrentDictionary<string, bool> BannedIpAddresses = new();
}