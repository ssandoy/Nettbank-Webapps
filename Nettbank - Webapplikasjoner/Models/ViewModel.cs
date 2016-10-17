using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nettbank___Webapplikasjoner.Models {
    // For use in Index
    public class Customer {
        [Required(ErrorMessage = "Personnummer må oppgis.")]
        [RegularExpression(@"[0-9]{11}", ErrorMessage = "Personnummeret må være på 11 siffer.")]
        public string personalNumber { get; set; }

        [Required(ErrorMessage = "Passord må oppgis.")]
        public string password { get; set; }

        [Required(ErrorMessage = "Koden fra din kodebrikke må oppgis.")]
        [RegularExpression(@"[0-9]{6}", ErrorMessage = "Koden må være på 6 siffer.")]
        public string bankId { get; set; }
    }

    // For use in ListAccounts
    public class Account {
        [DisplayName("Kontonummer")]
        public string accountNumber { get; set; }

        [DisplayName("Saldo")]
        public int balance { get; set; }
    }

    // For use in RegisterTransaction and ListTransactions
    public class Transaction {
        [DisplayName("Beløp")]
        [Required(ErrorMessage = "Beløpet må oppgis.")]
        [RegularExpression(@"[0-9]+", ErrorMessage = "Beløpet må være et positivt tall.")]
        public int amount { get; set; }

        public bool executed { get; set; }

        [DisplayName("Utførelsesdato")]
        public DateTime timeToBeTransfered { get; set; }

        [DisplayName("Dato utført")]
        public DateTime timeTransfered { get; set; }

        [DisplayName("Fra konto")]
        public string fromAccountNumber { get; set; }

        [DisplayName("Til konto")]
        [Required(ErrorMessage = "Kontonummer å betale til må oppgis.")]
        [RegularExpression(@"[0-9]{11}", ErrorMessage = "Kontonummer å betale til må oppgis.")]
        public string toAccountNumber { get; set; }
    }
}