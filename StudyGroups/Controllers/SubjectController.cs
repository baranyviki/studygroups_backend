using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;
using StudyGroups.WebAPI.Models;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/subject")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        ISubjectService subjectService;

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


        /// <summary>
        /// Gets all subjects user has passed.
        /// </summary>
        /// <returns>List of subjects as selection items.</returns>
        [Authorize]
        [HttpGet("completed")]
        public ActionResult<string> GetAllCompletedSubjectSelectionItems()
        {
            string userId = GetUserIdFromToken();
            var subjectListItems = subjectService.GetSubjectUserHasPassedAsSubjectDTO(userId);
            return Ok(subjectListItems);
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