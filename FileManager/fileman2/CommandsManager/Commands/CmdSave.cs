using fileman2.Messages;
using System.IO;

namespace fileman2.Commands
{
    internal class CmdSave : FileManagerCommand
    {
        public CmdSave(IMessager messager) : base(messager)
        {
            _commandName = "SAVE";
        }
        public override void Execute(params string[] args)
        {
            Properties.Settings1.Default.DefaultPath = Directory.GetCurrentDirectory();
            Properties.Settings1.Default.Save();
        }
    }
}
