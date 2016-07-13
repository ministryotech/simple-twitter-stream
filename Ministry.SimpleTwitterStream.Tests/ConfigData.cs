using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
