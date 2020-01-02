using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;
using StudyGroups.WebAPI.Models;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpPost, Route("registration"), DisableRequestSizeLimit]
        public IActionResult Registration([FromForm]StudentRegistrationDTO userReg)
        {
            _authenticationService.RegisterUser(userReg);
            return Ok();
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody]LoginDTO user)
        {
            string token = _authenticationService.Login(user);
            return Ok(new { Token = token });
        }

    }
}