// **********************************************************************************
// * Copyright (c) 2020 Robin Murray
// **********************************************************************************
// *
// * File: S3Uploader.cs
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

namespace S3FileUpload.Util
{
    /// <summary>
    /// This class implements functionality to upload a file to an S3 bucket and return a presigned URL to that file
    /// </summary>
    public class S3Uploader
    {
        /// <summary>
        /// Use S3 client to upload file to s3 bucket
        /// </summary>
        /// <param name="key">The name of the object created in the S3 bucket</param>
        /// <param name="filePath">The location of the object to be uploaded to S3</param>
        /// <returns>The presigned URL used to view the uploaded file</returns>
        public static async Task<string> UploadFileAsync(string filePath, string key, IS3BucketSettings s3BucketSettings)
        {
            IAmazonS3 s3Client = new AmazonS3Client(s3BucketSettings.BucketRegion);
            var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.UploadAsync(filePath, s3BucketSettings.BucketName);

            // Generate a pre-signed url for the file
            string presignedURL = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = s3BucketSettings.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(5.0)
            });

            return presignedURL;
        }
    }
}
