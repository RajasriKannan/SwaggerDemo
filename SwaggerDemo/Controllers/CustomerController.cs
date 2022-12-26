using Microsoft.AspNetCore.Mvc;
using SwaggerDemo.Models;


namespace SwaggerDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private List<Customer> _customers;
        private DBContext _context;
        private IConfiguration _configuration;
        
        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
            _context = new DBContext(_configuration);
            _customers = new List<Customer>()
            {
                new Customer() { Id = 1, Name = "John Beck", Department = "IT", DateOfBirth = new DateTime(1991, 12, 20) },
                new Customer() { Id = 2, Name = "Rich Ben", Department = "Sales", DateOfBirth = new DateTime(1987, 11, 01) },
                new Customer() { Id = 3, Name = "Marc Bell", Department = "IT", DateOfBirth = new DateTime(1991, 10, 05) }
            };
        }
        [HttpGet]
        public IEnumerable<Customer> GetCustomers()
        {
            return _context.GetCustomers().ToList();
        }

        [HttpGet]
        [Route("id")]
        public Customer GetCustomerByID(int  id)
        {
            return _customers.FirstOrDefault(c => c.Id == id);
        }
    }
}
