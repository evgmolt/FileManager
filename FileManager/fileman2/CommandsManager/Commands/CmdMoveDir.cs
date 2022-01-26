using fileman2.Messages;
using System;
using System.IO;

namespace fileman2.Commands
{
    internal class CmdMoveDir : FileManagerCommand
    {
        public CmdMoveDir(IMessager messager) : base(messager)
        {
            _commandName = "MOVED";
        }

        public override void Execute(params string[] args)
        {
            if (args.Length < 3)
            {
                _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
                return;
            }
            DirectoryInfo dirInfo = new DirectoryInfo(args[1]);
            if (dirInfo.Exists && Directory.Exists(args[2]) == false)
            {
                try
                {
                    dirInfo.MoveTo(args[2]);
                }
                catch (Exception e)
                {
                    _messager.ShowAndSaveError(e.Message, true);
                }
            }
            else
            {
                _messager.ShowAndSaveError(FMStrings.dirNameError, false);
            }
        }
    }
}
