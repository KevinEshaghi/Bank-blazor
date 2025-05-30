namespace BankBlazor.Client.Models
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string Givenname { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public List<AccountDTO> Accounts { get; set; }
    }
}