using StudyGroups.DTOmodels;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Contracts.Logic
{
    public interface IStudentService
    {
        List<StudentListItemDTO> GetStudentsAttendedToSubject(int subjectID, string semester);
        List<StudentListItemDTO> GetStudentsAttendedToSubjectWithGrade(int subjectID, string semester, int grade);



    }
}
