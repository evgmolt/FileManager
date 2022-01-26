using fileman2.Messages;

namespace fileman2.Commands
{
    internal class CmdCopyFile : FileManagerCommand
    {
        public CmdCopyFile(IMessager messager) : base(messager)
        {
            _commandName = "COPY";
        }

        public override void Execute(params string[] args)
        {
            if (args.Length < 3)
            {
                _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
                return;
            }
            Utils.FileCopyOrMove(false, args[1], args[2], _messager);
        }
    }
}
