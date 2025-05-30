using System.ComponentModel.DataAnnotations;

namespace bankapi.Models
{
    public class DepositRequestDTO
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [Range(1.00, double.MaxValue, ErrorMessage = "Beloppet måste vara större än 0")]
        public decimal Amount { get; set; }
    }
}


