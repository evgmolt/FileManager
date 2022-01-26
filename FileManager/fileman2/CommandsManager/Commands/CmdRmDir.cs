using fileman2.Messages;
using System;
using System.IO;

namespace fileman2.Commands
{
    internal class CmdRmDir : FileManagerCommand
    {
        public CmdRmDir(IMessager messager) : base(messager)
        {
            _commandName = "RMD";
        }

        public override void Execute(params string[] args)
        {
            if (args.Length != 2)
            {
                _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
                return;
            }
            try
            {
                Directory.Delete(args[1], true);
            }
            catch (Exception e)
            {
                _messager.ShowAndSaveError(e.Message, true);
            }
        }
    }
}
