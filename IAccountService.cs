using bankapi.Data;
using bankapi.Models;
using Microsoft.EntityFrameworkCore;

namespace bankapi.Server
{
    public interface IAccountService
    {
        Task<AccountDTO> GetAccountAsync(int id);
        Task<decimal> GetBalanceAsync(int id);
        Task<IEnumerable<TransactionDTO>> GetTransactionsAsync(int id);
        Task<AccountDTO> DepositAsync(DepositRequestDTO request);
        Task<AccountDTO> WithdrawAsync(WithdrawRequestDTO request);
        Task<TransferResponse> TransferAsync(TransferRequestDTO request);
    }


    public class AccountService : IAccountService
    {
        private readonly BankBlazorContext _context;

        public AccountService(BankBlazorContext context)
        {
            _context = context;
        }

        public async Task<AccountDTO> GetAccountAsync(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.Dispositions)
                .ThenInclude(d => d.Customer)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null)
            {
                return null;
            }

            return new AccountDTO
            {
                AccountId = account.AccountId,
                Balance = account.Balance,
                AccountHolderName = account.Dispositions.FirstOrDefault()?.Customer?.Givenname + " " + account.Dispositions.FirstOrDefault()?.Customer?.Surname
            };
        }

        public async Task<decimal> GetBalanceAsync(int id)
        {
            var balance = await _context.Accounts
                .Where(a => a.AccountId == id)
                .Select(a => a.Balance)
                .FirstOrDefaultAsync();

            return balance;
        }

        public async Task<IEnumerable<TransactionDTO>> GetTransactionsAsync(int id)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == id)
                .Select(t => new TransactionDTO
                {
                    TransactionId = t.TransactionId,
                    Date = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance
                })
                .ToListAsync();
        }

        public async Task<AccountDTO> DepositAsync(DepositRequestDTO request)
        {
            var account = await _context.Accounts
                .Include(a => a.Dispositions)
                .ThenInclude(d => d.Customer)
                .FirstOrDefaultAsync(a => a.AccountId == request.AccountId);

            if (account == null)
            {
                return null;
            }

            account.Balance += request.Amount;

            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = "Credit",
                Operation = "Deposit",
                Amount = request.Amount,
                Balance = account.Balance
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new AccountDTO
            {
                AccountId = account.AccountId,
                Balance = account.Balance,
                AccountHolderName = account.Dispositions.FirstOrDefault()?.Customer?.Givenname + " " + account.Dispositions.FirstOrDefault()?.Customer?.Surname
            };
        }

        public async Task<AccountDTO> WithdrawAsync(WithdrawRequestDTO request)
        {
            var account = await _context.Accounts
                .Include(a => a.Dispositions)
                .ThenInclude(d => d.Customer)
                .FirstOrDefaultAsync(a => a.AccountId == request.AccountId);

            if (account == null)
            {
                return null;
            }

            if (account.Balance < request.Amount)
            {
                throw new InvalidOperationException("Otillräckligt saldo.");
            }

            account.Balance -= request.Amount;

            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = "Debit",
                Operation = "Withdrawal",
                Amount = -request.Amount,
                Balance = account.Balance
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new AccountDTO
            {
                AccountId = account.AccountId,
                Balance = account.Balance,
                AccountHolderName = account.Dispositions.FirstOrDefault()?.Customer?.Givenname + " " + account.Dispositions.FirstOrDefault()?.Customer?.Surname
            };
        }

        public async Task<TransferResponse> TransferAsync(TransferRequestDTO request)
        {
            if (request.FromAccountId == request.ToAccountId)
            {
                throw new InvalidOperationException("Käll och målkonto kan inte vara samma.");
            }

            var fromAccount = await _context.Accounts
                .Include(a => a.Dispositions)
                .ThenInclude(d => d.Customer)
                .FirstOrDefaultAsync(a => a.AccountId == request.FromAccountId);

            var toAccount = await _context.Accounts
                .Include(a => a.Dispositions)
                .ThenInclude(d => d.Customer)
                .FirstOrDefaultAsync(a => a.AccountId == request.ToAccountId);

            if (fromAccount == null || toAccount == null)
            {
                throw new InvalidOperationException("Ett eller båda kontona hittades inte.");
            }

            if (fromAccount.Balance < request.Amount)
            {
                throw new InvalidOperationException("Otillräckligt saldo.");
            }

            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            var fromTransaction = new Transaction
            {
                AccountId = request.FromAccountId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = "Debit",
                Operation = $"Transfer to {request.ToAccountId}",
                Amount = -request.Amount,
                Balance = fromAccount.Balance
            };

            var toTransaction = new Transaction
            {
                AccountId = request.ToAccountId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = "Credit",
                Operation = $"Transfer from {request.FromAccountId}",
                Amount = request.Amount,
                Balance = toAccount.Balance
            };

            _context.Transactions.Add(fromTransaction);
            _context.Transactions.Add(toTransaction);
            await _context.SaveChangesAsync();

            return new TransferResponse
            {
                FromAccount = new AccountDTO
                {
                    AccountId = fromAccount.AccountId,
                    Balance = fromAccount.Balance,
                    AccountHolderName = fromAccount.Dispositions.FirstOrDefault()?.Customer?.Givenname + " " + fromAccount.Dispositions.FirstOrDefault()?.Customer?.Surname
                },
                ToAccount = new AccountDTO
                {
                    AccountId = toAccount.AccountId,
                    Balance = toAccount.Balance,
                    AccountHolderName = toAccount.Dispositions.FirstOrDefault()?.Customer?.Givenname + " " + toAccount.Dispositions.FirstOrDefault()?.Customer?.Surname
                }
            };
        }
    }
}