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

using LinqToTwitter;
using Ministry.SimpleTwitterStream.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Ministry.SimpleTwitterStream.Tests.Models
{
    [TestFixture]
    [Category("Builder Test")]
    public class TweetBuilderTests
    {
        private const string TestName = "Test Name";
        private const string TestHandle = "testname";
        private const string TestText = "this is the text for a test tweet";
        private const string TestAvatarUrl = "//pbs.twimg.com/profile_images/597801657871802368/ers_uX_4_normal.jpg";
        private DateTime TestCreatedDate = new DateTime(2016, 6, 12);
        private const string RetweetedFromTestName = "Retweet Author";
        private const string RetweetedFromTestHandle = "retweetauth";
        private const string RetweetText = "this is the text for a test tweet that was retweeted";
        private DateTime TestRetweetCreatedDate = new DateTime(2016, 5, 10);

        private Status statusStub;

        private TweetBuilder objUt;

        #region | Setup and TearDown |

        [SetUp]
        public void SetUp()
        {
            statusStub = new Status
            {
                Text = TestText,
                CreatedAt = TestCreatedDate,
                User = new User
                {
                    ScreenNameResponse = TestHandle,
                    Name = TestName
                }
            };

            objUt = new TweetBuilder();
        }

        [TearDown]
        public void TearDown()
        {
            objUt = null;
            statusStub = null;
        }

        #endregion

        [Test]
        public void CanBuildATweetFromALinqToTwitterStatus()
        {
            var result = objUt.Build(statusStub);

            Assert.AreEqual(TestName, result.Author);
            Assert.AreEqual(TestHandle, result.Handle);
            Assert.AreEqual(TestText, result.Text);
            Assert.AreEqual(TestCreatedDate, result.DateCreated);
        }

        [Test]
        public void AStandardTweetHasAProtocolStrippedAvatarUrl()
        {
            statusStub.User.ProfileImageUrl = "http:" + TestAvatarUrl;
            statusStub.User.ProfileImageUrlHttps = "https:" + TestAvatarUrl;
            var result = objUt.Build(statusStub);

            Assert.AreEqual(TestAvatarUrl, result.AvatarUrl);
        }

        [Test]
        public void AStandardTweetHasAFalseValueForRetweet()
        {
            var result = objUt.Build(statusStub);

            Assert.False(result.Retweet);
        }

        [Test]
        public void AStandardTweetHasNoValuesForRetweetProperties()
        {
            var result = objUt.Build(statusStub);

            Assert.False(result.DateRetweeted.HasValue);
            Assert.Null(result.RetweetedBy);
            Assert.Null(result.RetweetedByHandle);
        }

        [Test]
        public void ARetweetHasATrueValueForRetweetAndAValueForADate()
        {
            StubRetweetData();

            var result = objUt.Build(statusStub);

            Assert.True(result.Retweet, "Not marked as a Retweet");
            Assert.True(result.DateRetweeted.HasValue, "No date recorded");
        }

        [Test]
        public void ARetweetHasDataThatReflectsTheOriginalTweet()
        {
            StubRetweetData();

            var result = objUt.Build(statusStub);

            Assert.AreEqual(TestRetweetCreatedDate, result.DateCreated);
            Assert.AreEqual(RetweetedFromTestName, result.Author);
            Assert.AreEqual(RetweetedFromTestHandle, result.Handle);
            Assert.AreEqual(RetweetText, result.Text);
        }

        [Test]
        public void ARetweetHasDataThatDescribesTheRetweetItself()
        {
            StubRetweetData();

            var result = objUt.Build(statusStub);

            Assert.AreEqual(TestName, result.RetweetedBy);
            Assert.AreEqual(TestHandle, result.RetweetedByHandle);
            Assert.AreEqual(TestCreatedDate, result.DateRetweeted);
        }

        [Test]
        public void ARetweetHasAProtocolStrippedAvatarUrlFromTheOriginalTweet()
        {
            StubRetweetData();
            statusStub.RetweetedStatus.User.ProfileImageUrl = "http:" + TestAvatarUrl;
            statusStub.RetweetedStatus.User.ProfileImageUrlHttps = "https:" + TestAvatarUrl;
            var result = objUt.Build(statusStub);

            Assert.AreEqual(TestAvatarUrl, result.AvatarUrl);
        }

        [Test]
        [TestCase("@AgileCymru we all had a great time", "<a href=\"https://twitter.com/AgileCymru\" target=\"_blank\">@AgileCymru</a> we all had a great time")]
        [TestCase("I had fun with @microsoft office", "I had fun with <a href=\"https://twitter.com/microsoft\" target=\"_blank\">@microsoft</a> office")]
        [TestCase("Looking for things @ministryotech", "Looking for things <a href=\"https://twitter.com/ministryotech\" target=\"_blank\">@ministryotech</a>")]
        public void ATweetHasAMarkupTextPropertyThatConvertsAllMentionsIntoValidTwitterLinks(string text, string expectedMarkup)
        {
            statusStub.Text = text;
            var result = objUt.Build(statusStub);

            var markup = result.Markup.ToString();

            Assert.AreNotEqual(markup, result.Text);
            Assert.AreEqual(expectedMarkup, markup);
        }

        [Test]
        [TestCase("Talking @ agile conference")]
        [TestCase("@ this time")]
        [TestCase("Just an @")]
        public void ALonelyAtIsNotAMention(string text)
        {
            statusStub.Text = text;
            var result = objUt.Build(statusStub);

            Assert.AreEqual(result.Markup.ToString(), result.Text);
        }

        [Test]
        [TestCase("Some text about #agile", "Some text about <a href=\"https://twitter.com/hashtag/agile?src=hash\" target=\"_blank\">#agile</a>")]
        [TestCase("Something else #cakes like", "Something else <a href=\"https://twitter.com/hashtag/cakes?src=hash\" target=\"_blank\">#cakes</a> like")]
        [TestCase("#fish #cats #dogs", "<a href=\"https://twitter.com/hashtag/fish?src=hash\" target=\"_blank\">#fish</a> <a href=\"https://twitter.com/hashtag/cats?src=hash\" target=\"_blank\">#cats</a> <a href=\"https://twitter.com/hashtag/dogs?src=hash\" target=\"_blank\">#dogs</a>")]
        public void ATweetHasAMarkupTextPropertyThatConvertsAllHashtagsIntoValidTwitterLinks(string text, string expectedMarkup)
        {
            statusStub.Text = text;
            var result = objUt.Build(statusStub);

            var markup = result.Markup.ToString();

            Assert.AreNotEqual(markup, result.Text);
            Assert.AreEqual(expectedMarkup, markup);
        }

        [Test]
        [TestCase("Some text about # agile")]
        [TestCase("Something else# cakes like")]
        [TestCase("# fish # cats # dogs")]
        public void APrecedingHashIsNotAHashTag(string text)
        {
            statusStub.Text = text;
            var result = objUt.Build(statusStub);

            Assert.AreEqual(result.Markup.ToString(), result.Text);
        }

        [Test]
        [TestCase("Read my review of AGCYM16 now at https://t.co/ckDhj958Zk", "Read my review of AGCYM16 now at <a href=\"{0}\" target=\"_blank\">{1}</a>", "https://www.mysite.com/blog/things-about-boxes", "https://www.mysite.com/blog/th...", "https://t.co/ckDhj958Zk")]
        [TestCase("Look at https://t.co/ckDhj958Zk for more info", "Look at <a href=\"{0}\" target=\"_blank\">{1}</a> for more info", "https://www.yoursite.com/blog/things-about-boxes", "https://www.yoursite.com/blog/th...", "https://t.co/ckDhj958Zk")]
        [TestCase("http://t.co/ckDhj958Zk has what you need", "<a href=\"{0}\" target=\"_blank\">{1}</a> has what you need", "http://www.thatsite.com/blog/things-about-boxes", "http://www.thatsite.com/blog/th...", "http://t.co/ckDhj958Zk")]
        public void ATweetHasAMarkupTextPropertyThatConvertsAllUrlsToLinks(string text, string expectedMarkupFormat, string expandedUrl, string displayUrl, string twitterUrl)
        {
            statusStub.Text = text;
            statusStub.Entities = new Entities();
            statusStub.Entities.UrlEntities = new List<UrlEntity> {
                new UrlEntity { DisplayUrl = displayUrl, ExpandedUrl = expandedUrl, Url = twitterUrl }
                };

            var result = objUt.Build(statusStub);

            var markup = result.Markup.ToString();

            Assert.AreNotEqual(markup, result.Text);
            Assert.AreEqual(string.Format(expectedMarkupFormat, expandedUrl, displayUrl), markup);
        }

        [Test]
        [TestCase("Read my review of AGCYM16 now at https://t.co/ckDhj958Zk", "Read my review of AGCYM16 now at <a href=\"{0}\" target=\"_blank\">{1}</a>", "https://www.mysite.com/blog/things-about-boxes", "https://www.mysite.com/blog/th...", "https://t.co/ckDhj958Zk")]
        [TestCase("Look at https://t.co/ckDhj958Zk for more info", "Look at <a href=\"{0}\" target=\"_blank\">{1}</a> for more info", "https://www.yoursite.com/blog/things-about-boxes", "https://www.yoursite.com/blog/th...", "https://t.co/ckDhj958Zk")]
        [TestCase("http://t.co/ckDhj958Zk has what you need", "<a href=\"{0}\" target=\"_blank\">{1}</a> has what you need", "http://www.thatsite.com/blog/things-about-boxes", "http://www.thatsite.com/blog/th...", "http://t.co/ckDhj958Zk")]
        public void IfAUrlIsNotAUrlEntityThenTheOriginalLinkIsUsedWithAClickText(string text, string expectedMarkupFormat, string expandedUrl, string displayUrl, string twitterUrl)
        {
            statusStub.Text = text;
            statusStub.Entities = new Entities();
            statusStub.Entities.UrlEntities = new List<UrlEntity>();

            var result = objUt.Build(statusStub);

            var markup = result.Markup.ToString();

            Assert.AreNotEqual(markup, result.Text);
            Assert.AreEqual(string.Format(expectedMarkupFormat, twitterUrl, "(more)"), markup);
        }

        #region | Supporting Methods |

        /// <summary>
        /// Stubs the retweet data.
        /// </summary>
        private void StubRetweetData()
        {
            statusStub.Retweeted = true;
            statusStub.RetweetedStatus = new Status
            {
                ScreenName = string.Empty,
                Text = RetweetText,
                CreatedAt = TestRetweetCreatedDate,
                User = new User
                {
                    ScreenNameResponse = RetweetedFromTestHandle,
                    Name = RetweetedFromTestName
                }
            };
        }

        #endregion
    }
}
