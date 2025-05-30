namespace BankBlazor.Client.Models
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public DateOnly Date { get; set; }
        public string Type { get; set; }
        public string Operation { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}
