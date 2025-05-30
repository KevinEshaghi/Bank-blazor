using System.ComponentModel.DataAnnotations;

namespace BankBlazor.Client.Models
{
    public class WithdrawRequestDTO
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [Range(10.00, double.MaxValue, ErrorMessage = "Beloppet måste vara större än 0")]
        public decimal Amount { get; set; }
    }

}


