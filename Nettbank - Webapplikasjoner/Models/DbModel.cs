namespace Nettbank___Webapplikasjoner.Models {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class DbModel : DbContext {
        public DbModel()
            : base("name=DbModel") {
            Database.CreateIfNotExists();
        }
        public DbSet<Admins> admins { get; set; }
        public DbSet<Customers> customers { get; set; }
        public DbSet<PostalNumbers> postalNumbers { get; set; }
        public DbSet<Accounts> accounts { get; set; }
        public DbSet<Transactions> transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class Admins
    {
        [Key]
        public string employeeNumber { get; set; }
        public byte[] password { get; set; }
        public string salt { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string postalNumber { get; set; }
        public virtual PostalNumbers postalNumbers { get; set; }

    }

    public class Customers {
        [Key]
        public string personalNumber { get; set; }
        public byte[] password { get; set; }
        public string salt { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string postalNumber { get; set; }
        public virtual PostalNumbers postalNumbers { get; set; }
    }

    public class PostalNumbers {
        [Key]
        public string postalNumber { get; set; }
        public string postalCity { get; set; }
    }

    public class Accounts {
        [Key]
        public string accountNumber { get; set; }
        public int balance { get; set; }
        public string personalNumber { get; set; }
        public virtual Customers owner { get; set; }
        public virtual List<Transactions> transactions { get; set; }
    }

    public class Transactions {
        [Key]
        public int transactionID { get; set; }
        public int amount { get; set; }
        public DateTime? timeToBeTransfered { get; set; }
        public DateTime? timeTransfered { get; set; }
        public string accountNumber { get; set; }
        [ForeignKey("accountNumber")]
        public virtual Accounts account { get; set; }
        public string toAccountNumber { get; set; }
        public string comment { get; set; }
    }
}