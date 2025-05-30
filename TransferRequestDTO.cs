using System.ComponentModel.DataAnnotations;

namespace BankBlazor.Client.Models
{
    public class TransferRequestDTO
    {
        [Required]
        public int FromAccountId { get; set; }

        [Required]
        public int ToAccountId { get; set; }

        [Required]
        [Range(10.00, double.MaxValue, ErrorMessage = "Beloppet måste vara större än 0")]
        public decimal Amount { get; set; }
    }
}
