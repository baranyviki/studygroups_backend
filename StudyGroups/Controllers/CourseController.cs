﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.WebAPI.Models;

namespace StudyGroups.WebAPI.WebSite.Controllers
{
    [Route("api/course")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Returns all courses of subjects student enrolled to currently.
        /// </summary>
        /// <returns>Subjects as selection items</returns>
        [HttpGet("current-courses"), Authorize(Roles = "Student")]
        public ActionResult<string> GetAllCourseWithSubjectStudentCurrentlyHas()
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken tokens = handler.ReadToken(authHeader) as JwtSecurityToken;
            var id = tokens.Claims.Where(claim => claim.Type == JwtRegisteredClaimNames.Sub).SingleOrDefault().Value;

            var subjects = _courseService.GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester(id);
            return Ok(subjects);
        }



    }
}