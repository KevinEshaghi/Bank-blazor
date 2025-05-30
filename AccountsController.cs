using bankapi.Models;
using bankapi.Server;
using Microsoft.AspNetCore.Mvc;

namespace bankapi.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDTO>> GetAccount(int id)
        {
            var account = await _accountService.GetAccountAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpGet("{id}/balance")]
        public async Task<ActionResult<decimal>> GetBalance(int id)
        {
            var balance = await _accountService.GetBalanceAsync(id);
            if (balance == default(decimal))
            {
                return NotFound();
            }
            return Ok(balance);
        }

        [HttpGet("{id}/transactions")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactions(int id)
        {
            var transactions = await _accountService.GetTransactionsAsync(id);
            return Ok(transactions);
        }

        [HttpPost("deposit")]
        public async Task<ActionResult<AccountDTO>> Deposit([FromBody] DepositRequestDTO request)
        {
            try
            {
                var account = await _accountService.DepositAsync(request);
                if (account == null)
                {
                    return NotFound();
                }
                return Ok(account); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<AccountDTO>> Withdraw([FromBody] WithdrawRequestDTO request)
        {
            try
            {
                var account = await _accountService.WithdrawAsync(request);
                if (account == null)
                {
                    return NotFound();
                }
                return Ok(account);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDTO transferDto)
        {
            if (transferDto == null)
                return BadRequest();

            var result = await _accountService.TransferAsync(transferDto);
            if (result == null)
                return BadRequest("Transfer failed.");

            return Ok(result);
        }
    }
}
