
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.DTOmodels;
using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Services.Mapping
{
    internal static class MapStudent
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
                GenderType = studentDBModel.GenderType,
                InstagramName = studentDBModel.InstagramName,
                MessengerName = studentDBModel.MessengerName,
                NeptunCode = studentDBModel.NeptunCode,
                UserName = studentDBModel.UserName,
                ImagePath = studentDBModel.ImagePath
            };

        }

        internal static Student MapStudentDTOToStudentDBModel(StudentDTO studentDto,string userId) {

            return new Student {
                UserID =userId,
                NeptunCode = studentDto.NeptunCode,
                Email = studentDto.Email,
                MessengerName = studentDto.MessengerName,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                GenderType = studentDto.GenderType,
                ImagePath = studentDto.ImagePath,
                InstagramName = studentDto.InstagramName               
            };
            
        }
    }
}
