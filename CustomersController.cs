using bankapi.Models;
using bankapi.Server;
using Microsoft.AspNetCore.Mvc;

namespace bankapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            var customers = await _customerService.GetCustomersAsync();
            return Ok(customers); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer); 
        }
    }
}