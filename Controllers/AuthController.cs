using Microsoft.AspNetCore.Identity;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using CodeCamp.Filters;
using CodeCamp.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CodeCamp.Controllers
{
    public class AuthController : Controller
    {
        private CampContext _context;
        private ILogger<AuthController> _logger;
        private SignInManager<CampUser> _signInManager;

        // allower developers to send us credentials, and we will set a cookie.
        // use cookie if only one browser-based web app uses the api.
        public AuthController(CampContext context, 
            SignInManager<CampUser> signInManager,
            ILogger<AuthController> logger )
        {
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
        }

        // use POST instead of GET because we don't want the crendentials to be sent in plain text query string
        [HttpPost("api/auth/login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                // false: don't persist cookie after browser is closed;
                // false: don't lockout user on failer
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
                // if login fails, return BadRequest
            }
            catch (System.Exception)
            {

                _logger.LogError($"Exception thrown while loggin in");
            }

            return BadRequest("Falled login");
        }
    }
}
