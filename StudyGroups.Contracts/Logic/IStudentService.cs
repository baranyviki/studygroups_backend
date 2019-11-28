using StudyGroups.DTOmodels;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Contracts.Logic
{
    public interface IStudentService
    {
        List<StudentListItemDTO> GetStudentsAttendedToSubject(string subjectID, string semester);
        List<StudentListItemDTO> GetStudentsAttendedToSubjectWithGrade(string subjectID, string semester, int grade);



    }
}
