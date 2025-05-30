namespace BankBlazor.Client.Models
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public decimal Balance { get; set; }
        public string? AccountHolderName { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}