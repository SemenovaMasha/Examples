using System;
using System.Collections.Generic;
using System.Linq;

namespace ArgsParser
{
    public class CommandLineParser
    {
        private Dictionary<char, ArgumentMarshaler> marshalers = new Dictionary<char, ArgumentMarshaler>();
        private IEnumerator<string> currentArgument;

        public CommandLineParser(string scheme, string[] args)
        {
            ParseScheme(scheme);
            ParseArguments(args);
        }

        private void ParseScheme(string scheme)
        {
            foreach(string argDefinition in scheme.Split(','))
            {
                if(argDefinition.Length == 1)
                {
                    marshalers[argDefinition[0]] = new BoolArgumentMarshaler();
                }
                else
                {
                    switch (argDefinition.Substring(1))
                    {
                        case "*":
                            marshalers[argDefinition[0]] = new StringArgumentMarshaler();
                            break;
                        case "#":
                            marshalers[argDefinition[0]] = new IntArgumentMarshaler();
                            break;
                        case "##":
                            marshalers[argDefinition[0]] = new DoubleArgumentMarshaler();
                            break;
                    }
                }
            }
        }

        private void ParseArguments(string[] args)
        {
            for (currentArgument = args.AsEnumerable().GetEnumerator(); currentArgument.MoveNext();)
            {
                string arg = currentArgument.Current;
                ParseArgument(arg);
            }
        }

        private void ParseArgument(string arg)
        {
            if (arg.StartsWith("-"))
                SetArgument(arg[1]);
        }

        private void SetArgument(char elementId)
        {
            ArgumentMarshaler m = marshalers[elementId];
            if(currentArgument.MoveNext())
                m.Set(currentArgument.Current);
            else
                m.Set("");
        }

        public bool GetBool(char elementId)
        {
            ArgumentMarshaler m = marshalers[elementId];
            return (bool)m.Get();
        }

        public string GetString(char elementId)
        {
            ArgumentMarshaler m = marshalers[elementId];
            return (string)m.Get();
        }

        public int GetInt(char elementId)
        {
            ArgumentMarshaler m = marshalers[elementId];
            return (int)m.Get();
        }

        public double GetDouble(char elementId)
        {
            ArgumentMarshaler m = marshalers[elementId];
            return (double)m.Get();
        }
    }

    interface ArgumentMarshaler {
        Object Get();
        void Set(string arg);
    }

    class BoolArgumentMarshaler : ArgumentMarshaler
    {
        private bool Value;

        public object Get()
        {
            return Value;
        }

        public void Set(string arg)
        {
            Value = true;
        }
    }

    class StringArgumentMarshaler : ArgumentMarshaler
    {
        private string Value;

        public object Get()
        {
            return Value;
        }

        public void Set(string arg)
        {
            Value = arg;
        }
    }

    class IntArgumentMarshaler : ArgumentMarshaler
    {
        private int Value;

        public object Get()
        {
            return Value;
        }

        public void Set(string arg)
        {
            Value = int.Parse(arg);
        }
    }
    class DoubleArgumentMarshaler : ArgumentMarshaler
    {
        private Double Value;

        public object Get()
        {
            return Value;
        }

        public void Set(string arg)
        {
            Value = Double.Parse(arg.Replace('.',','));
        }
    }
}