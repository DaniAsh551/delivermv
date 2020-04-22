using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseConfig(this DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var provider = configuration["DbProvider"];
            var connectionString = configuration.GetConnectionString(provider);

            var providerLibraryName = "Microsoft.EntityFrameworkCore." + provider;

            var providerPackagesSection = configuration.GetSection("ProviderPackages");

            if (providerPackagesSection != null)
            {
                try
                {
                    var maps = providerPackagesSection.Get<Dictionary<string, string>>();
                    if (maps.Keys.Any(x => x == provider))
                        providerLibraryName = maps[provider];
                }
                catch (Exception ex)
                {
                }
            }

            var assemblyName = new System.Reflection.AssemblyName(providerLibraryName);

            var assemblyPath = Assembly.GetEntryAssembly().Location;

            assemblyPath = assemblyPath.Remove(assemblyPath.LastIndexOf(Path.DirectorySeparatorChar));

            var assembly = (Assembly)null;

            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                assembly = Assembly.LoadFile(Path.Combine(assemblyPath , providerLibraryName + ".dll"));
            }

            var types = assembly.DefinedTypes
                .Where(x => x.IsClass && x.IsAbstract && x.IsSealed);

            var useProviderFunctions = types
                .SelectMany(x => x.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                .Where(x => x.Name == "Use" + provider && x.GetParameters().FirstOrDefault().ParameterType == typeof(DbContextOptionsBuilder)).ToArray();

            var minParams = useProviderFunctions.Min(x => x.GetParameters().Length);
            var useProviderFunction = useProviderFunctions.FirstOrDefault(x => x.GetParameters().Length == minParams);
            var args = useProviderFunction.GetParameters();

            if (!args.Any(x => x.ParameterType == typeof(string)))
                throw new ArgumentException();

            var argValues = new List<object>();
            var hasConnectionStringBeenAdded = false;

            argValues.Add(options);
            foreach (var arg in args.Skip(1))
            {
                if (arg.ParameterType != typeof(string) || hasConnectionStringBeenAdded)
                {
                    argValues.Add(null);
                    continue;
                }

                argValues.Add(connectionString);
                hasConnectionStringBeenAdded = true;
            }

            var result = (DbContextOptionsBuilder)useProviderFunction.Invoke(options, argValues.ToArray());
            return result;
        }
    }
}
