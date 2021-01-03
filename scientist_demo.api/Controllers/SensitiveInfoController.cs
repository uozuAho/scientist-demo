using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace scientist_demo.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensitiveInfoController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<DataWithSomeSensitiveStuff> Get()
        {
            return new[]
            {
                new DataWithSomeSensitiveStuff
                {
                    NumberOfThings = 7,
                    Customers = new List<Customer>
                    {
                        new()
                        {
                            Name = "Barry",
                            Age = 11,
                            Email = "1@2.com"
                        }
                    }
                }
            };
        }
    }

    public class DataWithSomeSensitiveStuff
    {
        public Guid Version { get; set; } = Guid.NewGuid();
        public int NumberOfThings { get; set; }
        public List<Customer> Customers { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }
}
