// Copyright (c) 2016 Minotech Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
// (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Ministry.SimpleTwitterStream.Tests
{
    /// <summary>
    /// The config data to pas to the API Integration tests to enable calling the Twitter API.
    /// </summary>
    /// <remarks>
    /// To run these you will need to set up your own app for running the tests at http://apps.twitter.com and add the details here.
    ///
    /// Do NOT commit any changes to this file.
    /// </remarks>
    internal static class ConfigData
    {
        public static string AccessToken { get { return "Valid Twitter access token"; } }
        public static string AccessTokenSecret { get { return "Valid Twitter Access Token Secret"; } }
        public static string ConsumerKey { get { return "Valid Twitter Consumer Key"; } }
        public static string ConsumerSecret { get { return "Valid Twitter Consumer Secret"; } }
    }
}
