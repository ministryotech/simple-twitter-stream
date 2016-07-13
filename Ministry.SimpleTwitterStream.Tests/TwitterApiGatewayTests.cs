using Ministry.SimpleTwitterStream.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace Ministry.SimpleTwitterStream.Tests
{
    [TestFixture]
    [Category("Gateway Test")]
    public class TwitterApiGatewayTests
    {
        private Mock<ITwitterConfig> mockTwitterConfig;
        private Mock<ITwitterLocalCacheGateway> mockLocalCache;
        private Mock<ITimeProvider> mockTimeProvider;
        private Mock<ITweetBuilder> mockTweetBuilder;

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

            mockTimeProvider = new Mock<ITimeProvider>();
            mockTimeProvider.Setup(tp => tp.Now).Returns(DateTime.Now);

            mockTweetBuilder = new Mock<ITweetBuilder>();
        }

        [TearDown]
        public void TearDown()
        {
            mockTwitterConfig = null;
            mockLocalCache = null;
            mockTimeProvider = null;
            mockTweetBuilder = null;
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
            var testHandle = "ministryotech";

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
            var testHandle = "ministryotech";

            var objUt = new TwitterApiGateway(mockTwitterConfig.Object, mockTimeProvider.Object);
            var result = objUt.GetTweetsForHandle(testHandle);

            Assert.That(objUt.TwitterRateLimitResetsOn > mockTimeProvider.Object.Now);
        }

        [Test]
        [Category("API Integration")]
        public void TheRateLimitResetTimeIsUpdatedIfTheResetTimeIsCurrentlyLessThanNow()
        {
            var testHandle = "ministryotech";
            var testTime = mockTimeProvider.Object.Now.Subtract(new TimeSpan(1, 0, 0));

            var objUt = new TwitterApiGateway(mockTwitterConfig.Object, mockTimeProvider.Object);
            objUt.TwitterRateLimitHit = false;
            objUt.TwitterRateLimitResetsOn = testTime;

            var result = objUt.GetTweetsForHandle(testHandle);

            Assert.That(objUt.TwitterRateLimitResetsOn > testTime);
            Assert.AreEqual(mockTimeProvider.Object.Now.AddMinutes(15), objUt.TwitterRateLimitResetsOn);
        }

        [Test]
        [Category("API Integration")]
        public void TheRateLimitResetTimeIsNotUpdatedIfTheResetTimeIsCurrentlyMoreThanNow()
        {
            var testHandle = "ministryotech";
            var testTime = mockTimeProvider.Object.Now.AddMinutes(5);

            var objUt = new TwitterApiGateway(mockTwitterConfig.Object, mockTimeProvider.Object);
            objUt.TwitterRateLimitHit = false;
            objUt.TwitterRateLimitResetsOn = testTime;

            var result = objUt.GetTweetsForHandle(testHandle);

            Assert.AreEqual(testTime, objUt.TwitterRateLimitResetsOn);
        }
    }
}
