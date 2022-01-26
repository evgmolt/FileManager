using fileman2.Messages;
using System;
using System.IO;

namespace fileman2.Commands
{
    internal class CmdMkDir : FileManagerCommand
    {
        public CmdMkDir(IMessager messager) : base(messager)
        {
            _commandName = "MD";
        }

        public override void Execute(params string[] args)
        {
            if (!Directory.Exists(args[1]))
            {
                try
                {
                    Directory.CreateDirectory(args[1]);
                }
                catch (Exception e)
                {
                    _messager.ShowAndSaveError(e.Message, true);
                    return;
                }
            }
            else
            {
                _messager.ShowAndSaveError(args[1] + FMStrings.dirExist, false);
            }
        }
    }
}
