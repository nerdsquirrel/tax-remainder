using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tax_Reminder.Models
{
    public class TaxInformationModel
    {
        public int Id { get; set; }

        [Required]
        public string VehicleRegistration { get; set; }

        [Required]
        public string VehicleMake { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ReferralCode { get; set; }

        [Required]
        [CreditCard]
        [DisplayName("Card Number")]
        public string CardNumber { get; set; }

        [Required]
        public string CvcSecurityCode { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Expiry { get; set; }

        public bool IsAgree { get; set; }
    }
}