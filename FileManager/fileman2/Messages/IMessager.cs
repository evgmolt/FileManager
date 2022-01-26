namespace fileman2.Messages
{
    public interface IMessager
    {
        void ShowInfo(string mess);
        void ShowHelp();
        void ShowAndSaveError(string mess, bool save);
    }
}
