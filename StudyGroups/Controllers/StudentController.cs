using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        IStudentService studentService;

        public StudentController(IStudentService studentService)
        {
            this.studentService = studentService;
        }

        // GET api/student/5/'2015/16/1'
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            // TODO: need to calculate from date
            string semester = "2017/18/1";
            var students = studentService.GetStudentsAttendedToSubject(id, semester);
            return Ok(students);
        }



    }
}