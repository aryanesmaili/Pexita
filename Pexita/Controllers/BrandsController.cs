using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.Brands;
using Pexita.Exceptions;
using Pexita.Services.Interfaces;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        [HttpGet("Brands")]
        public IActionResult GetAllBrands()
        {
            try
            {
                var brands = _brandService.GetBrands();
                return Ok(brands);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("Brands/Get/{count:int}")]
        public IActionResult GetAllBrands(int count)
        {
            try
            {
                var brands = _brandService.GetBrands(count);
                return Ok(brands);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpGet("GetBrand/{id}")]
        public IActionResult GetBrands(int id)
        {
            try
            {
                return Ok(_brandService.GetBrandByID(id));
            }
            catch (NotFoundException)
            {
                return NotFound(id);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("Login")]
        public IActionResult Login([FromBody] BrandInfoVM brand)
        {
            return Ok();
        }

        [HttpPost("AddBrand")]
        public IActionResult AddBrand([FromBody] BrandCreateVM createVM)
        {
            try
            {
                _brandService.AddBrand(createVM);
                return Ok();
            }

            catch (ArgumentNullException e)
            {
                return BadRequest($"Arguement null {e.Message}");
            }

            catch (FormatException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("Edit/{id:int}")]
        public IActionResult EditBrand(int id, [FromBody] BrandUpdateVM brand)
        {
            try
            {
                _brandService.UpdateBrandInfo(id, brand);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound(nameof(id));
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Delete/{id:int}")]
        public IActionResult DeleteBrand(int id)
        {
            try
            {
                return _brandService.RemoveBrand(id) ? NoContent() : StatusCode(StatusCodes.Status500InternalServerError);
            }

            catch (NotFoundException)
            {
                return NotFound(id);
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
