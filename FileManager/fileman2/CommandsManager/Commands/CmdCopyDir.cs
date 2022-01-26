using fileman2.Messages;

namespace fileman2.Commands
{
    internal class CmdCopyDir : FileManagerCommand
    {
        public CmdCopyDir(IMessager messager) : base(messager)
        {
            _commandName = "COPYD";
        }

        public override void Execute(params string[] args)
        {
            bool recurse = args[1].ToUpper() == FMStrings.keyRecurs;
            if (recurse)
            {
                if (args.Length == 4)
                {
                    Utils.DirectoryCopy(args[2], args[3], recurse, _messager); //есть параметр -r
                }
                else
                {
                    _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
                    return;
                }
            }
            else
            {
                if (args.Length == 3)
                {
                    Utils.DirectoryCopy(args[1], args[2], false, _messager);// нет параметра
                }
                else
                {
                    _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
                }
            }
        }
    }
}
