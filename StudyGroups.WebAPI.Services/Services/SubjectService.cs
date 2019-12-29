using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using StudyGroups.WebAPI.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudyGroups.WebAPI.Services.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            this._subjectRepository = subjectRepository;
        }

        public void CreateSubject(SubjectDTO subjectDTO)
        {
            if (subjectDTO == null)
                throw new ParameterException("subject object cannot be null");
            if (subjectDTO.SubjectID != null)
                throw new ParameterException("subjectID cannot have initial value");
            var sub = MapSubject.MapSubjectDTOToSubject(subjectDTO);
            _subjectRepository.Create(sub);

        }

        public IEnumerable<SubjectListItemDTO> GetAllSubjectAsSubjectListItem()
        {
            return _subjectRepository.FindAll().Select(x => MapSubject.MapSubjectToSubjectListItemDTO(x));
        }

        public List<GeneralSelectionItem> GetAllSubjectsAsSelectionItem()
        {
            var subjects = _subjectRepository.FindAll();
            List<GeneralSelectionItem> subjectDTOs = new List<GeneralSelectionItem>();
            subjects.ToList().ForEach(x => subjectDTOs.Add(MapSubject.MapSubjectToGeneralSelectionItem(x)));
            return subjectDTOs;
        }

        public SubjectDTO GetSubjectById(string subjectId)
        {
            var subject = _subjectRepository.GetSubjectById(subjectId);
            if (subject == null)
            {
                throw new ParameterException("Subject with given id does not exists.");
            }
            return MapSubject.MapSubjectToSubjectDTO(subject);
        }

        public IEnumerable<GeneralSelectionItem> GetSubjectUserHasPassedAsSubjectDTO(string userId)
        {
            if (userId == null)
            { throw new ParameterException("Requested userId cannot be null"); }
            var subjects = _subjectRepository.GetSubjectsStudentHasPassed(userId);
            var selectionItems = subjects.Select(x => MapSubject.MapSubjectToGeneralSelectionItem(x));
            return selectionItems;
        }

        public SubjectDTO UpdateSubject(SubjectDTO subject)
        {
            if (subject == null)
            {
                throw new ParameterException("Subject to be updated cannot be null");
            }
            else if (subject.SubjectID == null || String.IsNullOrEmpty(subject.SubjectCode))
            {
                throw new ParameterException("SubjectID, SubjectCode cannot be null or empty");
            }

            var subjectDBModel = MapSubject.MapSubjectDTOToSubject(subject);
            Subject updated = _subjectRepository.UpdateSubject(subjectDBModel);

            return MapSubject.MapSubjectToSubjectDTO(updated);

        }
    }
}
