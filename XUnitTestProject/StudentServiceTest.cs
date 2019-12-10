using Moq;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Services;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Xunit;

namespace XUnitTestProject
{
    public class StudentServiceTest
    {
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly Mock<ISubjectRepository> _subjectRepository;
        private readonly StudentService _studentService;

        public StudentServiceTest()
        {
            _studentRepository = new Mock<IStudentRepository>();
            _subjectRepository = new Mock<ISubjectRepository>();
            _studentService = new StudentService(_studentRepository.Object, _subjectRepository.Object);
        }

        [Fact]
        public void GetStudentsTutoringSubject_NullsPassed_ThrowsParameterException()
        {
            string subjectId = null;
            string loggedInUserId = null;

            Assert.Throws<ParameterException>(() => _studentService.GetStudentsTutoringSubject(subjectId, loggedInUserId));
        }

        [Fact]
        public void GetStudentsTutoringSubject_InvalidIdsPassed_ThrowsParameterException()
        {
            string subjectId = "some string value";
            string loggedInUserId = "some string value";

            Assert.Throws<ParameterException>(() => _studentService.GetStudentsTutoringSubject(subjectId, loggedInUserId));
        }

        [Fact]
        public void GetStudentsTutoringSubject_ValidParamsPassed_NotContainsLoggedInUser()
        {
            string subjectId = Guid.NewGuid().ToString();
            string userId = Guid.NewGuid().ToString();
            var students = CreateRandomStudents();
            students.Append(
            new Student { Email = "emial@stud.com", FirstName = "John", LastName = "Doe", GenderType = 1, ImagePath = null, NeptunCode = "dfssbc", Password = "somehash", InstagramName = "Inst", MessengerName = "messenger", UserID = userId, UserName = "johndoe" });
            _studentRepository.Setup(x => x.GetStudentsTutoringSubjectByID(It.IsAny<string>())).Returns(students);

            var res = _studentService.GetStudentsTutoringSubject(subjectId, userId);

            Assert.DoesNotContain(userId, res.Select(x => x.Id));
        }


        [Fact]
        public void GetStudentsTutoringSubject_ValidParamsPassed_ReturnsExactNumberOfSubjects()
        {
            string subjectId = Guid.NewGuid().ToString();
            string userId = Guid.NewGuid().ToString();
            var students = CreateRandomStudents();
            _studentRepository.Setup(x => x.GetStudentsTutoringSubjectByID(It.IsAny<string>())).Returns(students);

            var res = _studentService.GetStudentsTutoringSubject(subjectId, userId);

            Assert.Equal(students.Count(), res.Count());
        }

        [Fact]
        public void GetStudentsAttendedToSubject_InvalidSemesterPassed_ThrowsParameterException()
        {
            string subId = Guid.NewGuid().ToString();
            string semester = "2018/2019/5";

            Assert.Throws<ParameterException>(() => _studentService.GetStudentsAttendedToSubject(subId, semester));
        }

        [Fact]
        public void GetStudentsAttendedToSubject_NullsPassed_ThrowsParameterException()
        {
            string subId = null;
            string semester = null;

            Assert.Throws<ParameterException>(() => _studentService.GetStudentsAttendedToSubject(subId, semester));
        }

        [Fact]
        public void GetStudentsAttendedToSubject_InvalidGuidPassed_ThrowsParameterException()
        {
            string subId = Guid.NewGuid().ToString();
            string semester = Guid.NewGuid().ToString() + "some value";

            Assert.Throws<ParameterException>(() => _studentService.GetStudentsAttendedToSubject(subId, semester));
        }

        [Fact]
        public void GetStudentsAttendedToSubject_ValidParamsPassed_ReturnExactCount()
        {
            string subId = Guid.NewGuid().ToString();
            string semester = "2016/17/2";

            var students = CreateRandomStudents();
            _studentRepository.Setup(x => x.GetStudentsAttendedToSubject(It.IsAny<string>(), It.IsAny<string>())).Returns(students);

            var res = _studentService.GetStudentsAttendedToSubject(subId, semester);

            Assert.Equal(students.Count(), res.Count());
        }

        [Fact]
        public void GetStudentsAttendedToSubject_ValidParamsPassed_ReturnEmptyResult()
        {
            string subId = Guid.NewGuid().ToString();
            string semester = "2017/18/1";

            _studentRepository.Setup(x => x.GetStudentsAttendedToSubject(It.IsAny<string>(), It.IsAny<string>())).Returns((IEnumerable<Student>)null);

            var res = _studentService.GetStudentsAttendedToSubject(subId, semester);

            Assert.Empty(res);
        }

        [Fact]
        public void GetStudentDetails_NullPassed_ThrowsParameterException()
        {
            string userID = null;

            Assert.Throws<ParameterException>(() => _studentService.GetStudentDetails(userID));
        }

        [Fact]
        public void GetStudentDetails_InvalidGuidPassed_ThrowsParameterException()
        {
            string userID = Guid.NewGuid().ToString() + "invalid";

            Assert.Throws<ParameterException>(() => _studentService.GetStudentDetails(userID));
        }

        [Fact]
        public void GetStudentDetails_UserNotFound_ThrowsAuthenticationException()
        {
            string userID = Guid.NewGuid().ToString();
            _studentRepository.Setup(x => x.FindStudentByUserID(It.IsAny<string>())).Returns((Student)null);

            Assert.Throws<AuthenticationException>(() => _studentService.GetStudentDetails(userID));
        }

        [Fact]
        public void GetStudentDetails_UserExists_ReturnsUserWithTutoredSubjects()
        {
            string userID = Guid.NewGuid().ToString();
            var subjects = CreateRandomSubjects();
            _studentRepository.Setup(x => x.FindStudentByUserID(It.IsAny<string>())).Returns(new Student { FirstName = "Jane", UserID = userID, UserName = "jan4" });
            _subjectRepository.Setup(x => x.GetSubjectsStudentIsTutoring(It.IsAny<string>())).Returns(subjects);

            var res = _studentService.GetStudentDetails(userID);
            Assert.NotNull(res.TutoringSubjects);
            Assert.Equal(res.TutoringSubjects.Count(), subjects.Count());
            Assert.All(res.TutoringSubjects, x => { _ = x.SubjectID != null; });
        }


        [Fact]
        public void GetStudentDetails_UserExists_ReturnsUserWithAllDetails()
        {
            string userID = Guid.NewGuid().ToString();
            var student =
                new Student
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane@mail.com",
                    GenderType = 0,
                    ImagePath = "/res/img.bmp",
                    InstagramName = "jane4D",
                    MessengerName = "jane4D",
                    NeptunCode = "JANE12",
                    Password = "hashvalue",
                    UserID = userID,
                    UserName = "jan4"
                };
            var subjects = CreateRandomSubjects();
            _studentRepository.Setup(x => x.FindStudentByUserID(It.IsAny<string>())).Returns(student);
            _subjectRepository.Setup(x => x.GetSubjectsStudentIsTutoring(It.IsAny<string>())).Returns(subjects);

            var res = _studentService.GetStudentDetails(userID);
            Assert.Equal(student.FirstName, res.FirstName);
            Assert.Equal(student.LastName, res.LastName);
            Assert.Equal(student.MessengerName, res.MessengerName);
            Assert.Equal(student.InstagramName, res.InstagramName);
            Assert.Equal(student.NeptunCode, res.NeptunCode);
            Assert.Equal(student.Email, res.Email);
            Assert.Equal(student.UserName, res.UserName);
            Assert.Equal(student.GenderType, res.GenderType);
            Assert.Equal(student.ImagePath, res.ImagePath);
        }

        [Fact]
        public void GetStudentDetails_UserExistsButNotTutoring_ReturnsUserWithTutoredSubjectsNullValue()
        {
            string userID = Guid.NewGuid().ToString();
            _studentRepository.Setup(x => x.FindStudentByUserID(It.IsAny<string>())).Returns(new Student { FirstName = "Jane", UserID = userID, UserName = "jan4" });
            _subjectRepository.Setup(x => x.GetSubjectsStudentIsTutoring(It.IsAny<string>())).Returns((IEnumerable<Subject>)null);

            var res = _studentService.GetStudentDetails(userID);
            Assert.Equal("jan4", res.UserName);
            Assert.Null(res.TutoringSubjects);
        }

        [Fact]
        public void GetStudentFromStudyGroupSearch_NullUserIdPassed_ThrowsParameterException()
        {
            string userId = null;
            StudyGroupSearchDTO queryParams = null;
            Assert.Throws<ParameterException>(() => _studentService.GetStudentFromStudyGroupSearch(queryParams, userId));
        }

        [Fact]
        public void GetStudentFromStudyGroupSearch_NullQueryObjectPassed_ThrowsParameterException()
        {
            string userId = Guid.NewGuid().ToString();
            StudyGroupSearchDTO queryParams = null;
            Assert.Throws<ParameterException>(() => _studentService.GetStudentFromStudyGroupSearch(queryParams, userId));
        }

        [Fact]
        public void GetStudentFromStudyGroupSearch_ParamObjectWithCourseIDNullValuePassed_ThrowsParameterException()
        {
            string userId = Guid.NewGuid().ToString();
            StudyGroupSearchDTO queryParams = new StudyGroupSearchDTO { CourseID=null,IsHavingOtherCourseInCommonCurrently=true,IsSameCompletedSemesters=true,IsSameGradeAverage=true};

            Assert.Throws<ParameterException>(() => _studentService.GetStudentFromStudyGroupSearch(queryParams, userId));
        }

        [Fact]
        public void GetStudentFromStudyGroupSearch_HavingAnotherCommonCourse_ReturnsRightStudents()
        {
            string userId = Guid.NewGuid().ToString();
            StudyGroupSearchDTO queryParams = new StudyGroupSearchDTO { CourseID = Guid.NewGuid().ToString(), IsHavingOtherCourseInCommonCurrently = true };

            var students = CreateRandomStudents();
            _studentRepository.Setup(x => x.GetStudentsHavingCommonPracticalCoursesInCurrentSemester(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(students);

            var res = _studentService.GetStudentFromStudyGroupSearch(queryParams, userId);

            Assert.Equal(students.Count(), res.Count());
        }


        [Fact]
        public void GetStudentFromStudyGroupSearch_QueryParamsAllFalse_ReturnsRightStudents()
        {
            string userId = Guid.NewGuid().ToString();
            StudyGroupSearchDTO queryParams = new StudyGroupSearchDTO { CourseID = Guid.NewGuid().ToString() };

            var students = CreateRandomStudents();
            _studentRepository.Setup(x => x.GetStudentsAttendingToCourseInCurrentSemester(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(students);

            var res = _studentService.GetStudentFromStudyGroupSearch(queryParams, userId);

            Assert.Equal(students.Count(), res.Count());
        }


        [Fact]
        public void GetStudentFromStudyGroupSearch_HavingSameGradeAverage_ReturnsRightStudents()
        {

            string userId = Guid.NewGuid().ToString();
            string anotheruserId = Guid.NewGuid().ToString();
            StudyGroupSearchDTO queryParams = new StudyGroupSearchDTO { CourseID = Guid.NewGuid().ToString(), IsSameGradeAverage=true };
            var students = CreateRandomStudents().Append(new Student { UserID = anotheruserId});
            
            _studentRepository.Setup(x => x.GetStudentsAttendingToCourseInCurrentSemester(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(students);
            _studentRepository.Setup(x => x.GetStudentGradeAverage(It.IsAny<string>())).Returns<string>((y) => {
                if ((string)y == userId)
                    return 5.0;
                else if ((string)y == anotheruserId)
                    return 4.7;
                else
                    return 2.0;
            });
            
            var res = _studentService.GetStudentFromStudyGroupSearch(queryParams, userId);
            Assert.Single(res);
        }
        [Fact]
        public void GetStudentFromStudyGroupSearch_HavingSameGradeAverage_ReturnsNone()
        {

            string userId = Guid.NewGuid().ToString();
            string anotheruserId = Guid.NewGuid().ToString();
            StudyGroupSearchDTO queryParams = new StudyGroupSearchDTO { CourseID = Guid.NewGuid().ToString(), IsSameGradeAverage = true };
            var students = CreateRandomStudents().Append(new Student { UserID = anotheruserId });

            _studentRepository.Setup(x => x.GetStudentsAttendingToCourseInCurrentSemester(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(students);
            _studentRepository.Setup(x => x.GetStudentGradeAverage(It.IsAny<string>())).Returns<string>((y) => {
                if ((string)y == userId)
                    return 2.0;
                else if ((string)y == anotheruserId)
                    return 5.0;
                else
                    return 3.5;
            });

            var res = _studentService.GetStudentFromStudyGroupSearch(queryParams, userId);
            Assert.Empty(res);
        }


        [Fact]
        public void GetStudentFromStudyGroupSearch_HavingSameGradeAverageAndSameCompletedSemesters_ReturnsRightStudents()
        { 
            string userId = Guid.NewGuid().ToString();
            string anotheruserIdWithSameGradeAndSemesters = Guid.NewGuid().ToString();
            string anotheruserIdWithSameGrade = Guid.NewGuid().ToString();

            StudyGroupSearchDTO queryParams = new StudyGroupSearchDTO { CourseID = Guid.NewGuid().ToString(), IsSameGradeAverage = true, IsSameCompletedSemesters=true };
            var students = CreateRandomStudents()
                .Append(new Student { UserID = anotheruserIdWithSameGradeAndSemesters })
                .Append(new Student {UserID = anotheruserIdWithSameGrade });

            _studentRepository.Setup(x => x.GetStudentsAttendingToCourseInCurrentSemester(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(students);
            _studentRepository.Setup(x => x.GetStudentGradeAverage(It.IsAny<string>())).Returns<string>((y) => {
                if ((string)y == userId)
                    return 5.0;
                else if ((string)y == anotheruserIdWithSameGradeAndSemesters || (string)y == anotheruserIdWithSameGrade)
                    return 4.7;
                else
                    return 2.0;
            });
       
            _studentRepository.Setup(x => x.GetStudentSemesterCount(It.IsAny<string>())).Returns<string>((y) => {
                if ((string)y == userId)
                    return 5;
                else if ((string)y == anotheruserIdWithSameGradeAndSemesters)
                    return 4;
                else
                    return 2;
            });

            var res = _studentService.GetStudentFromStudyGroupSearch(queryParams, userId);
            Assert.Single(res);
            Assert.Contains(anotheruserIdWithSameGradeAndSemesters, res.Select(x => x.Id));
        }

        [Fact]
        public void GetStudentFromStudyGroupSearch_HavingSameCompletedSemesters_ReturnsRightStudents()
        {
            string userId = Guid.NewGuid().ToString();
            string anotheruserId1 = Guid.NewGuid().ToString();
            string anotheruserId2= Guid.NewGuid().ToString();
            
            StudyGroupSearchDTO queryParams = new StudyGroupSearchDTO { CourseID = Guid.NewGuid().ToString(),IsSameCompletedSemesters = true };
            var students = CreateRandomStudents()
                .Append(new Student { UserID = anotheruserId1 })
                .Append(new Student { UserID = anotheruserId2 });

            _studentRepository.Setup(x => x.GetStudentsAttendingToCourseInCurrentSemester(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(students);
           

            _studentRepository.Setup(x => x.GetStudentSemesterCount(It.IsAny<string>())).Returns<string>((y) => {
                if ((string)y == userId)
                    return 4;
                else if ((string)y == anotheruserId1 || (string)y == anotheruserId2)
                    return 5;
                else
                    return 2;
            });

            var res = _studentService.GetStudentFromStudyGroupSearch(queryParams, userId);
            Assert.Equal(2, res.Count());
            Assert.Contains(anotheruserId1, res.Select(x => x.Id));
            Assert.Contains(anotheruserId2, res.Select(x => x.Id));

        }


        [Fact]
        public void GetStudentFromStudyGroupSearch_HavingCommonCourseAndSameCompletedSemesters_ReturnsRightStudents()
        {
            string userId = Guid.NewGuid().ToString();
            string userSameCourseAndSemesters = Guid.NewGuid().ToString();
            string userSameSemesters = Guid.NewGuid().ToString();

            StudyGroupSearchDTO queryParams = new StudyGroupSearchDTO { CourseID = Guid.NewGuid().ToString(), IsSameCompletedSemesters = true, IsHavingOtherCourseInCommonCurrently=true };
            var students = CreateRandomStudents()
                .Append(new Student { UserID = userSameCourseAndSemesters });

            _studentRepository.Setup(x => x.GetStudentsHavingCommonPracticalCoursesInCurrentSemester(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(students);

            _studentRepository.Setup(x => x.GetStudentSemesterCount(It.IsAny<string>())).Returns<string>((y) => {
                if ((string)y == userId)
                    return 4;
                else if ((string)y == userSameCourseAndSemesters || (string)y == userSameSemesters)
                    return 5;
                else
                    return 2;
            });

            var res = _studentService.GetStudentFromStudyGroupSearch(queryParams, userId);
            
            Assert.Single(res);
            Assert.Contains(userSameCourseAndSemesters, res.Select(x => x.Id));
            Assert.DoesNotContain(userSameSemesters, res.Select(x => x.Id));
        }

        
        //private IEnumerable<Course> CreateRandomCourses()
        //{
        //    return new List<Course> {
        //        new Course{CourseCode="LA_1",CourseID=Guid.NewGuid().ToString(),CourseType=1,Semester="2017/18/1" },
        //        new Course{CourseCode="LA_2",CourseID=Guid.NewGuid().ToString(),CourseType=1,Semester="2018/19/1" },
        //        new Course{CourseCode="GY_3",CourseID=Guid.NewGuid().ToString(),CourseType=2,Semester="2018/19/2" },
        //        new Course{CourseCode="EA_1",CourseID=Guid.NewGuid().ToString(),CourseType=0,Semester="2018/19/2" },
        //        new Course{CourseCode="EA",CourseID=Guid.NewGuid().ToString(),CourseType=0,Semester="2019/20/1" },
        //     }.AsEnumerable();
        //}

        private IEnumerable<Subject> CreateRandomSubjects()
        {
            return new List<Subject> {
                new Subject {Name="Subject1",Credits=3,SubjectCode="SUBJCODE1",SubjectID=Guid.NewGuid().ToString(),SubjectType=2,SuggestedSemester =2},
                 new Subject {Name="Subject2",Credits=3,SubjectCode="SUBJCODE2",SubjectID=Guid.NewGuid().ToString(),SubjectType=3,SuggestedSemester =4 },
                 new Subject {Name="Subject3",Credits=3,SubjectCode="SUBJCODE3",SubjectID=Guid.NewGuid().ToString(),SubjectType=1,SuggestedSemester =1 },
                 new Subject {Name="Subject4",Credits=3,SubjectCode="SUBJCODE4",SubjectID=Guid.NewGuid().ToString(),SubjectType=0,SuggestedSemester =3 },
            }.AsEnumerable();
        }

        private IEnumerable<Student> CreateRandomStudents()
        {
            List<Student> students = new List<Student> {
            new Student{Email="emial@stud.com",FirstName="John",LastName="Doe",GenderType=1,ImagePath=null,NeptunCode="dfssbc",Password="somehash",InstagramName="Inst",MessengerName="messenger",UserID=Guid.NewGuid().ToString(),UserName="johndoe"},
            new Student{Email="emial@stud.com",FirstName="Jane",LastName="Doe",GenderType=0,ImagePath="/Res/jpg.jpg",NeptunCode="NEPTUN54",Password="hashing",InstagramName="Inst",MessengerName="messenger",UserID=Guid.NewGuid().ToString(),UserName="janedoe"},
            new Student{Email="emial@stud.com",FirstName="Little",LastName="Doe",GenderType=1,ImagePath=null,NeptunCode="dfssbc",Password="somehash",InstagramName="Inst",MessengerName="messenger",UserID=Guid.NewGuid().ToString(),UserName="littledoe"}
            };

            return students.AsEnumerable();
        }


    }
}
