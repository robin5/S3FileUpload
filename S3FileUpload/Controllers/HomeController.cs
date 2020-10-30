﻿// **********************************************************************************
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
using S3FileUpload.Models;
using S3FileUpload.Util;
using Amazon.S3;

namespace S3FileUpload.Controllers
{
    public class HomeController : Controller
    {
        private const string UPLOAD_DIRECTORY = "upload";

        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IS3BucketSettings _s3BucketSettings;
        private readonly ISendGridSettings _sendGridSettings;

        public HomeController(
            ILogger<HomeController> logger,
            IWebHostEnvironment env,
            IS3BucketSettings s3BucketSettings,
            ISendGridSettings sendGridSettings
            )
        {
            _logger = logger;
            _env = env;
            _s3BucketSettings = s3BucketSettings;
            _sendGridSettings = sendGridSettings;
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
                // Generate a unique filename and key for the file
                var key = S3KeyGenerator.Create(model.file.FileName);

                // Generate path to where file will be temporarily stored locally
                string filePath = Path.Combine(_env.WebRootPath, UPLOAD_DIRECTORY, key);

                try
                {
                    // Copy file to local storage
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await model.file.CopyToAsync(fileStream);
                    }

                    // Upload file to S3 bucket and obtain presigned URL for the file
                    string presignedURL = await S3Uploader.UploadFileAsync(filePath, key, _s3BucketSettings);

                    // Send email with URL to user
                    await Mailer.Send(model.email, presignedURL, _sendGridSettings);

                    return RedirectToAction("Success", "Home", new { presigned = presignedURL });
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
                finally
                {
                    // 5. Delete file
                    FileInfo fileInfo = null;
                    if (null != filePath)
                        if (null != (fileInfo = new FileInfo(filePath)))
                            fileInfo.Delete();
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
