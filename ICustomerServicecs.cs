using bankapi.Data;
using bankapi.Models;
using Microsoft.EntityFrameworkCore;

namespace bankapi.Server
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetCustomersAsync();
        Task<CustomerDTO> GetCustomerAsync(int id);
    }


    public class CustomerService : ICustomerService
    {
        private readonly BankBlazorContext _context;

        public CustomerService(BankBlazorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerDTO>> GetCustomersAsync()
        {
            return await _context.Customers
                .Select(c => new CustomerDTO
                {
                    CustomerId = c.CustomerId,
                    Givenname = c.Givenname,
                    Surname = c.Surname
                })
                .ToListAsync();
        }

        public async Task<CustomerDTO> GetCustomerAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                Givenname = customer.Givenname,
                Surname = customer.Surname,
                Accounts = customer.Dispositions.Select(d => new AccountDTO
                {
                    AccountId = d.Account.AccountId,
                    Balance = d.Account.Balance,
                    AccountHolderName = customer.Givenname + " " + customer.Surname
                }).ToList()
            };
        }
    }
}
