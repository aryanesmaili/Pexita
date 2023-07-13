using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.Brands;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllBrands() 
        {
            return Ok();
        }
        [HttpGet("GetBrand/{id}")]
        public IActionResult GetBrands(int id)
        {
            return Ok();
        }
        [HttpGet("Login")]
        public IActionResult Login([FromBody] BrandVM brand)
        {
            return Ok();
        }
        [HttpPost("AddBrand")]
        public IActionResult AddBrand()
        {
            return Ok();
        }
        [HttpPut("Edit")]
        public IActionResult EditBrand(int id, [FromBody] BrandVM brand)
        {
            return Ok();
        }
        [HttpDelete("Delete")]
        public IActionResult DeleteBrand(int id)
        {
            return Ok();
        }
    }
}
