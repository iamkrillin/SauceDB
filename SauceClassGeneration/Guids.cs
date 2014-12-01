// Guids.cs
// MUST match guids.h
using System;

namespace TheCodeHaven.SauceClassGeneration
{
    static class GuidList
    {
        public const string guidSauceClassGenerationPkgString = "2560de1c-b91e-4fd9-b6c6-9bc9e7208428";
        public const string guidSauceClassGenerationCmdSetString = "1c26b504-511a-4bd3-864c-83829fe74058";
        public const string guidToolWindowPersistanceString = "1ca5281a-5250-40da-903a-270344f549bb";

        public static readonly Guid guidSauceClassGenerationCmdSet = new Guid(guidSauceClassGenerationCmdSetString);
    };
}