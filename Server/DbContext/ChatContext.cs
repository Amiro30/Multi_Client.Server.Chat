using System.Data.Entity;
using Server.Models;

namespace Server.DbContext
{
    public class ChatContext : System.Data.Entity.DbContext
    {
        //Name of Database
        public ChatContext() : base("ChatContext")
        {}

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
