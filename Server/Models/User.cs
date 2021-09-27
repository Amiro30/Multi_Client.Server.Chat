using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [StringLength(64)]
        public string Login { get; set; }

        [StringLength(128)]
        public string Password { get; set; }
        public virtual List<Message> Messages { get; set; }
    }
}
