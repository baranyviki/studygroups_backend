using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Services.Mapping
{
    internal static class MapSubject
    {
        internal static GeneralSelectionItem MapSubjectToGeneralSelectionItem(Subject subjectDBModel)
        {
            return new GeneralSelectionItem
            {
                ID = subjectDBModel.SubjectID,
                DisplayName = $"{subjectDBModel.Name} - {subjectDBModel.SubjectCode}"
            };

        }

        internal static SubjectListItemDTO MapSubjectToSubjectListItemDTO(Subject subjectDBModel)
        {
            return new SubjectListItemDTO
            {
                SubjectID = subjectDBModel.SubjectID,
                Name = $"{subjectDBModel.Name} - {subjectDBModel.SubjectCode}"
            };
        }

        internal static SubjectDTO MapSubjectToSubjectDTO(Subject subjectDbModel)
        {
            return new SubjectDTO {
            Credits=subjectDbModel.Credits,
            Name=subjectDbModel.Name,
            SubjectCode=subjectDbModel.SubjectCode,
            SubjectID = subjectDbModel.SubjectID,
            SubjectType=(SubjectType)subjectDbModel.SubjectType,
            SuggestedSemester=subjectDbModel.SuggestedSemester
            };
        }

        internal static Subject MapSubjectDTOToSubject(SubjectDTO subjectDto)
        {
            return new Subject
            {
                SubjectCode = subjectDto.SubjectCode,
                Credits = subjectDto.Credits,
                Name = subjectDto.Name,
                SubjectID = subjectDto.SubjectID,
                SubjectType = (int)subjectDto.SubjectType,
                SuggestedSemester = subjectDto.SuggestedSemester
            };
        }

    }
}
