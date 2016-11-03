using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DAL {
    public class DbModel : DbContext {
        public DbModel()
            : base("name=DbModel") {
            Database.CreateIfNotExists();
        }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<PostalNumbers> PostalNumbers { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class Customers {
        [Key]
        public string PersonalNumber { get; set; }
        public byte[] Password { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalNumber { get; set; }
        public virtual PostalNumbers PostalNumbers { get; set; }
    }

    public class PostalNumbers {
        [Key]
        public string PostalNumber { get; set; }
        public string PostalCity { get; set; }
    }

    public class Accounts {
        [Key]
        public string AccountNumber { get; set; }
        public int Balance { get; set; }
        public string PersonalNumber { get; set; }
        public virtual Customers Owner { get; set; }
        public virtual List<Transactions> Transactions { get; set; }
    }

    public class Transactions {
        [Key]
        public int TransactionId { get; set; }
        public int Amount { get; set; }
        public DateTime? TimeToBeTransfered { get; set; }
        public DateTime? TimeTransfered { get; set; }
        public string AccountNumber { get; set; }
        [ForeignKey("AccountNumber")]
        public virtual Accounts Account { get; set; }
        public string ToAccountNumber { get; set; }
        public string Comment { get; set; }
    }

    public class Admins { //TODO: Fiks
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
}