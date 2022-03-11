using LiteDB;
using System.Reflection;

namespace WindowsBouncer.Persistence
{
    public class LiteDBDataService : IDataService
    {
        private LiteDatabase _db;
        private ILiteCollection<LoginAttempt> _loginattempts;

        public LiteDBDataService()
        {
            _db = new LiteDatabase(Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "datastore.db"));
            _loginattempts = _db.GetCollection<LoginAttempt>("loginattempts");
            _loginattempts.EnsureIndex(e => e.Ip);
        }

        public DbOperationResult AddLoginAttempt(LoginAttempt entry)
        {
            try
            {
                if (GetLoginAttempt(entry.Ip) != null)
                {
                    return DbOperationResult.AlreadyExists;
                }
                _loginattempts.Insert(entry);
                return DbOperationResult.Success;
            }
            catch
            {
                return DbOperationResult.Failure;
            }
        }

        public bool DeleteLoginAttempt(long key)
        {
            var existingEntry = GetLoginAttempt(key);
            return existingEntry != null ? _loginattempts.Delete(existingEntry.Id) : false;
        }

        public LoginAttempt GetLoginAttempt(long key)
        {
            return _loginattempts.FindOne(e => e.Ip.Equals(key));
        }

        public void Close()
        {
            _db.Dispose();
        }
    }
}
