using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Nevermind.Compiler;

namespace NevermindCompiler.CLParser
{
    public class ArgumentParser<T>
    {
        private  List<Flag> Flags;
        private List<Value> Values;
        private InlineValue InlineValue;

        public bool HasErrors;

        public bool AutoHelp = false;
        public bool AutoVersion = false;

        public string Description;
        public string BinaryName;

        public string Prefix;
        public string FullPrefix;

        public string HelpCommand;
        public string VersionCommand;

        public ArgumentParser()
        {
            var type = typeof(T);
            var fields = type.GetFields();

            Flags = fields.Where(prop => prop.IsDefined(typeof(FlagAttribute), false))
                    .Select(p => new Flag(p, (FlagAttribute)p.GetCustomAttributes(typeof(FlagAttribute), false)[0]))
                    .ToList();

            Values = fields.Where(prop => prop.IsDefined(typeof(ValueAttribute), false))
                .Select(p => new Value(p, (ValueAttribute)p.GetCustomAttributes(typeof(ValueAttribute), false)[0]))
                .ToList();

            InlineValue = fields.Where(prop => prop.IsDefined(typeof(InlineValueAttribute), false))
                .Select(p => new InlineValue(p, (InlineValueAttribute)p.GetCustomAttributes(typeof(InlineValueAttribute), false)[0]))
                .ToList()[0];
        }

        public ArgumentParser<T> SetParameters(Action<ArgumentParser<T>> func)
        {
            func(this);
            return this;
        }

        void PrintHelp()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine("{0} {1}", versionInfo.ProductName, versionInfo.FileVersion);
            Console.WriteLine("{0}", versionInfo.CompanyName);

            Console.WriteLine("{0}", Description);
            Console.WriteLine("Usage: {0} {1} [{4}{2}] [{3}]",
                BinaryName, InlineValue.Attribute.Name,
                string.Join("", Flags.Select(p => p.Attribute.ShortName)),
                string.Join(" ", Values.Select(p => Prefix + p.Attribute.ShortName + " <value>")),
                Prefix);

            Console.WriteLine();

            Console.WriteLine("Available flags: ");
            foreach (var flag in Flags)
            {
                if (flag.Attribute.ShortName == Flag.NoName)
                {
                    Console.WriteLine("          {2}{0,-22}{1}.",
                        flag.Attribute.Name,
                        flag.Attribute.Description,
                        FullPrefix);
                }
                else
                {
                    Console.WriteLine("   {3}{0,-4}  {4}{1,-22}{2}.",
                        flag.Attribute.ShortName + ",",
                        flag.Attribute.Name,
                        flag.Attribute.Description,
                        Prefix,
                        FullPrefix);
                }
            }

            Console.WriteLine("          {2}{0,-22}{1}.",
                HelpCommand,
                "Outputs list of all commands",
                FullPrefix);
            Console.WriteLine("          {2}{0,-22}{1}.",
                VersionCommand,
                "Outputs program version",
                FullPrefix);

            Console.WriteLine("\nAvailable options: ");
            foreach (var value in Values)
            {
                if (value.Attribute.ShortName == Value.NoName)
                {
                    Console.WriteLine("          {3}{0,-22}{1}{2}.",
                        value.Attribute.Name,
                        value.Attribute.Required ? "(required) " : "",
                        value.Attribute.Description,
                        FullPrefix);
                }
                else
                {
                    Console.WriteLine("   {4}{0,-4}  {5}{1,-22}{2}{3}.",
                        value.Attribute.ShortName + ",",
                        value.Attribute.Name,
                        value.Attribute.Required ? "(required) " : "",
                        value.Attribute.Description,
                        Prefix,
                        FullPrefix);
                }
            }
        }

        void PrintVersion()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("{0} {1}", versionInfo.ProductName, versionInfo.FileVersion);
        }

        public ArgumentParser<T> ParseArguments(string[] arg)
        {
            var iterator = new TokenIterator<string>(arg.ToList());
            while (iterator.GetNext() != null)
            {
                if (iterator.Current.StartsWith(Prefix) && !iterator.Current.StartsWith(FullPrefix))
                {
                    if (iterator.Current.Length != Prefix.Length + 1)
                    {
                        //Multiple flags. Couldn't be a value

                        foreach (var c in iterator.Current.Skip(Prefix.Length))
                        {
                            var f = Flags.Find(p => p.Attribute.ShortName == c);
                            if (f == null)
                            {
                                Console.WriteLine("Unknown flag \"{0}\"", c);
                                HasErrors = true;
                                return this;
                            }
                            f.Exists = true;
                        }
                    }
                    else
                    {
                        //Only one flag. Could be a value
                        var f = Flags.Find(p => p.Attribute.ShortName == iterator.Current[Prefix.Length]);
                        if (f != null)
                        {
                            f.Exists = true;
                            continue;
                        }

                        var v = Values.Find(p => p.Attribute.ShortName == iterator.Current[Prefix.Length]);
                        if(v == null)
                        {
                            Console.WriteLine("Unknown flag \"{0}\"", iterator.Current[Prefix.Length]);
                            HasErrors = true;
                            return this;
                        }

                        var next = iterator.GetNext();
                        if (next == null || next.StartsWith(Prefix) || next.StartsWith(FullPrefix))
                        {
                            if(next == null)
                                Console.WriteLine("Value expected, but end of argument list found");
                            else
                                Console.WriteLine("Value expected, but \"{0}\" found", iterator.Current);
                            HasErrors = true;
                            return this;
                        }

                        if (v.StringValue != null)
                        {
                            Console.WriteLine("Multiple definition of \"{0}\" value", v.Attribute.Name);
                            HasErrors = true;
                            return this;
                        }
                        v.StringValue = next;
                    }
                }
                else if (iterator.Current.StartsWith(FullPrefix))
                {
                    var name = iterator.Current.Substring(FullPrefix.Length,
                        iterator.Current.Length - FullPrefix.Length);

                    var f = Flags.Find(p => p.Attribute.Name == name);
                    if (f != null)
                    {
                        f.Exists = true;
                        continue;
                    }

                    var v = Values.Find(p => p.Attribute.Name == name);
                    if(v == null)
                    {
                        if (AutoHelp && name == HelpCommand)
                        {
                            PrintHelp();
                            HasErrors = true;
                            return this;
                        }

                        if(AutoVersion && name == VersionCommand)
                        {
                            PrintVersion();
                            HasErrors = true;
                            return this;
                        }

                        Console.WriteLine("Unknown flag \"{0}\"", name);
                        HasErrors = true;
                        return this;
                    }

                    var next = iterator.GetNext();
                    if (next == null || next.StartsWith(Prefix) || next.StartsWith(FullPrefix))
                    {
                        if(next == null)
                            Console.WriteLine("Value expected, but end of argument list found");
                        else
                            Console.WriteLine("Value expected, but \"{0}\" found", iterator.Current);
                        HasErrors = true;
                        return this;
                    }

                    if (v.StringValue != null)
                    {
                        Console.WriteLine("Multiple definition of \"{0}\" value", v.Attribute.Name);
                        HasErrors = true;
                        return this;
                    }
                    v.StringValue = next;
                }
                else
                {
                    //Just value
                    InlineValue.StringValue = iterator.Current;
                }
            }
            return this;
        }

        public ArgumentParser<T> Run(Action<T, bool> action)
        {
            if (HasErrors)
            {
                action(default(T), true);
                return this;
            }

            T instance = Activator.CreateInstance<T>();
            foreach (var flag in Flags)
                flag.ClassField.SetValue(instance, flag.Exists);

            foreach (var value in Values)
            {
                if (value.Attribute.Required && value.StringValue == null)
                {
                    Console.WriteLine("Required field \"{0}\" is not set", value.Attribute.Name);
                    HasErrors = true;
                    action(default(T), true);
                    return this;
                }

                if (value.StringValue != null)
                {
                    if (value.Attribute.CheckFileExistence)
                    {
                        if (!File.Exists(value.StringValue))
                        {
                            Console.WriteLine("File \"{0}\" specified in parameter \"{1}\" doesn't exist", value.StringValue,
                                value.Attribute.Name);
                            HasErrors = true;
                            action(default(T), true);
                            return this;
                        }
                    }
                    value.ClassField.SetValue(instance, value.StringValue);
                }

                if (InlineValue.StringValue != null)
                {
                    if (!File.Exists(InlineValue.StringValue))
                    {
                        Console.WriteLine("File \"{0}\" specified in parameter \"{1}\" doesn't exist", InlineValue.StringValue,
                            InlineValue.Attribute.Name);
                        HasErrors = true;
                        action(default(T), true);
                        return this;
                    }
                    InlineValue.ClassField.SetValue(instance, InlineValue.StringValue);
                }
            }

            action(instance, false);
            return this;
        }
    }
}