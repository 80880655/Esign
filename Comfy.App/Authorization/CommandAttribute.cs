using System;

namespace Comfy.App.Authorization
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandAttribute : Attribute
    {
        Command _Command;
        public CommandAttribute(Command command)
        {
            _Command = command;
        }
        public Command Command
        {
            get { return _Command; }
        }
    }
}