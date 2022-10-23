using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers
{
    [ApiController]
    public class GenericController : Controller
    {
        private readonly DataContext _dataContext;
        public IConfiguration _configuration;

        public GenericController(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext; //внедрение зависимостей
            _configuration = configuration;
        }

        //вывод списка товаров
        [HttpGet("{{host}}/products")]
        public async Task<ActionResult<List<Product>>> GetAllProduct()
        {
            var product = await _dataContext.Products.ToListAsync();

            if (product.Any())
                return Ok(product);

            if (!product.Any())
                return NotFound("Not found");

            return Ok();
        }

        //регистрация
        [HttpPost("{{host}}/signup")]
        public async Task<IActionResult> Registration(User user)
        {
            var check = await _dataContext.Users.Where(x => x.Email == user.Email).FirstOrDefaultAsync();
            if (check == null)
            {
                user.Role = "user";
                await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();
                UserDTO userDTO = new UserDTO
                {
                    Email = user.Email,
                    Password = user.Password
                };
                return await Authorization(userDTO);
            }
            else
                return BadRequest("User with this email is already registered");
        }

        //авторизация
        [HttpPost("{{host}}/login")]
        public async Task<IActionResult> Authorization(UserDTO user)
        {
            if (user != null && user.Email != null && user.Password != null)
            {
                var userData = await GetUser(user);
                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

                if (userData != null)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Email", userData.Email),
                        new Claim("Password", userData.Password),
                        new Claim(ClaimTypes.Role, userData.Role)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.key));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                            jwt.Issuer,
                            jwt.Audience,
                                claims,
                                expires: DateTime.Now.AddMinutes(20),
                                signingCredentials: signIn
                            );

                    Helper.idUser = userData.Id;

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                    return BadRequest("Authentication failed");
            }
            else
                return BadRequest("Authentication failed");
        }

        private async Task<User> GetUser(UserDTO user)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);
        }
    }
}
