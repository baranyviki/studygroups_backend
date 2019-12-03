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
        // GET api/subject/'
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("selections")]
        public ActionResult<string> GetAllSelectionItems()
        {
            var subjectListItems = subjectService.GetAllSubjectsAsSelectionItem();
            return Ok(subjectListItems);
        }
    }

}