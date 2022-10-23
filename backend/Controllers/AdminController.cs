using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;

        public AdminController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        //добавление товара
        [HttpPost("{{host}}/products")]
        public async Task<ActionResult> AddProduct(Product product)
        {
            if (product.Name != null && product.Description != null && product.Price != 0)
            {
                await _dataContext.Products.AddAsync(product);
                await _dataContext.SaveChangesAsync();
                return Created("Product added", product);
            }
            else
                return BadRequest();
        }

        //редактирование отвара
        [HttpPut("{{host}}/product/{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(Product newProduct, int id)
        {
            var product = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product != null)
            {
                product.Name = newProduct.Name;
                product.Description = newProduct.Description;
                product.Price = newProduct.Price;

                _dataContext.Products.Update(product);
                await _dataContext.SaveChangesAsync();
                return Ok(product);
            }

            else
                return NotFound("Not found");
        }

        //удаление товара
        [HttpDelete("{{host}}/product/{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product != null)
            {
                _dataContext.Products.Remove(product);
                await _dataContext.SaveChangesAsync();
                return Ok("Product removed");
            }
            else
                return NotFound("Not found");
        }
    }
}
