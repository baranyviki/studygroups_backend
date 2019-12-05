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
        IEnumerable<SubjectDTO> GetSubjectUserHasPassedAsSubjectDTO(string userId);

    }
}
