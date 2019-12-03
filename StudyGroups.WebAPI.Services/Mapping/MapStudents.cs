
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.DTOmodels;
using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Services.Mapping
{
    internal static class MapStudents
    {
        internal static StudentListItemDTO MapStudentDBModelToStudentListItemDTO(Student studentDBModel)
        {
            return new StudentListItemDTO {
                Id = studentDBModel.UserID.ToString(),
                Email = studentDBModel.Email,
                Name = studentDBModel.FirstName+" "+studentDBModel.LastName
            };

        }

        internal static StudentDTO MapStudentDBModelToStudentDTO(Student studentDBModel)
        {
            return new StudentDTO
            {
                Email = studentDBModel.Email,
                FirstName = studentDBModel.FirstName,
                LastName = studentDBModel.LastName,
                DateOfBirth = studentDBModel.DateOfBirth,
                GenderType = studentDBModel.GenderType,
                InstagramName = studentDBModel.InstagramName,
                MessengerName = studentDBModel.MessengerName,
                NeptunCode = studentDBModel.NeptunCode,
                UserName = studentDBModel.UserName,
                ImagePath = studentDBModel.ImagePath
            };

        }


    }
}
