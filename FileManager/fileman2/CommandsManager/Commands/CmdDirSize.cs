using fileman2.Messages;
using System;
using System.IO;

namespace fileman2.Commands
{
    internal class CmdDirSize : FileManagerCommand
    {
        public CmdDirSize(IMessager messager) : base(messager)
        {
            _commandName = "DIRSIZE";
        }

        public override void Execute(params string[] args)
        {
            if (args.Length != 2)
            {
                _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
                return;
            }
            long size;
            try
            {
                size = Utils.DirSize(new DirectoryInfo(args[1]));
                _messager.ShowInfo(args[1] + " : " + FMStrings.GetSizeString(size));
            }
            catch (Exception e)
            {
                _messager.ShowAndSaveError(e.Message, true);
            }
        }
    }
}
