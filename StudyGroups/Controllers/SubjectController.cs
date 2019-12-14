using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;
using StudyGroups.WebAPI.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/subject")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            this.subjectService = subjectService;
        }

        /// <summary>
        /// Gets all subjects.
        /// </summary>
        /// <returns>Subjects as selection items</returns>
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("selections")]
        public ActionResult<string> GetAllSelectionItems()
        {
            var subjectListItems = subjectService.GetAllSubjectsAsSelectionItem();
            return Ok(subjectListItems);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public ActionResult<string> GetAll()
        {
            IEnumerable<SubjectListItemDTO> subjectListItems = subjectService.GetAllSubjectAsSubjectListItem();
            return Ok(subjectListItems);
        }

        /// <summary>
        /// Gets all subjects user has passed.
        /// </summary>
        /// <returns>List of subjects as selection items.</returns>
        [Authorize(Roles = "Student")]
        [HttpGet("completed")]
        public ActionResult<string> GetAllCompletedSubjectSelectionItems()
        {
            string userId = GetUserIdFromToken();
            var subjectListItems = subjectService.GetSubjectUserHasPassedAsSubjectDTO(userId);
            return Ok(subjectListItems);
        }

        [Authorize]
        [HttpGet("details/{id}")]
        public ActionResult<string> GetSubjectById(string subjectId)
        {
            var results = subjectService.GetSubjectById(subjectId);
            return Ok(results);
        }

        private string GetUserIdFromToken()
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken tokens = handler.ReadToken(authHeader) as JwtSecurityToken;
            return tokens.Claims.Where(claim => claim.Type == JwtRegisteredClaimNames.Sub).SingleOrDefault().Value;

        }

    }

}