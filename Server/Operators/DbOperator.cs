using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.DbContext;
using Server.Models;

namespace Server.Operators
{
    public class DbOperator
    {
        ChatContext _db;

        public DbOperator()
        {
            _db = new ChatContext();
        }

        public void SaveUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void SaveMessage(Message msg)
        {
            _db.Messages.Add(msg);
            _db.SaveChanges();
        }
    }
}
