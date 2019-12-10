using Moq;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services;
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
        public void GetAllSubjectsAsSelectionItem_WhenCalled_ReturnsSelectionItemList()
        {
            //Arrange

            //Act
            var result = _subjectService.GetAllSubjectsAsSelectionItem();

            //Assert
            Assert.IsType<List<GeneralSelectionItem>>(result);
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
        public void GetSubjectById_WithNewGUID_ReturnsNull()
        {
            //Arrange
            string guid = Guid.NewGuid().ToString();
            _subjectRepository.Setup(x => x.GetSubjectById(It.IsAny<string>())).Returns((Subject) null);

            //Act
            var result = _subjectService.GetSubjectById(guid);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateSubject_ExistingSubjectUpdate_Updates()
        {
            //Arrange
            string guid = Guid.NewGuid().ToString();
            SubjectDTO subToUpdate = new SubjectDTO { Credits = 1, Name = "Subject", SubjectCode = "SubjectCode", SubjectID = guid, SubjectType = 0, SuggestedSemester = 1 };
            SubjectDTO subExisting = new SubjectDTO { Credits = 1, Name = "SubjectUpdated", SubjectCode = "SubjectCode", SubjectID = guid, SubjectType = 0, SuggestedSemester = 1 };
            _subjectRepository.Setup


            //Act
            _subjectService.UpdateSubject(sub);

            //Assert

        }


        [Fact]
        public void UpdateSubject_NonExistingSubjectUpdate_ThrowsException()
        {
            //Arrange
            string guid = Guid.NewGuid().ToString();
            SubjectDTO sub = new SubjectDTO { Credits = 1, Name = "Subject", SubjectCode = "SubjectCode", SubjectID = guid, SubjectType = 0, SuggestedSemester = 1 };

            //Act
            _subjectService.UpdateSubject(sub);

            //Assert

        }

    }
}
