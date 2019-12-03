using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudyGroups.Contracts.Logic;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.WebSite.Exceptions;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        IAuthenticationService _authenticationService;
        private readonly IConfiguration _config;

        public AuthenticationController(IAuthenticationService authenticationService,IConfiguration config)
        {
            _authenticationService = authenticationService;
            _config = config;
        }


        [HttpPost, Route("registration"),DisableRequestSizeLimit]
        public async Task<IActionResult> RegistrationAsync([FromForm]StudentRegistrationDTO userReg)
        {
            await _authenticationService.RegisterUserAsync(userReg);
            return Ok();           
        }

        [HttpPost,Route("login")]
        public IActionResult Login([FromBody]LoginDTO user)
        {
            string token = _authenticationService.Login(user);
            return Ok(new { Token = token });

        }

    }
}