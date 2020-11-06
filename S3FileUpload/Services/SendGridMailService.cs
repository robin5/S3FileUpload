// **********************************************************************************
// * Copyright (c) 2020 Robin Murray
// **********************************************************************************
// *
// * File: SendGridMailService.cs
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
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace S3FileUpload.Services
{
    /// <summary>
    /// This class implements functionality to send a presigned URL to a given email address
    /// </summary>
    public class SendGridMailService : IMailService
    {
        private const string EMAIL_SUBJECT = "S3FileUpload Pre-signed Key";

        private ISendGridSettings _settings;
        public SendGridMailService(ISendGridSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Sends an email to the given email address
        /// </summary>
        /// <param name="mail">email to send</param>
        /// <returns></returns>
        public async Task Send(IMail mail)
        {
            var client = new SendGridClient(_settings.Key);

            var email = MailHelper.CreateSingleEmail(
                new EmailAddress(mail.FromAddress, mail.FromName),
                new EmailAddress(mail.ToAddress, mail.ToName),
                mail.Subject, 
                mail.PlainTextContent, 
                mail.HtmlContent
                );

            var response = await client.SendEmailAsync(email);

            Console.WriteLine($"Response: {response.StatusCode}");
        }
    }
}
