using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Services.Mapping
{
    internal static class MapSubjects
    {
        internal static GeneralSelectionItem MapSubjectToGeneralSelectionItem(Subject subjectDBModel)
        {
            return new GeneralSelectionItem
            {
                ID = subjectDBModel.SubjectID.ToString(),
                DisplayName = $"{subjectDBModel.Name} - {subjectDBModel.SubjectCode}"
            };

        }

        
    }
}
