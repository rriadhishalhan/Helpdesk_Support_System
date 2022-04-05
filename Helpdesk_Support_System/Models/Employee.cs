using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    [Table("tb_m_employees")]
    public class Employee
    {
        [Key]
        [Column(Order = 2)]
        public string Email { get; set; }
        [Required]
        public string First_name { get; set; }
        [Required]
        public string Last_name { get; set; }
        [Required]
        public int Workload { get; set; }
        [Required]
        public string Speciality { get; set; }
        [Required]
        public string Phone_number { get; set; }

        //fk position
        [ForeignKey("Position")]
        [Column(Order = 1)]
        public int Position_id { get; set; }
        public Position Position { get; set; }

        public Account Account { get; set; }
        public ICollection<TicketHistory> TicketHistories { get; set; }
    }
}
