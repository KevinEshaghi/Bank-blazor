namespace BankBlazor.Client.Models
{
    public class TransferResponseDTO
    {
        public AccountDTO FromAccount { get; set; }
        public AccountDTO ToAccount { get; set; }
    }
}
