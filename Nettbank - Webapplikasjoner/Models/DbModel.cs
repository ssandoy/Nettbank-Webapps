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

        public DbSet<Customers> customers { get; set; }
        public DbSet<PostalNumbers> postalNumbers { get; set; }
        public DbSet<Accounts> accounts { get; set; }
        public DbSet<Transactions> transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class Customers {
        [Key]
        public string personalNumber { get; set; }
        public byte[] password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public virtual PostalNumbers postalNumber { get; set; }
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
        public virtual Customers owner { get; set; }
        public virtual List<Transactions> transactions { get; set; }
    }

    public class Transactions {
        [Key]
        public int transactionID { get; set; }
        public int amount { get; set; }
        public bool executed { get; set; }
        public DateTime timeToBeTransfered { get; set; }
        public DateTime timeTransfered { get; set; }
        public virtual Accounts fromAccount { get; set; }
        public virtual Accounts toAccount { get; set; }
    }
}