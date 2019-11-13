using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;
using StudyGroups.WebAPI.Models;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService _authenticationService)
        {
            this._authenticationService = _authenticationService;
        }


        [HttpPost, Route("createUser"),DisableRequestSizeLimit]
        public IActionResult CreateUser([FromForm]StudentRegistrationDTO userReg)
        {
            _authenticationService.RegisterUser(userReg);
            return Ok();
           
        }

    }
}