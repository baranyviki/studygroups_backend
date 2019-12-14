using StudyGroups.WebAPI.Models;
using System.Collections.Generic;

namespace StudyGroups.Contracts.Logic
{
    public interface ISubjectService
    {
        List<GeneralSelectionItem> GetAllSubjectsAsSelectionItem();
        IEnumerable<GeneralSelectionItem> GetSubjectUserHasPassedAsSubjectDTO(string userId);
        SubjectDTO GetSubjectById(string subjectId);
        SubjectDTO UpdateSubject(SubjectDTO subject);
        IEnumerable<SubjectListItemDTO> GetAllSubjectAsSubjectListItem();
    }
}
