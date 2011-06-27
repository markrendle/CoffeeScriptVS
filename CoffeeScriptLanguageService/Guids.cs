// Guids.cs
// MUST match guids.h
using System;

namespace MarkRendle.CoffeeScriptLanguageService
{
    static class GuidList
    {
        public const string guidCoffeeScriptLanguageServicePkgString = "f1f90246-dd41-41e5-81f3-bec61ca9c348";
        public const string guidCoffeeScriptLanguageServiceCmdSetString = "f75f0949-3dfc-44c8-9a1f-375122547632";

        public static readonly Guid guidCoffeeScriptLanguageServiceCmdSet = new Guid(guidCoffeeScriptLanguageServiceCmdSetString);
    };
}