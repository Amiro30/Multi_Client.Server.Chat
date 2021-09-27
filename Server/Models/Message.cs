using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [StringLength(256)]
        public string Msg { get; set; }

        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }

        public int SenderId { get; set; }

        public int RcptId { get; set; }

        public virtual User User { get; set; }
    }
}
