// **********************************************************************************
// * Copyright (c) 2020 Robin Murray
// **********************************************************************************
// *
// * File: S3FileUploadService.cs
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

using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Threading.Tasks;
using Amazon.S3.Model;
using System.IO;

namespace S3FileUpload.Services
{
    /// <summary>
    /// This class implements functionality to upload a file to an S3 bucket and return a presigned URL to that file
    /// </summary>
    public class S3FileUploadService : IS3FileUploadService
    {
        private IS3BucketSettings _s3BucketSettings;
        public S3FileUploadService(IS3BucketSettings s3BucketSettings)
        {
            _s3BucketSettings = s3BucketSettings;
        }
        /// <summary>
        /// Uploads a file to the S3 bucket
        /// </summary>
        /// <param name="stream">file to upload to bucket</param>
        /// <param name="key">A unique name for the file placed into the S3 bucket</param>
        /// <param name="urlExpires">Expiration time for the pre-signed URL</param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(
            Stream stream,
            string key,
            DateTime urlExpires)
        {
            IAmazonS3 s3Client = new AmazonS3Client(_s3BucketSettings.BucketRegion);
            Console.WriteLine(s3Client.Config);
            var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.UploadAsync(stream, _s3BucketSettings.BucketName, key);

            // Generate a pre-signed url for the file
            string presignedURL = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _s3BucketSettings.BucketName,
                Key = key,
                Expires = urlExpires
            });

            return presignedURL;
        }
    }
}
