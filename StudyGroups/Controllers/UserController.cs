using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;
using StudyGroups.WebAPI.Models;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userService.GetUsers();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetUserByID(string id)
        {
            var user = _userService.GetUserByID(id);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public IActionResult UpdateUserDisabled(string id, [FromBody] UserPatchDTO user)
        {
            _userService.UpdateUserDisabled(user);
            return Ok();
        }

    }
}