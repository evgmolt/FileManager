using fileman2.Messages;
using System;

namespace fileman2.Commands
{
    internal class CmdColor : FileManagerCommand
    {
        public CmdColor(IMessager messager) : base(messager)
        {
            _commandName = "COLOR";
        }
        public override void Execute(params string[] args)
        {
            Properties.Settings1.Default.BColor = !Properties.Settings1.Default.BColor;
            FMConstants.backColor = Properties.Settings1.Default.BColor ? ConsoleColor.Black : ConsoleColor.Blue;
        }
    }
}
