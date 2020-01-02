using Moq;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using StudyGroups.WebAPI.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTestProject
{
    public class SubjectServiceTest
    {
        private readonly SubjectService _subjectService;
        private readonly Mock<ISubjectRepository> _subjectRepository;

        public SubjectServiceTest()
        {
            _subjectRepository = new Mock<ISubjectRepository>();
            _subjectService = new SubjectService(_subjectRepository.Object);
        }

        [Fact]
        public void GetAllSubjectsAsSelectionItem_WhenCalled_ReturnsExactNumberOfItems()
        {
            //Arrange
            IQueryable<Subject> listItems = new List<Subject> { new Subject(), new Subject() }.AsQueryable();
            _subjectRepository.Setup(x => x.FindAll()).Returns(listItems);


            //Act
            var result = _subjectService.GetAllSubjectsAsSelectionItem();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllSubjectsAsSelectionItem_WhenCalled_ReturnsSelectionItemList()
        {
            var result = _subjectService.GetAllSubjectsAsSelectionItem();

            Assert.IsType<List<GeneralSelectionItem>>(result);
        }

        [Fact]
        public void GetSubjectById_WithNewGUID_ThrowsException()
        {
            //Arrange
            string guid = Guid.NewGuid().ToString();
            _subjectRepository.Setup(x => x.GetSubjectById(It.IsAny<string>())).Returns((Subject)null);

            //Act & Assert
            Assert.Throws<ParameterException>(() => _subjectService.GetSubjectById(guid));
        }

        [Fact]
        public void UpdateSubject_ExistingSubjectUpdate_Updates()
        {
            //Arrange
            string guid = Guid.NewGuid().ToString();
            SubjectDTO subToUpdate = new SubjectDTO { Credits = 1, Name = "Subject", SubjectCode = "SubjectCode", SubjectID = guid, SubjectType = 0, SuggestedSemester = 1 };
            //SubjectDTO subExisting = new SubjectDTO { Credits = 1, Name = "SubjectUpdated", SubjectCode = "SubjectCode", SubjectID = guid, SubjectType = 0, SuggestedSemester = 1 };

            _subjectRepository.Setup(x => x.UpdateSubject(It.IsAny<Subject>())).Returns(new Subject { });
            var result = _subjectService.UpdateSubject(subToUpdate);

            //Assert
            Assert.IsType<SubjectDTO>(result);
        }


        [Fact]
        public void UpdateSubject_InvalidSubjectUpdate_ThrowsParameterException()
        {
            //Arrange
            string guid = Guid.NewGuid().ToString();
            SubjectDTO sub = new SubjectDTO { Credits = 1, Name = "Subject", SubjectCode = "SubjectCode", SubjectID = null, SubjectType = 0, SuggestedSemester = 1 };
            _subjectRepository.Setup(x => x.UpdateSubject(It.IsAny<Subject>()));

            //Act
            //Assert
            //_subjectRepository.Verify(x => x.UpdateSubject(It.IsAny<Subject>()), Times.Never);

            Assert.Throws<ParameterException>(() =>
            _subjectService.UpdateSubject(sub));
        }

        [Fact]
        public void GetSubjectUserHasPassedAsSubjectDTO_InvalidUserId_ThrowsParameterException()
        {
            string userid = null;
            Assert.Throws<ParameterException>(() => _subjectService.GetSubjectUserHasPassedAsSubjectDTO(userid));
        }


        [Fact]
        public void GetSubjectUserHasPassedAsSubjectDTO_InvalidUserId_ReturnsExactSubjectDTOList()
        {
            string userId = Guid.NewGuid().ToString();
            _subjectRepository.Setup(x => x.GetSubjectsStudentHasPassed(It.IsAny<string>())).Returns(new List<Subject> { new Subject(), new Subject() });

            var res = _subjectService.GetSubjectUserHasPassedAsSubjectDTO(userId);
            Assert.Equal(2, res.Count());
        }

    }
}
