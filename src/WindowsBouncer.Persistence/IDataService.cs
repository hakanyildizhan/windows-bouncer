namespace WindowsBouncer.Persistence
{
    public interface IDataService
    {
        LoginAttempt GetLoginAttempt(long key);
        DbOperationResult AddLoginAttempt(LoginAttempt entry);
        bool DeleteLoginAttempt(long key);
        void Close();
    }
}
