using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OData.Models;

namespace OData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ODataController
    {
        // Initialisation in memory
        private List<Product> products = new List<Product>()
        {
            new Product()
            {
                Id = 1,
                Name = "Bread",
            },
            new Product()
            {
                Id = 1,
                Name = "Milk",
            },
            new Product()
            {
                Id = 1,
                Name = "Potatoes",
            }
        };

        [EnableQuery(PageSize = 2)] // Annotation OData
        public List<Product> Get()
        {
            return products;
        }
    }
}