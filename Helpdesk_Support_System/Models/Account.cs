using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    [Table("tb_tr_accounts")]
    public class Account
    {
        [Key]
        //[ForeignKey("Customer, Employee")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        //fk role
        [ForeignKey("Role")]
        public int Role_id { get; set; }
        public Role Role { get; set; }

        public Customer Customer { get; set; }
        public Employee Employee { get; set; }

    }
}
