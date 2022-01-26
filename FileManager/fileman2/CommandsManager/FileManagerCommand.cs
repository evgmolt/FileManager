using fileman2.Messages;

namespace fileman2.Commands
{
    abstract class FileManagerCommand : IFileManagerCommand
    {
        protected readonly IMessager _messager;
        protected string _commandName;

        public string CommandName { get => _commandName; }

        public FileManagerCommand(IMessager messager)
        {
            _messager = messager;
        }
        public abstract void Execute(params string[] args);
    }
}
