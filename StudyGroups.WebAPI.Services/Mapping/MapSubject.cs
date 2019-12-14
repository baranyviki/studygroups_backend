using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;

namespace StudyGroups.WebAPI.Services.Mapping
{
    public static class MapSubject
    {
        public static GeneralSelectionItem MapSubjectToGeneralSelectionItem(Subject subjectDBModel)
        {
            return new GeneralSelectionItem
            {
                ID = subjectDBModel.SubjectID,
                DisplayName = $"{subjectDBModel.Name} - {subjectDBModel.SubjectCode}"
            };

        }

        public static SubjectListItemDTO MapSubjectToSubjectListItemDTO(Subject subjectDBModel)
        {
            return new SubjectListItemDTO
            {
                ID = subjectDBModel.SubjectID,
                Name = subjectDBModel.Name,
                SubjectCode = subjectDBModel.SubjectCode
            };
        }

        public static SubjectDTO MapSubjectToSubjectDTO(Subject subjectDbModel)
        {
            return new SubjectDTO
            {
                Credits = subjectDbModel.Credits,
                Name = subjectDbModel.Name,
                SubjectCode = subjectDbModel.SubjectCode,
                SubjectID = subjectDbModel.SubjectID,
                SubjectType = (SubjectType)subjectDbModel.SubjectType,
                SuggestedSemester = subjectDbModel.SuggestedSemester
            };
        }

        public static Subject MapSubjectDTOToSubject(SubjectDTO subjectDto)
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
