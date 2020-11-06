// **********************************************************************************
// * Copyright (c) 2020 Robin Murray
// **********************************************************************************
// *
// * File: Mail.cs
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

namespace S3FileUpload.Services
{
    public class Mail : IMail
    {
        private const string EMAIL_SUBJECT = "S3FileUpload Pre-signed Key";
        private const string TO_NAME = "User";
        public Mail(IMailSettings mailSettings,
            string toAddress,
            string fileName, 
            string presignedURL, 
            DateTime urlExpires)
        {
            FromAddress = mailSettings.FromAddress;
            FromName = mailSettings.FromName;
            ToAddress = toAddress;
            ToName = TO_NAME;
            Subject = EMAIL_SUBJECT;
            FileName = fileName;
            PresignedURL = presignedURL;
            URLExpires = urlExpires;
        }
        public string FromAddress { get; }
        public string FromName { get; }
        public string ToAddress { set;  get; }
        public string ToName { get; }
        public string Subject { get; }
        public string FileName { get; }
        public string PresignedURL { get; }
        public DateTime URLExpires { get; }

        /// <summary>
        /// Creates the plain text version of the content
        /// </summary>
        /// <param name="fileName">File that was uploaded to S3 bucket</param>
        /// <param name="presignedURL">Presigned URL of file uploaded to S3 bucket</param>
        /// <returns></returns>
        public string PlainTextContent 
        { 
            get 
            {  
                return $"Here is the pre-signed URL for your file {FileName}: \n\n{PresignedURL}\n\n(note: this URL expires at {URLExpires}";
            }
        }
        /// <summary>
        /// Creates the HTML version of the content
        /// </summary>
        /// <param name="fileName">File that was uploaded to S3 bucket</param>
        /// <param name="presignedURL">Presigned URL of file uploaded to S3 bucket</param>
        /// <returns></returns>
        public string HtmlContent
        {
            get
            {
                return $"<p>Here is the pre-signed URL: <a href=\"{PresignedURL}\">{FileName}</a><br /><br /><em>(note: this URL expires at {URLExpires.ToString()})</em></p>";
            }
        }
    }
}
