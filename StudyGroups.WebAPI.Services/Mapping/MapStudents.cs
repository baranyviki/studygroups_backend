using StudyGroups.DataAccessLayer.DAOs;
using StudyGroups.DTOmodels;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Services.Mapping
{
    internal static class MapStudents
    {
        internal static StudentListItemDTO MapStudentDBModelToStudentDTO(Student studentDBModel)
        {
            return new StudentListItemDTO {
                Id = studentDBModel.UserID,
                Email = studentDBModel.Email,
                Name = studentDBModel.FirstName+" "+studentDBModel.LastName
            };

        }


    }
}
