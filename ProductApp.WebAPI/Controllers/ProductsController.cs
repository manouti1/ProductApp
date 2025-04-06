using Microsoft.AspNetCore.Mvc;
using ProductApp.Shared;
using ProductApp.WebAPI.Repositories;

namespace ProductApp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { Message = "Product not found" });

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                return BadRequest(new { Message = "Name is required" });

            if (product.Price < 0 || product.Quantity < 0)
                return BadRequest(new { Message = "Price and quantity must be non-negative" });

            await _repository.AddAsync(product);

            // Return the product with the generated Id
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product updatedProduct)
        {
            if (string.IsNullOrWhiteSpace(updatedProduct.Name))
                return BadRequest(new { Message = "Name is required" });

            if (updatedProduct.Price < 0 || updatedProduct.Quantity < 0)
                return BadRequest(new { Message = "Price and quantity must be non-negative" });

            var existingProduct = await _repository.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound(new { Message = "Product not found" });

            updatedProduct.Id = id;
            await _repository.UpdateAsync(updatedProduct);
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _repository.DeleteAsync(id))
                return Ok(new { Message = "Product successfully deleted" });

            return NotFound(new { Message = "Product not found" });
        }
    }
}
