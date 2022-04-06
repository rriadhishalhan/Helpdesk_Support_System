using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    [Table("tb_m_tickets")]
    public class Ticket
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Issue { get; set; }
        public string Solution { get; set; }
        public string Feedback { get; set; }

        //fk customer
        [ForeignKey("Customer")]
        public string Customer_Id { get; set; }
        public Customer Customer { get; set; }
        //fk category
        [ForeignKey("Category")]
        public int Category_Id { get; set; }
        public Category Category { get; set; }
        //fk priority
        [ForeignKey("Priority")]
        public int Priority_Id { get; set; }
        public Priority Priority { get; set; }

        public ICollection<TicketHistory> TicketHistories { get; set; }

    }
}
