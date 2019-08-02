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

using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace Ministry.SimpleTwitterStream.NetFramework.Tests
{
    [TestFixture]
    [Category("Gateway Test")]
    public class TwitterApiGatewayTests
    {
        private Mock<ITwitterConfig> mockTwitterConfig;
        private Mock<ITwitterLocalCacheGateway> mockLocalCache;
        private Mock<IDateTimeAccessor> mockTimeProvider;

        #region | Setup and TearDown |

        [SetUp]
        public void SetUp()
        {
            mockTwitterConfig = new Mock<ITwitterConfig>();
            mockTwitterConfig.Setup(cr => cr.AccessToken).Returns(ConfigData.AccessToken);
            mockTwitterConfig.Setup(cr => cr.AccessTokenSecret).Returns(ConfigData.AccessTokenSecret);
            mockTwitterConfig.Setup(cr => cr.ConsumerKey).Returns(ConfigData.ConsumerKey);
            mockTwitterConfig.Setup(cr => cr.ConsumerSecret).Returns(ConfigData.ConsumerSecret);

            mockLocalCache = new Mock<ITwitterLocalCacheGateway>();
            mockLocalCache.SetupAllProperties();

            mockTimeProvider = new Mock<IDateTimeAccessor>();
            mockTimeProvider.Setup(tp => tp.Now).Returns(DateTime.Now);
        }

        [TearDown]
        public void TearDown()
        {
            mockTwitterConfig = null;
            mockLocalCache = null;
            mockTimeProvider = null;
        }

        #endregion

        [Test]
        [Category("API Integration")]
        public void GettingATwitterStreamWithInvalidAuthorizationWillCauseTheMethodToFallOver()
        {
            mockTwitterConfig.Setup(cr => cr.TweetCount).Returns(3).Verifiable();
            mockTwitterConfig.Setup(cr => cr.AccessToken).Returns("AccessToken");
            mockTwitterConfig.Setup(cr => cr.AccessTokenSecret).Returns("AccessTokenSecret");
            mockTwitterConfig.Setup(cr => cr.ConsumerKey).Returns("ConsumerKey");
            mockTwitterConfig.Setup(cr => cr.ConsumerSecret).Returns("ConsumerSecret");

            var objUt = new TwitterApiGateway(mockTwitterConfig.Object, mockTimeProvider.Object);

            Assert.Throws<AggregateException>(() => objUt.GetTweetsForHandle("ministryotech"));
        }

        [Test]
        [Category("API Integration")]
        public void GettingATwitterStreamWithValidAuthorizationReturnsTweets()
        {
            const string testHandle = "ministryotech";

            var objUt = new TwitterApiGateway(mockTwitterConfig.Object, mockTimeProvider.Object);
            var result = objUt.GetTweetsForHandle(testHandle, 6);

            Assert.True(result.Any());
            Assert.AreEqual(6, result.Count);

            foreach (var item in result)
            {
                Assert.That(item.User.ScreenNameResponse == testHandle);
            }
        }

        [Test]
        [Category("API Integration")]
        public void GettingATwitterStreamWithValidAuthorizationExposesTheRateLimitState()
        {
            const string testHandle = "ministryotech";

            var objUt = new TwitterApiGateway(mockTwitterConfig.Object, mockTimeProvider.Object);
            objUt.GetTweetsForHandle(testHandle);

            Assert.That(objUt.TwitterRateLimitResetsOn > mockTimeProvider.Object.Now);
        }

        [Test]
        [Category("API Integration")]
        public void TheRateLimitResetTimeIsUpdatedIfTheResetTimeIsCurrentlyLessThanNow()
        {
            const string testHandle = "ministryotech";
            var testTime = mockTimeProvider.Object.Now.Subtract(new TimeSpan(1, 0, 0));

            var objUt = new TwitterApiGateway(mockTwitterConfig.Object, mockTimeProvider.Object)
            {
                TwitterRateLimitHit = false,
                TwitterRateLimitResetsOn = testTime
            };

            objUt.GetTweetsForHandle(testHandle);

            Assert.That(objUt.TwitterRateLimitResetsOn > testTime);
            Assert.AreEqual(mockTimeProvider.Object.Now.AddMinutes(15), objUt.TwitterRateLimitResetsOn);
        }

        [Test]
        [Category("API Integration")]
        public void TheRateLimitResetTimeIsNotUpdatedIfTheResetTimeIsCurrentlyMoreThanNow()
        {
            const string testHandle = "ministryotech";
            var testTime = mockTimeProvider.Object.Now.AddMinutes(5);

            var objUt = new TwitterApiGateway(mockTwitterConfig.Object, mockTimeProvider.Object)
            {
                TwitterRateLimitHit = false,
                TwitterRateLimitResetsOn = testTime
            };

            objUt.GetTweetsForHandle(testHandle);

            Assert.AreEqual(testTime, objUt.TwitterRateLimitResetsOn);
        }
    }
}
