namespace bankapi.Models
{
    public class TransferResponse
    {
        public AccountDTO FromAccount { get; set; }
        public AccountDTO ToAccount { get; set; }
    }
}
