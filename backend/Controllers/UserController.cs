using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;

        public UserController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        //добавление товара в корзину
        [HttpPost("{{host}}/cart/{product_id}")]
        public async Task<ActionResult> AddProductToCart(int product_id)
        {
            var product = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == product_id);
            if (product != null)
            {
                var cart = await _dataContext.Carts.FirstOrDefaultAsync(x => x.UserId == Helper.idUser && x.ProductId == product_id);
                Cart addCart;
                if (cart != null)
                {
                    addCart = new Cart
                    {
                        UserId = Helper.idUser,
                        ProductId = product_id
                    };
                }

                else
                {
                    addCart = new Cart
                    {
                        UserId = Helper.idUser,
                        ProductId = product_id
                    };
                }

                await _dataContext.Carts.AddAsync(addCart);
                await _dataContext.SaveChangesAsync();
                return Ok("Product add to card");
            }
            else
                return BadRequest();
        }

        //просмотр товаров в корзине
        [HttpGet("{{host}}/cart")]
        public async Task<ActionResult<List<CartProductDTO>>> GetProductsFromCart()
        {
            List<CartProductDTO> list = new List<CartProductDTO>();

            var cart = await _dataContext.Carts.Include(x => x.Product).Where(x => x.User.Id == Helper.idUser).ToListAsync();

            if (cart.Any())
            {
                foreach (var product in cart)
                {
                    CartProductDTO cartProductDTO = new CartProductDTO
                    {
                        CartId = product.Id,
                        Product = product.Product
                    };

                    list.Add(cartProductDTO);
                }

                return Ok(list);
            }
            else
                return NotFound("#Not found");
        }

        //удаление товара из корзины
        [HttpDelete("{{host}}/cart/{id}")]
        public async Task<ActionResult<Product>> DeleteProductFromCart(int id)
        {
            var cart = await _dataContext.Carts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == Helper.idUser);

            if (cart != null)
            {
                _dataContext.Carts.Remove(cart);
                await _dataContext.SaveChangesAsync();
                return Ok("Item removed from cart");
            }
            else
                return NotFound("Not found");
        }

        //оформление заказа
        [HttpPost("{{host}}/order")]
        public async Task<ActionResult> PostNewOrder()
        {
            var cart = await _dataContext.Carts.Where(x => x.UserId == Helper.idUser).ToListAsync();

            if (cart.Any())
            {
                var order = await _dataContext.Orders.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                int numberOrder = 0;
                if (order != null)
                    numberOrder = order.Number + 1;

                foreach (var item in cart)
                {
                    Order newOrder = new Order
                    {
                        ProductId = item.ProductId,
                        UserId = Helper.idUser,
                        Number = numberOrder
                    };
                    await _dataContext.Orders.AddAsync(newOrder);
                    _dataContext.Carts.RemoveRange(cart);
                }

                await _dataContext.SaveChangesAsync();
                return Ok("Order is processed");
            }
            else
                return UnprocessableEntity("Cart is empty");
        }

        //вывод заказов
        [HttpGet("{{host}}/order")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrder()
        {
            List<OrderDTO> orderDTO = new List<OrderDTO>();
            var orders = _dataContext.Orders.Where(x => x.UserId == Helper.idUser).ToList().GroupBy(x => x.Number).ToList();

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    OrderDTO newOrder = new OrderDTO();

                    List<int> listProducts = new List<int>();

                    foreach (var item in order)
                    {
                        listProducts.Add(item.ProductId);
                        newOrder.Id = item.Number;
                        var product = await _dataContext.Products.FirstAsync(x => x.Id == item.ProductId);
                        newOrder.Price += product.Price;
                    }
                    newOrder.Products = listProducts.ToArray();

                    orderDTO.Add(newOrder);
                }

                return Ok(orderDTO);
            }

            else
                return NotFound("Not found");
        }
    }
}
