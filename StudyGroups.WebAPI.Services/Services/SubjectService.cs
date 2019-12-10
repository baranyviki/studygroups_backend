﻿using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
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
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            this._subjectRepository = subjectRepository;
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

        public IEnumerable<SubjectListItemDTO> GetSubjectUserHasPassedAsSubjectDTO(string userId)
        {
            var subjects = _subjectRepository.GetSubjectsStudentHasPassed(userId);
            var selectionItems = subjects.Select(x => MapSubject.MapSubjectToSubjectListItemDTO(x));
            return selectionItems;
        }

        public void UpdateSubject(SubjectDTO subject)
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
            _subjectRepository.UpdateSubject(subjectDBModel);
        }
    }
}
