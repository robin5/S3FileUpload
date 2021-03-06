﻿// **********************************************************************************
// * Copyright (c) 2020 Robin Murray
// **********************************************************************************
// *
// * File: FormFileSizeAttribute.cs
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

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace S3FileUpload.Util
{
    /// <summary>
    /// This class validate that a file picked in a form is less than or equal to 10MB
    /// </summary>
    public class FormFileSizeAttribute : ValidationAttribute
    {
        /// <summary>
        /// IsValid is true if the file specified is less than or equal to 10MB, false otherwise
        /// </summary>
        /// <param name="value">File to be validated</param>
        /// <returns>true if the file specified is less than or equal to 10MB, false otherwise</returns>
        public override bool IsValid(object value)
        {
            IFormFile file = value as IFormFile;
            if (null != file)
            {
                return (file.Length <= 10485760);
            }
            return false;
        }
    }
}
