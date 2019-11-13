using StudyGroups.Contracts.Logic;
using StudyGroups.DataAccessLayer.DAOs;
using StudyGroups.WebAPI.Models;
using System;
using System.IO;
using System.Net.Http.Headers;


namespace StudyGroups.WebAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {


        public void RegisterUser(StudentRegistrationDTO studentRegistrationDTO)
        {
            var student = new Student();
            
            if (studentRegistrationDTO.Image != null)
            {
                var imgFile = studentRegistrationDTO.Image;
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (imgFile.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(imgFile.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        imgFile.CopyTo(stream);
                    }

                }
            }
        }


    }
}
