using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp9
{
    public class CheckContext : DbContext
    {
        public CheckContext(string connectionString)
        {
            
        }
        public DbSet<CheckType> CheckTypes { get; set; }
        public DbSet<Check> Checks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"");
        }
    }

    public class CheckType
    {
        public int CheckTypeId { get; set; }
        public string Name { get; set; }

        public List<Check> Checks { get; set; }
    }

    public class Check
    {
        public int CheckId { get; set; }
        public string PullRequestName { get; set; }
        public TimeSpan TimeTaken { get; set; }

        public int CheckTypeId { get; set; }
        public CheckType CheckType { get; set; }
    }
}
