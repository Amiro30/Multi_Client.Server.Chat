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
            //Database.SetInitializer<ChatContext>(new DropCreateDatabaseAlways<ChatContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.RcptId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.SenderId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

       
    }
}
