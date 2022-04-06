using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    [Table("tb_tr_ticketHistory")]
    public class TicketHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public DateTime Start_date { get; set; }
        public DateTime End_date { get; set; }

        //fk ticket
        [ForeignKey("Ticket")]
        public string Ticket_Id { get; set; }
        public Ticket Ticket { get; set; }

        //fk employees position
        //public int Employee_position { get; set; }
        
        //fk employees email
        [ForeignKey("Employee")]
        public string Employee_Id { get; set; }
        public Employee Employee { get; set; }
    }

    public enum Status {
        Terkirim,
        Dibuka,
        Diteruskan,
        Terjawab,
        Ditutup
    }

}
