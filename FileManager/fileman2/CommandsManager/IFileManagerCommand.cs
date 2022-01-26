namespace fileman2.Commands
{
    interface IFileManagerCommand
    {
        string CommandName { get; }
        void Execute(params string[] args);
    }
}
