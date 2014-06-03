﻿using System;
using System.Linq;

namespace TestUtils
{
    public static class TypeExtensions
    {
        public static bool AssemblyContainsReferencesTo(this Type typeToCheck, string unwantedAssemblyName)
        {
            var references = typeToCheck.Assembly.GetReferencedAssemblies();

            return references.Any(x => x.Name == unwantedAssemblyName);
        }
    }
}