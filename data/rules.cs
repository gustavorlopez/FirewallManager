using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;
    

namespace FirewallManager.data
{
    public class Rule
    {
        [Key]
        public int Id { get; set; } //Clave primaria
        public DateTime Created { get; set; }
        [MaxLength(50)]
        [Required]
        public string Server { get; set; }
        [MaxLength(500)]
        [Required]
        public string Ipaddr { get; set; }
        [MaxLength(500)]
        [Required]
        public string RuleName { get; set; }
        public Boolean Processed { get; set; }

        //Entity Framework Core
        public DbSet<Rule> Rules{ get; set; } //Objeto de navegación virtual EFC
    }
}
