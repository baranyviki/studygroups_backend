using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Contracts.Logic
{
    public interface ISubjectService
    {
        List<GeneralSelectionItem> GetAllSubjectsAsSelectionItem();
        IEnumerable<SubjectListItemDTO> GetSubjectUserHasPassedAsSubjectDTO(string userId);
        SubjectDTO GetSubjectById(string subjectId);
        void UpdateSubject(SubjectDTO subject);
    }
}
