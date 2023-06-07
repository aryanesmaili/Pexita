using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.ViewModels;
using Pexita.Services;
using Pexita.Services.Interfaces;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("products")]
        public IActionResult GetAllProducts()
        {
            try
            {
                var result = _productService.GetAllProducts();
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("products/{id}")]
        public IActionResult GetProductByID(int id)
        {
            try
            {
                return Ok(_productService.GetProductByID(id));
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpPost("product/add")]
        public IActionResult AddProduct([FromBody] ProductVM product)
        {
            return _productService.AddProduct(product) ? Ok() : BadRequest();
        }
        [HttpPut("product/update/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductVM product)
        {
            return Ok(_productService.UpdateProductInfo(id, product));
        }
        [HttpDelete("products/delete/{id}")]
        public IActionResult DeleteProduct(int id) 
        {
            return _productService.DeleteProduct(id) ? Ok(id) : BadRequest(id);
        }
    }
}

