using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;
using StudyGroups.DTOmodels;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Utils;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            this._studentService = studentService;
        }

        /// <summary>
        /// Gets all student who enrolled to a given subject in actual semester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("group-search/{id}"), Authorize(Roles = "Student")]
        public ActionResult<string> GetStudentsEnrolledToSubject(string id)
        {
            string semester = SemesterManager.GetCurrentSemester();
            var students = _studentService.GetStudentsAttendedToSubject(id, semester);
            return Ok(students);
        }

        /// <summary>
        /// Fetch a user student with details.
        /// </summary>
        /// <returns>Student with profile details.</returns>
        [HttpGet("details"), Authorize(Roles = "Student")]
        public ActionResult<StudentDTO> GetStudentDetails()
        {
            var userID = GetUserIdFromToken();
            StudentDTO student = _studentService.GetStudentDetails(userID);
            return Ok(student);
        }

        /// <summary>
        /// Fetch student details.
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        [HttpGet("details/{id}"), Authorize(Roles = "Student")]
        public ActionResult<StudentDTO> GetStudentDetailsById(string id)
        {
            StudentDTO student = _studentService.GetStudentDetails(id);
            return Ok(student);
        }


        /// <summary>
        /// Gets filtered student list, applied with given params.
        /// </summary>
        /// <param name="searchParams">Object holding filter parameters.</param>
        /// <returns>List of students, who met the search criteria.</returns>
        [HttpGet("team-search"), Authorize(Roles = "Student")]
        public ActionResult<string> GetStudentsFromStudyGroupSearch([FromQuery] StudyGroupSearchDTO searchParams)
        {
            string id = GetUserIdFromToken();
            IEnumerable<StudentListItemDTO> student = _studentService.GetStudentFromStudyGroupSearch(searchParams, id);
            return Ok(student);
        }

        /// <summary>
        /// Gets filtered student list, applied with given params.
        /// </summary>
        /// <param name="searchParams">Object holding filter parameters.</param>
        /// <returns>List of students, who met the search criteria.</returns>
        [HttpGet("study-search"), Authorize(Roles = "Student")]
        public ActionResult<string> GetStudentsFromStudyBuddySearch([FromQuery] StudyBuddySearchDTO searchParams)
        {
            string id = GetUserIdFromToken();
            IEnumerable<StudentListItemDTO> student = _studentService.GetStudentFromStudyBuddySearch(searchParams, id);
            return Ok(student);
        }

        /// <summary>
        /// Updates a student.
        /// </summary>
        /// <param name="value">Student DTO with updateable fields.</param>
        /// <returns></returns>
        [HttpPut("update"), Authorize(Roles = "Student")]
        public IActionResult UpdateStudent([FromBody] StudentDTO value)
        {
            var studentId = GetUserIdFromToken();
            _studentService.UpdateStudentAndTutoringRelationShips(value, studentId);
            return Ok();
        }

        /// <summary>
        /// Gets students who are tutoring the given subject.
        /// </summary>
        /// <returns>List of subjects as selection items.</returns>
        [Authorize]
        [HttpGet("tutors/{id}")]
        public ActionResult<string> GetAllStudentTutoringSubject(string id)
        {
            IEnumerable<StudentListItemDTO> studentListItemDtos = _studentService.GetStudentsTutoringSubject(id, GetUserIdFromToken());
            return Ok(studentListItemDtos);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("reports/semester-avgs")]
        public ActionResult<string> GetSemesterAverages()
        {
            var avg = _studentService.GetSemesterAverages();
                return Ok(avg);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(string id)
        {
            _studentService.DeleteStudent(id);
            return Ok();
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