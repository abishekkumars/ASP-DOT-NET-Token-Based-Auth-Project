﻿using ABC_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ABC_Website.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IConfiguration _configuration;
        private readonly MovieDbContext _context;

        public TokenController(IConfiguration config, MovieDbContext context)
        {
            _configuration = config;
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Post(UserInfo _userData)
        {

            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {
                var user = await GetUser(_userData.Email, _userData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("FirstName", user.Firstname),
                    new Claim("LastName", user.Lastname),
                    new Claim("Email", user.Email),
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    var LogIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: LogIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        //private async Task<UserDetailss> GetUser(string email, string password)
        //{
        //    UserDetailss userDetails = null;
        //    var result = await _context.usersdetails.Where(u => u.Email == email && u.Password == password).ToListAsync();
        //    if (result.Count > 0)
        //        userDetails = result[0];
        //    return userDetails;

        //}
        private async Task<UserInfo> GetUser(string email, string password)
        {
            UserInfo usercredential = null;
            var response = await _context.obj.Where(u => u.Email == email && u.Password == password).ToListAsync();
            if (response.Count > 0)
                usercredential = response[0];
            return usercredential;



        }

    }
}
