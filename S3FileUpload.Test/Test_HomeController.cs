using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using S3FileUpload.Controllers;
using S3FileUpload.Models;
using S3FileUpload.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace S3FileUpload.Test
{
    public class Test_HomeController
    {
        private readonly ILogger<HomeController> _logger = null;
        private readonly string _testFilesDir;

        private const string FILE_10_MB = "10MB_FILE";
        private const string FILE_10_MB_PLUS_1 = "10MB_FILE_PLUS_ONE";
        public Test_HomeController()
        {
            // the type specified here is just so the secrets library can 
            // find the UserSecretId we added in the csproj file
            var builder = new ConfigurationBuilder().AddUserSecrets<Test_HomeController>();
            var configuration = builder.Build();

            _testFilesDir = configuration["TestFilesDir"];
        }

        [Fact]
        public void Get_Index()
        {
            // Arrange

            var homeController = new HomeController(null, null, null, null);

            // Act

            var result = homeController.Index();

            // Assert

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Post_Index_InvalidModel()
        {
            // Arrange -------------------------------------------------

            var homeController = new HomeController(
                null,
                null,
                null,
                null);

            homeController.ModelState.AddModelError("SessionName", "Required");

            // Act ------------------------------------------------------

            Task<IActionResult> task = homeController.Index(null);
            task.Wait();
            var result = task.Result;

            // Assert -----------------------------------------------------

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Post_Index()
        {
            const string PRESIGNED_URL = "this is the presigned url";

            // Arrange -------------------------------------------------

            // Create a mock data file stream with a specific size of 10MB
            var uploadFile = new FileStream(
                Path.Combine(_testFilesDir, FILE_10_MB), 
                FileMode.Open, 
                FileAccess.Read);

            // Create mock for S3FileUploadService
            var mS3FileUploadService = new Mock<IS3FileUploadService>();
            mS3FileUploadService
                .Setup(n => n.UploadFileAsync(uploadFile, FILE_10_MB, It.IsAny<DateTime>()))
                .Returns(Task.FromResult<string>(PRESIGNED_URL));

            // Create mock for MailService
            var mMailService = new Mock<IMailService>();
            mMailService.Setup(n => n.Send(
                new Mail(
                    new MailSettings("NoReply@robin5.net", "S3FileUpload"),
                    "ToAddress@somewhere.com",
                    "filename","presignedurl", new DateTime())));

            // Instantiate HomeController
            var homeController = new HomeController(
                _logger,
                mS3FileUploadService.Object, 
                mMailService.Object,
                new MailSettings("ToAddress@somewhere.com", "FromName"));

            // Create 
            var mIFormFile = new Mock<IFormFile>();
            mIFormFile.Setup(n => n.FileName).Returns(FILE_10_MB);
            mIFormFile.Setup(n => n.OpenReadStream()).Returns(() => uploadFile);

            // Create Model for HomeController's Index action
            var model = new HomeViewModel() {
                email = "someone@somwhere.com",
                file = mIFormFile.Object
            };

            // Act ------------------------------------------------------

            Task<IActionResult> task = homeController.Index(model);
            task.Wait();
            var result = task.Result;

            // Assert -----------------------------------------------------

            mS3FileUploadService.Verify(s => s.UploadFileAsync(uploadFile, FILE_10_MB, It.IsAny<DateTime>()));

            mMailService.Verify(s => s.Send(It.IsAny<Mail>()));

            Assert.IsType<RedirectToActionResult>(result);

            // Verify result is a redirect to the success 
            // page with the proper query variables
            var redirect = result as RedirectToActionResult;

            Assert.Equal("Success", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
            Assert.Contains<string>("presigned", redirect.RouteValues.Keys);
            Assert.Contains<string>("filename", redirect.RouteValues.Keys);
            Assert.Equal(PRESIGNED_URL, redirect.RouteValues["presigned"]);
            Assert.Equal(FILE_10_MB, redirect.RouteValues["filename"]);
        }

        [Fact]
        public void Get_Privacy()
        {
            // Arrange

            var homeController = new HomeController(null, null, null, null);

            // Act

            var result = homeController.Privacy();

            // Assert

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Get_Success()
        {
            // Arrange

            var homeController = new HomeController(null, null, null, null);

            // Act

            var result = homeController.Success();

            // Assert

            Assert.IsType<ViewResult>(result);
        }
    }
}
