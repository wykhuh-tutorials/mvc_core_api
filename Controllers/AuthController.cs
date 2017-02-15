using Microsoft.AspNetCore.Identity;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using CodeCamp.Filters;
using CodeCamp.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace CodeCamp.Controllers
{
    public class AuthController : Controller
    {
        private CampContext _context;
        private ILogger<AuthController> _logger;
        private SignInManager<CampUser> _signInManager;
        private UserManager<CampUser> _userMgr;
        private IPasswordHasher<CampUser> _hasher;
        private IConfigurationRoot _config;

        // allower developers to send us credentials, and we will set a cookie.
        // use cookie if only one browser-based web app uses the api.
        public AuthController(CampContext context, 
            SignInManager<CampUser> signInManager,
            ILogger<AuthController> logger,
            UserManager<CampUser> userMgr,
            IPasswordHasher<CampUser> hasher,
            IConfigurationRoot config)
        {
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
            _userMgr = userMgr;
            _hasher = hasher;
            _config = config;
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
            catch (Exception ex)
            {

                _logger.LogError($"Exception thrown while loggin in {ex}");
            }

            return BadRequest("Falled login");
        }

        [ValidateModel]
        [HttpPost("api/auth/token")]
        public async Task<IActionResult> CreateToken([FromBody] CredentialModel model)
        {
            try
            {
                // check if POST username exists 
                var user = await _userMgr.FindByNameAsync(model.Username);
                if (user != null)
                {
                    // check if POST password matches the stored PasswordHash
                    if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password)  == PasswordVerificationResult.Success)
                    {
                        // claims from Identity
                        var userClaims = await _userMgr.GetClaimsAsync(user);

                        // custom claims
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email)
                        }.Union(userClaims);

                        // in real prod code, but config somewhere else
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        // create token

                        var token = new JwtSecurityToken(
                            issuer: _config["Tokens:Issuer"],
                            audience: _config["Tokens:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(15),
                            signingCredentials: creds
                          );


                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Exception thrown while creating token: {ex}");

            }

            return BadRequest("Failed token creation");
        }
    }
}
