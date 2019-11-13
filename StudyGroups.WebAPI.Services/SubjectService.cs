using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.DataAccessLayer.DAOs;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudyGroups.WebAPI.Services
{
    public class SubjectService : ISubjectService
    {
        ISubjectRepository subjectRepository;


        public SubjectService(ISubjectRepository subjectRepository)
        {
            this.subjectRepository = subjectRepository;
        }

        public List<GeneralSelectionItem> GetAllSubjectsAsSelectionItem()
        {
            var subjects = subjectRepository.FindAll();
            List<GeneralSelectionItem> subjectDTOs = new List<GeneralSelectionItem>();
            subjects.ToList().ForEach(x => subjectDTOs.Add(MapSubjects.MapSubjectToGeneralSelectionItem(x)));
            return subjectDTOs;
        }
    }
}
