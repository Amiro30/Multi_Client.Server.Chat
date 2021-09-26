using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Msg { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }

        [ForeignKey("User")]
        public int SenderId { get; set; }

        [ForeignKey("User")]
        public int RcptId { get; set; }


        public int UserId { get; set; }
        public virtual User User { get; set; }

    }
}
