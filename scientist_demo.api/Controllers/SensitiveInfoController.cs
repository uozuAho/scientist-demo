﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GitHub;
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
            return Scientist.Science<
                IEnumerable<DataWithSomeSensitiveStuff>,
                IEnumerable<DataWithSomeSensitiveStuff>>(
                "sensitive-stuff", experiment =>
                {
                    experiment.Compare(CompareOriginalAndNewResults);
                    experiment.AddContext("request id", Guid.NewGuid());

                    experiment.Use(OriginalGetStuff);
                    experiment.Try(NewGetStuff);

                    experiment.Clean(dirty => dirty.Select(SanitiseForReporting));
                });
        }

        private static bool CompareOriginalAndNewResults(
            IEnumerable<DataWithSomeSensitiveStuff> original,
            IEnumerable<DataWithSomeSensitiveStuff> _new)
        {
            var cleanedOriginal = original.Select(IgnoreFields);
            var cleanedNew = _new.Select(IgnoreFields);

            return Serialize(cleanedOriginal) == Serialize(cleanedNew);
        }

        private static DataWithSomeSensitiveStuff IgnoreFields(DataWithSomeSensitiveStuff original)
        {
            return new()
            {
                Timestamp = DateTime.MaxValue,
                Version = Guid.Empty,
                Customers = original.Customers,
                NumberOfThings = original.NumberOfThings
            };
        }

        private static DataWithSomeSensitiveStuff SanitiseForReporting(DataWithSomeSensitiveStuff original)
        {
            return new()
            {
                Timestamp = DateTime.MaxValue,
                Version = Guid.Empty,
                Customers = original.Customers.Select(c => new Customer
                {
                    Age = c.Age,
                    Email = "***removed***",
                    Name = "***removed***"
                }).ToList(),
                NumberOfThings = original.NumberOfThings
            };
        }

        private static IEnumerable<DataWithSomeSensitiveStuff> OriginalGetStuff()
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

        private static IEnumerable<DataWithSomeSensitiveStuff> NewGetStuff()
        {
            return new[]
            {
                new DataWithSomeSensitiveStuff
                {
                    NumberOfThings = 8,
                    Customers = new List<Customer>
                    {
                        new()
                        {
                            Name = "Tim",
                            Age = 12,
                            Email = "3@4.com"
                        }
                    }
                }
            };
        }

        private static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }

    public class DataWithSomeSensitiveStuff
    {
        public Guid Version { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
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
