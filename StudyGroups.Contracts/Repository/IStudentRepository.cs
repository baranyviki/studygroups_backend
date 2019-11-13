using StudyGroups.DataAccessLayer.DAOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Contracts.Repository
{
    public interface IStudentRepository : IBaseRepository <Student>
    {
        /// <summary>
        /// Gets a list of student who are attended to this subject
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        //IEnumerable<StudentDBModel> GetStudentsAttendingToSubjectCurrently(int subjectID);
        
        /// <summary>
        /// Gets students having a specific subject in a specific semester. 
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        IEnumerable<Student> GetStudentsAttendedToSubject(int subjectID, string semester);

        /// <summary>
        /// Gets students who had been comleted a specific subject, with a better grade then specified.
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param name="semester"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        IEnumerable<Student> GetStudentsAttendedToSubjectWithGrade(int subjectID, string semester, int grade);



    }
}
