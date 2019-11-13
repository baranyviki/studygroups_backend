using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;

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

        // GET api/subject/'
        [HttpGet("selections")]
        public ActionResult<string> GetAllSelectionItems()
        {
            var subjectListItems = subjectService.GetAllSubjectsAsSelectionItem();
            return Ok(subjectListItems);
        }


    }
}