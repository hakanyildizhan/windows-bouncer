using LiteDB;

namespace WindowsBouncer.Persistence
{
    public class LoginAttempt
    {
        [BsonId]
        public int Id { get; set; }

        [BsonField("ip")]
        public long Ip { get; set; }

        [BsonField("login")]
        public string Login { get; set; }

        [BsonField("date")]
        public DateTime Date { get; set; }
    }
}
