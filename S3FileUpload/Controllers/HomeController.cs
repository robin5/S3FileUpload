// **********************************************************************************
// * Copyright (c) 2020 Robin Murray
// **********************************************************************************
// *
// * File: HomeController.cs
// *
// * Author: Robin Murray
// *
// **********************************************************************************
// *
// * Granting License: The MIT License (MIT)
// * 
// *   Permission is hereby granted, free of charge, to any person obtaining a copy
// *   of this software and associated documentation files (the "Software"), to deal
// *   in the Software without restriction, including without limitation the rights
// *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// *   copies of the Software, and to permit persons to whom the Software is
// *   furnished to do so, subject to the following conditions:
// *   The above copyright notice and this permission notice shall be included in
// *   all copies or substantial portions of the Software.
// *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// *   THE SOFTWARE.
// * 
// **********************************************************************************

using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using S3FileUpload.Services;
using S3FileUpload.Models;
using S3FileUpload.Util;
using Amazon.S3;

namespace S3FileUpload.Controllers
{
    public class HomeController : Controller
    {
        private const double PRESIGNED_URL_EXPIRATION_MINUTES = 5.0;

        private readonly ILogger<HomeController> _logger;
        private readonly IS3FileUploadService _s3FileUploadService;
        private readonly IMailService _mailService;
        private readonly IMailSettings _mailSettings;

        public HomeController(
            ILogger<HomeController> logger,
            IS3FileUploadService s3FileUploadService,
            IMailService mailService,
            IMailSettings mailSettings
            )
        {
            _logger = logger;
            _s3FileUploadService = s3FileUploadService;
            _mailService = mailService;
            _mailSettings = mailSettings;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (Stream stream = model.file.OpenReadStream())
                    {
                        // Create expiration date and time for the presigned URL
                        DateTime urlExpires = DateTime.UtcNow.AddMinutes(PRESIGNED_URL_EXPIRATION_MINUTES);

                        // Upload file and get presigned URL
                        string presignedURL = await _s3FileUploadService.UploadFileAsync(
                            stream, model.file.FileName, urlExpires);

                        // If presigned URL was generated, send email to user
                        if (!string.IsNullOrEmpty(presignedURL))
                        {
                            // Generate the email containing the presigned URL and expiration time
                            IMail mail = new Mail(
                                _mailSettings,
                                model.email,
                                model.file.FileName,
                                presignedURL,
                                urlExpires);

                            // Send email to user
                            await _mailService.Send(mail);

                            // Go to success page with presigned URL and filename of file
                            return RedirectToAction("Success", "Home", 
                                new { presigned = presignedURL, filename = model.file.FileName });
                        }
                    }

                    return RedirectToAction("Error", new { message = "Unable to create pre-signed URL!" });
                }
                catch (AmazonS3Exception ex)
                {
                    _logger.LogError("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                    return RedirectToAction("Error", new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unknown encountered on server: '{0}'", ex.Message);
                    return RedirectToAction("Error", new { message = ex.Message });
                }
            }
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
