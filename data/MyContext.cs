using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 

namespace FirewallManager.data
{
    class MyContext : DbContext
    {
        private String Sqlconn;
        private String Servername;
        //Constructor sin parametros
        public MyContext()
        {
        }

        public MyContext(String Conn, String server)
        {
            Sqlconn = Conn;
            Servername = server;
        }

        //Constructor con parametros para la configuracion
        public MyContext(DbContextOptions options) : base(options)
        {
        }

        //Sobreescribimos el metodo OnConfiguring para hacer los ajustes que queramos en caso de
        //llamar al constructor sin parametros
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //En caso de que el contexto no este configurado, lo configuramos mediante la cadena de conexion
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Sqlconn);
                //optionsBuilder.UseSqlServer("Data Source = (localdb)\\ProjectsV13; Initial Catalog = firewallblocker;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rule>()
                .Property(b => b.Created)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Rule>()
                .Property(b => b.Processed)
                .HasDefaultValue(false);

            modelBuilder.Entity<Rule>()
               .Property(b => b.Server)
               .HasDefaultValue(Servername);

            modelBuilder.Entity<Rule>()
                .HasIndex(b => b.Processed);
 
            modelBuilder.Entity<Rule>()
                .HasIndex(b => b.Created);

            modelBuilder.Entity<Rule>()
                .HasIndex(b => new { b.Server, b.Ipaddr })
                .IsUnique();

            modelBuilder.Entity<Rule>()
                .HasIndex(b => b.RuleName);
        }
        //Tablas de datos
        public virtual DbSet<Rule> Rules { get; set; }
    }
}

