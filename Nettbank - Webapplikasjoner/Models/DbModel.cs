namespace Nettbank___Webapplikasjoner.Models {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class DbModel : DbContext {
        public DbModel()
            : base("name=DbModel") {
            Database.CreateIfNotExists();
        }

        public DbSet<Customer> customers { get; set; }
        public DbSet<PostalNumber> postalNumbers { get; set; }
        public DbSet<Account> accounts { get; set; }
        public DbSet<Transaction> transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class Customer {
        [Key]
        public string personalNumber { get; set; }
        public byte[] password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public virtual PostalNumber postalNumber { get; set; }
    }

    public class PostalNumber {
        [Key]
        public string postalNumber { get; set; }
        public string postalCity { get; set; }
    }

    public class Account {
        [Key]
        public string accountNumber { get; set; }
        public int balance { get; set; }
        public virtual Customer owner { get; set; }
        public virtual List<Transaction> transactions { get; set; }
    }

    public class Transaction {
        [Key]
        public int transactionID { get; set; }
        public int amount { get; set; }
        public bool executed { get; set; }
        public DateTime timeToBeTransfered { get; set; }
        public DateTime timeTransfered { get; set; }
        public virtual Account fromAccount { get; set; }
        public virtual Account toAccount { get; set; }
    }
}