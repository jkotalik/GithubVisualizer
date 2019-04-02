using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp9
{
    public class CheckContext : DbContext
    {
        public CheckContext()
        {
        }

        public DbSet<CheckType> CheckTypes { get; set; }
        public DbSet<Check> Checks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddUserSecrets<AdminInformation>();
            var root = configBuilder.Build();
            var connectionString = root["ConnectionString"];
            optionsBuilder.UseSqlServer(connectionString);
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
        public string SHA { get; set; }
        public double TimeTaken { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset Finished { get; set; }
        public int CheckTypeId { get; set; }
        public CheckType CheckType { get; set; }
    }
}
