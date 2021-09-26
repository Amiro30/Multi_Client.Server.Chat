using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Msg { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }

        //[ForeignKey("UserId")]
        public int SenderId { get; set; }

        //[ForeignKey("UserId")]
        public int RcptId { get; set; }


        public int UserId { get; set; }
        public virtual User User { get; set; }

    }
}
