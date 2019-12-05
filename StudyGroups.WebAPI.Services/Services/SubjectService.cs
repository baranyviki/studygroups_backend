using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Mapping;
using StudyGroups.WebAPI.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudyGroups.WebAPI.Services
{
    public class SubjectService : ISubjectService
    {
        ISubjectRepository _subjectRepository;
        
        public SubjectService(ISubjectRepository subjectRepository)
        {
            this._subjectRepository = subjectRepository;
        }

        public List<GeneralSelectionItem> GetAllSubjectsAsSelectionItem()
        {
            var subjects = _subjectRepository.FindAll();
            List<GeneralSelectionItem> subjectDTOs = new List<GeneralSelectionItem>();
            subjects.ToList().ForEach(x => subjectDTOs.Add(MapSubjects.MapSubjectToGeneralSelectionItem(x)));
            return subjectDTOs;
        }

        public IEnumerable<SubjectDTO> GetSubjectUserHasPassedAsSubjectDTO(string userId)
        {
            var subjects = _subjectRepository.GetSubjectsStudentHasPassed(userId);
            var selectionItems = subjects.Select(x => MapSubjects.MapSubjectToSubjectDTO(x));
            return selectionItems;
        }

        
    }
}
