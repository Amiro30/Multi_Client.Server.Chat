using System;
using System.Linq;
using System.Threading.Tasks;
using Server.DbContext;
using Server.Models;

namespace Server.Operators
{
    public class ChatRepository : IDisposable
    {
        ChatContext _db;

        public ChatRepository()
        {
            _db = new ChatContext();
        }

        public void SaveUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void  SaveMessage(Message msg)
        {
            _db.Messages.Add(msg);
            _db.SaveChanges();
        }

        public User GetUserId(string name)
        {
            return _db.Users.FirstOrDefault(x => x.Login == name);
        }

        public bool UserExist(string name)
        {
            return _db.Users.Any(u => u.Login == name);
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool IsDisposed()
        {
            return disposed;
        }
    }
}
