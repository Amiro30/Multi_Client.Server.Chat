using System.Data.Entity;
using Server.Models;

namespace Server.DbContext
{
    public class ChatContext : System.Data.Entity.DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }


        public ChatContext() : base("ChatContext")
        {
            Database.CreateIfNotExists();
        }

    }
}
