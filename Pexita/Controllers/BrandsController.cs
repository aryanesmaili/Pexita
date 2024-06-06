using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pexita.Data.Entities.Brands;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IValidator<BrandCreateVM> _brandCreateValidator;
        private readonly IValidator<BrandUpdateVM> _brandUpdateValidator;
        public BrandsController(IBrandService brandService,
            IValidator<BrandCreateVM> brandCreateValidator,
            IValidator<BrandUpdateVM> brandUpdateValidator)
        {
            _brandService = brandService;
            _brandCreateValidator = brandCreateValidator;
            _brandUpdateValidator = brandUpdateValidator;
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

        [Authorize(Policy = "Brand")]
        [HttpPost("AddBrand")]
        public async Task<IActionResult> AddBrand([FromBody] BrandCreateVM createVM)
        {
            try
            {
                if (createVM == null)
                    throw new ArgumentNullException(nameof(createVM));

                await _brandCreateValidator.ValidateAndThrowAsync(createVM);

                _brandService.AddBrand(createVM);
                return Ok();
            }

            catch (ArgumentNullException e)
            {
                return BadRequest($"Arguement null {e.Message}");
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "Brand")]
        [HttpPut("Edit/{id:int}")]
        public async Task<IActionResult> EditBrand(int id, [FromBody] BrandUpdateVM brand)
        {
            try
            {
                await _brandUpdateValidator.ValidateAndThrowAsync(brand);

                _brandService.UpdateBrandInfo(id, brand);
                return Ok();
            }

            catch (ValidationException e)
            {
                return BadRequest(e.Message);
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

        [Authorize(Policy = "Brand")]
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
