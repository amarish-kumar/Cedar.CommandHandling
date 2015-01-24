﻿/*
 * Example mechanism resolving a command type from a command type name that has been 
 * parsed from the media type.
 * 
 * There are any number of ways this could be done including using relection,
 * conventions etc. In this example we will just create an explict map to lookup a
 * type from a key.
 */

namespace Cedar.HttpCommandHandling.Example.CommandVersioning.V1
{
    // 1. Version 1 of a command.
    public class Command
    {
        public string Name { get; set; }
    }
}


namespace Cedar.HttpCommandHandling.Example.CommandVersioning.V2
{
    // 2. Version 2 of a command where Name has changed to Title
    public class Command
    {
        public string Title { get; set; }
    }
}

// ReSharper disable once CheckNamespace
namespace Cedar.HttpCommandHandling.Example.CommandTypeResolution
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Cedar.HttpCommandHandling;
    using Cedar.HttpCommandHandling.TypeResolution;

    public class CommandModule : CommandHandlerModule
    {
        public CommandModule()
        {
            For<CommandVersioning.V1.Command>()
                .Handle((commandMessage, ct) => Task.FromResult(0));

            For<CommandVersioning.V2.Command>()
                .Handle((commandMessage, ct) => Task.FromResult(0));
        }
    }

    public class Program
    {
        static void Main()
        {
            var resolver = new CommandHandlerResolver(new CommandModule());

            // 1. Create a map to resolve types from a key.
            // This could of course be done with reflection + conventions.
            // For the sake of example, we're going to be explict.
            var commandMap = new[]
            {
                typeof(CommandVersioning.V1.Command),
                typeof(CommandVersioning.V2.Command),
            }.ToDictionary(t => t.Name.ToLowerInvariant(), t => t);

            var resolveCommandType = new ResolveCommandType((typeName, version) =>
            {
                // 2. In this example, we're not handling unversioned commands. You 
                // may of course handle them. 
                if(version == null)
                {
                    return null; // 3. Return null if the command type can't be resolved.
                }

                // 4. Here we're just converting the typeName to a key that matches our
                // convetion.
                var typeNameSegments = typeName.Split('.').ToList(); // my.type.name -> { my, type, name }
                typeNameSegments.Insert(typeNameSegments.Count - 1, "v" + version); // { my, type, name } -> { my, type, vX, name }
                typeName = string.Join(".", typeNameSegments); // { my, type, vX, name } -> my.type.vX.name

                Type commandType;
                commandMap.TryGetValue(typeName, out commandType);
                return commandType;
            });

            // 5. CommandHandlingSettings has a contructor overload to pass in the command type resolver.
            var settings = new CommandHandlingSettings(resolver, resolveCommandType);
        }
    }
}
