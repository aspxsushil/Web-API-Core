using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NotificationApi.Models;
using NotificationApi.Services;

namespace NotificationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        [HttpPost, Route("Register")]
        public ActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Invalid Client Request");
            }
            var result = _authService.Create(user);

            if (!string.IsNullOrEmpty(result.Id))
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokenOptions = new JwtSecurityToken(

                    issuer: "https://localhost:44316",
                    audience: "http://localhost:4200", //valid receipients
                    claims: new List<Claim>(), //list of user roles like admin,manager
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signinCredentials

                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                
                return CreatedAtRoute("GetUser", new { id = result.Id },new {Token = tokenString,User = result });
            }
            else
            {
                return BadRequest();
            }
            
        }

        [HttpGet(Name = "GetUser"), Authorize]
        [Route("GetUser/{id}")]
        public ActionResult Get(string id)
        {
            var user = _authService.GetUser(id);
            return Ok(user);
        }
    }
}