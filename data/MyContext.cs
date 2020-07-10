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
        public MyContext()
        {
        }

        public MyContext(String Conn, String server)
        {
            Sqlconn = Conn;
            Servername = server;
        }

        public MyContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Sqlconn);
                // uncomment this for Initial commit
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
        public virtual DbSet<Rule> Rules { get; set; }
    }
}