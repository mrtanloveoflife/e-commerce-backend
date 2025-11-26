using E_commerce.Services;
using E_commerce.Dto;
using E_commerce.Models.Request;
using Microsoft.AspNetCore.Mvc;
using E_commerce.Services.Request;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<ProductDto>>> GetProducts(
            [FromQuery] GetProductsRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _productService.GetProductsAsync(request, cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailDto>> GetProduct([FromRoute]int id, CancellationToken cancellationToken)
        {
            var result = await _productService.GetProductAsync(id, cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        // Accept multipart/form-data for file upload
        [HttpPost]
        public async Task<ActionResult<ProductDetailDto>> CreateProduct([FromForm]CreateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _productService.CreateProductAsync(request, cancellationToken);
            return CreatedAtAction(nameof(CreateProduct), new { id = result.Id }, result);
        }

        // Accept multipart/form-data for file upload on update
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDetailDto>> UpdateProduct([FromRoute]int id, [FromForm] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;
            var result = await _productService.UpdateProductAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            await _productService.DeleteProductAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
