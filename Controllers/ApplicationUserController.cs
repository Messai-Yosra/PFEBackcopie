using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using stage_api.configuration;
using stage_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace stage_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _UserManager;
        private SignInManager<ApplicationUser> _SignInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly dbContext _context;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<ApplicationSettings> appSettings, dbContext context)
        {
            _UserManager = userManager;
            _SignInManager = signInManager;
            _appSettings = appSettings.Value;
            _context = context;
        }

        // GET: api/GetUsers
        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> getUsers()
        {
            if (_context.ApplicationUsers == null)
            {
                return NotFound();
            }
            return await _context.ApplicationUsers.ToListAsync();
        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            model.Role = "USER";
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber


            };

            try
            {
                var result = await _UserManager.CreateAsync(applicationUser, model.Password);
                await _UserManager.AddToRoleAsync(applicationUser, model.Role);
                return Ok(result);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        [Route("Login")]
        //POST : /api/ApplicationUser/Login
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _UserManager.FindByNameAsync(model.UserName);
            if (user != null && await _UserManager.CheckPasswordAsync(user, model.Password))
            {
               
                    //Get role assigned to the user
                    var role = await _UserManager.GetRolesAsync(user);
                    IdentityOptions _options = new IdentityOptions();

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,role.FirstOrDefault())
                        }),
                        Expires = DateTime.UtcNow.AddMinutes(30),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Ok(new { token });
                
                

            }
            else
                return BadRequest(new { message = "Username or password is incorrect. " });
        }
    }
}
