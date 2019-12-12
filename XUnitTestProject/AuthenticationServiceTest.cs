using CryptoHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services;
using StudyGroups.WebAPI.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Text;
using Xunit;

namespace XUnitTestProject
{
    public class AuthenticationServiceTest
    {
        private readonly AuthenticationService _authenticationService;

        private readonly Mock<ICourseRepository> _courseRepository;
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly Mock<ISubjectRepository> _subjectRepository;
        private readonly Mock<ITeacherRepository> _teacherRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly IConfiguration _config;
        private readonly Mock<ILogger<AuthenticationService>> _logger;


        public AuthenticationServiceTest()
        {
            _userRepository = new Mock<IUserRepository>();
            var myconfig = new Dictionary<string, string>
                {
                    {"JWTSecretKey", "JWTSecretKey_FOR_TEST"},
                    {"ValidClientURI","localhost://test"}
                };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myconfig)
                .Build();

            _config = configuration;
            _courseRepository = new Mock<ICourseRepository>();
            _logger = new Mock<ILogger<AuthenticationService>>();
            _studentRepository = new Mock<IStudentRepository>();
            _subjectRepository = new Mock<ISubjectRepository>();
            _teacherRepository = new Mock<ITeacherRepository>();

            _authenticationService = new AuthenticationService(_courseRepository.Object, _studentRepository.Object, _subjectRepository.Object, _teacherRepository.Object,
                _userRepository.Object, _config, _logger.Object);
        }

        [Fact]
        public void Login_NullValuesPassed_ThrowsParameterException()
        {

            LoginDTO loginCredentials = new LoginDTO();
            Assert.Throws<ParameterException>(() => _authenticationService.Login(loginCredentials));
        }

        [Fact]
        public void Login_UserNameNotInDB_ThrowsAuthenticationException()
        {
            LoginDTO loginCredentials = new LoginDTO { UserName = "janedoe", Password = "password" };
            _userRepository.Setup(x => x.FindUserByUserName(It.IsAny<string>())).Returns((User)null);
            Assert.Throws<AuthenticationException>(() => _authenticationService.Login(loginCredentials));
        }

        [Fact]
        public void Login_UserNameGoodPasswordWrong_ThrowsAuthenticationException()
        {
            LoginDTO loginCredentials = new LoginDTO { UserName = "janedoe", Password = "password" };
            string pw = Crypto.HashPassword("dragon");
            _userRepository.Setup(x => x.FindUserByUserName(It.IsAny<string>()))
                .Returns(new User { UserID = "guid", UserName = loginCredentials.UserName, Password = pw });

            Assert.Throws<AuthenticationException>(() => _authenticationService.Login(loginCredentials));
        }
        [Fact]
        public void Login_UserNameGoodPasswordGood_ReturnsGoodToken()
        {
            LoginDTO loginCredentials = new LoginDTO { UserName = "janedoe", Password = "password" };
            string pw = Crypto.HashPassword("password");
            _userRepository.Setup(x => x.FindUserByUserName(It.IsAny<string>()))
                .Returns(new User { UserID = "guid", UserName = loginCredentials.UserName, Password = pw });
            _userRepository.Setup(x => x.GetUserLabelsByUserID(It.IsAny<string>())).Returns(new List<string> { "User", "Student" });


            var res = _authenticationService.Login(loginCredentials);

            JwtSecurityTokenHandler r = new JwtSecurityTokenHandler();
            var token = r.ReadToken(res);

            Assert.NotNull(res);
            Assert.IsType<JwtSecurityToken>(token);
        }

        [Fact]
        public void Login_UserNameGoodPasswordGood_ReturnsGoodTokenWithValidIssuer()
        {
            LoginDTO loginCredentials = new LoginDTO { UserName = "janedoe", Password = "password" };
            string pw = Crypto.HashPassword("password");
            _userRepository.Setup(x => x.FindUserByUserName(It.IsAny<string>()))
                .Returns(new User { UserID = "guid", UserName = loginCredentials.UserName, Password = pw });
            _userRepository.Setup(x => x.GetUserLabelsByUserID(It.IsAny<string>())).Returns(new List<string> { "User", "Student" });

            var res = _authenticationService.Login(loginCredentials);

            JwtSecurityTokenHandler r = new JwtSecurityTokenHandler();
            var token = r.ReadToken(res);

            Assert.Equal(_config["ValidClientURI"], token.Issuer);
        }
    }
}
