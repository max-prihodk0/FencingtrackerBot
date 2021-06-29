using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore;

namespace FencingtrackerBot.References.SQL
{
    public class ServerContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DbSet<Verification> Verification { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options)
            => Options.UseMySQL(SQL.Connection);    
    }

    public class Member
    { 
        [Key]
        public long UserId { get; set; }
        public int Warnings { get; set; }
        public int MessagesSent { get; set; }
        public Verification Verification { get; set; }
    }

    public class Verification
    { 
        [Key]
        public int Id { get; set; }
        public string Captcha { get; set; }
        public int Tries { get; set; }
    }
}
