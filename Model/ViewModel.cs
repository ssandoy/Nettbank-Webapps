using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model {
    // For use in Index
    public class Customer {
        [Required(ErrorMessage = "Personnummer må oppgis.")]
        [RegularExpression(@"[0-9]{11}", ErrorMessage = "Personnummeret må være på 11 siffer.")]
        public string PersonalNumber { get; set; }

        [Required(ErrorMessage = "Passord må oppgis.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Koden fra din kodebrikke må oppgis.")]
        [RegularExpression(@"[0-9]{6}", ErrorMessage = "Koden må være på 6 siffer.")]
        public string BankId { get; set; }
        
    }

    // For use in AccountInfo
    public class CustomerInfo {
        public string PersonalNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
    }

    // For use in ListAccounts
    public class Account {
        [DisplayName("Kontonummer")]
        public string AccountNumber { get; set; }

        [DisplayName("Eier")]
        public string OwnerName { get; set; }

        [DisplayName("Saldo")]
        public int Balance { get; set; }
    }

    // For use in RegisterTransaction and ListTransactions
    public class Transaction {
        [DisplayName("Betalings-Id")]
        public int TransactionId { get; set; }

        [DisplayName("Beløp")]
        [Required(ErrorMessage = "Beløpet må oppgis.")]
        [RegularExpression(@"[0-9]+", ErrorMessage = "Beløpet må være et positivt tall.")]
        public int Amount { get; set; }

        [DisplayName("Utførelsesdato")]
        [DataType(DataType.Date)]
        public DateTime? TimeToBeTransfered { get; set; }

        [DisplayName("Dato")]
        public DateTime? TimeTransfered { get; set; }

        [DisplayName("Fra konto")]
        public string FromAccountNumber { get; set; }

        [DisplayName("Til konto")]
        [Required(ErrorMessage = "Kontonummer å betale til må oppgis.")]
        [RegularExpression(@"[0-9]{11}", ErrorMessage = "Kontonummeret må være på 11 siffer.")]
        public string ToAccountNumber { get; set; }

        [DisplayName("Kommentar")]
        [StringLength(30, ErrorMessage = "Kommentaren kan være på maksimalt 30 tegn.")]
        public string Comment { get; set; }
    }

    public class Admin {
        [Required(ErrorMessage = "Personnummer må oppgis.")]
        [RegularExpression(@"[0-9]{11}", ErrorMessage = "Ansattnummeret må være på X siffer.")]
        public string PersonalNumber { get; set; }

        [Required(ErrorMessage = "Passord må oppgis.")]
        public string Password { get; set; }
    }
}