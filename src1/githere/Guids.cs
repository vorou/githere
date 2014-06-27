// Guids.cs
// MUST match guids.h

using System;

namespace vorou.githere
{
    internal static class GuidList
    {
        public const string guidGitherePkgString = "6daf1c44-d71d-4f2f-a7ca-c7a769593923";
        public const string guidgithereCmdSetString = "c2247e91-0611-4e4b-841c-4e9c1429f2f9";

        public static readonly Guid guidgithereCmdSet = new Guid(guidgithereCmdSetString);
    };
}