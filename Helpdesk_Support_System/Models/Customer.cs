using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    [Table("tb_m_customers")]
    public class Customer
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string First_name { get; set; }
        public string Last_name { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Phone_number { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
