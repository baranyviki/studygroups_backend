using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;
using StudyGroups.WebAPI.Services.Utils;
using StudyGroups.WebAPI.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using StudyGroups.DTOmodels;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        IStudentService _studentService;      

        public StudentController(IStudentService studentService)
        {
            this._studentService = studentService;
        }

        /// <summary>
        /// Gets all student who enrolled to a given subject in current semester/all time
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/student/5/'2015/16/1'
        [HttpGet("group-search/{id}"),Authorize(Roles ="Student")]
        public ActionResult<string> GetStudentsEnrolledToSubject(string id)
        {
            // TODO: need to calculate from date
            string semester = SemesterManager.GetCurrentSemester();
            var students = _studentService.GetStudentsAttendedToSubject(id, semester);
            return Ok(students);
        }

        /// <summary>
        /// Fetch a student with details.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Student with profile details.</returns>
        [HttpGet("details"), Authorize(Roles = "Student")]
        public ActionResult<StudentDTO> GetStudentDetails()
        {
            var userID = GetUserIdFromToken();
            StudentDTO student=_studentService.GetStudentDetails(userID);
            return Ok(student);
        }
        
        /// <summary>
        /// Gets filtered student list, applied with given params.
        /// </summary>
        /// <param name="searchParams">Object holding filter parameters.</param>
        /// <returns>List of students, who met the search criteria.</returns>
        [HttpGet("team-search"), Authorize(Roles = "Student")]
        public ActionResult<string> GetStudentsFromStudyGroupSearch ([FromQuery] StudyGroupSearchDTO searchParams)
        {
            string id = GetUserIdFromToken();
            List<StudentListItemDTO> student = _studentService.GetStudentFromStudyGroupSearch(searchParams,id);
            return Ok(student);
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