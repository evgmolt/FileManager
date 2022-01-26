using fileman2.Messages;
using System;

namespace fileman2.Commands
{
    internal class CmdViewMode : FileManagerCommand
    {
        public CmdViewMode(IMessager messager) : base(messager)
        {
            _commandName = "VIEW";
        }

        public override void Execute(params string[] args)
        {
            if (args.Length == 1)
            {
                Properties.Settings1.Default.ViewMode = 0;
                return;
            }
            bool err = false;
            if (Byte.TryParse(args[1], out byte res))
            {
                if (res > 0 && res < FMConstants.numOfViewMode + 1)
                {
                    Properties.Settings1.Default.ViewMode = res;
                }
                else
                {
                    err = true;
                }
            }
            else
            {
                err = true;
            }
            if (err)
            {
                _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
            }
        }
    }
}
