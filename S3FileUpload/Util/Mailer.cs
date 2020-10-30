// **********************************************************************************
// * Copyright (c) 2020 Robin Murray
// **********************************************************************************
// *
// * File: Mailer.cs
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

using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace S3FileUpload.Util
{
    /// <summary>
    /// This class implements functionality to send a presigned URL to a given email address
    /// </summary>
    public class Mailer
    {
        private const string UTILITY_NAME = "SAM File Upload Utility";
        private const string EMAIL_SUBJECT = "SAM FileUpload Pre-signed Key";
        /// <summary>
        /// Sends an email to the given email address
        /// </summary>
        /// <param name="emailAddress">Address to send email</param>
        /// <param name="presigned">Pre-signed URL specified in email</param>
        /// <param name="sendGridSettings">SendGrid API settings</param>
        /// <returns></returns>
        public static async Task Send(string emailAddress, string presigned, ISendGridSettings sendGridSettings)
        {
            var client = new SendGridClient(sendGridSettings.Key);
            var from = new EmailAddress(sendGridSettings.FromAddress, sendGridSettings.FromName);
            var to = new EmailAddress(emailAddress, UTILITY_NAME);
            var subject = EMAIL_SUBJECT;
            var plainTextContent = presigned;
            var htmlContent = HtmlContent(plainTextContent);

            var email = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(email);
            Console.WriteLine($"Response: {response.StatusCode}");
        }
        /// <summary>
        /// Creates an HTML version of the passed in content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string HtmlContent(string content)
        {
            // TODO: format email into an HTML document
            return "<strong>" + content + "</strong>";
        }
    }
}
