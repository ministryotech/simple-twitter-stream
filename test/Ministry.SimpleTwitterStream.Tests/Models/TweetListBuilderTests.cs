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
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ministry.SimpleTwitterStream.Tests.Models
{
    [TestFixture]
    [Category("Builder Test")]
    public class TweetListBuilderTests
    {
        private Mock<ITwitterConfig> mockTwitterConfig;
        private Mock<ITwitterLocalCacheGateway> mockLocalCache;
        private Mock<ITwitterApiGateway> mockTwitterApiGateway;
        private Mock<ITweetBuilder> mockTweetBuilder;

        private Status standardTestStatus = new Status()
        {
            Text = "Test Status",
            User = new User()
            {
                ScreenNameResponse = "ministryotech"
            }
        };

        #region | Setup and TearDown |

        [SetUp]
        public void SetUp()
        {
            mockTwitterConfig = new Mock<ITwitterConfig>();
            mockLocalCache = new Mock<ITwitterLocalCacheGateway>();
            mockLocalCache.SetupAllProperties();

            mockTwitterApiGateway = new Mock<ITwitterApiGateway>();
            mockTwitterApiGateway.SetupAllProperties();

            mockTweetBuilder = new Mock<ITweetBuilder>();
        }

        [TearDown]
        public void TearDown()
        {
            mockTwitterConfig = null;
            mockLocalCache = null;
            mockTwitterApiGateway = null;
            mockTweetBuilder = null;
        }

        #endregion

        [Test]
        public void InstantatesWithATweetCountFromConfig()
        {
            mockTwitterConfig.Setup(cr => cr.TweetCount).Returns(3).Verifiable();

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, mockTweetBuilder.Object);

            Assert.AreEqual(3, objUt.TweetCount);
            mockTwitterConfig.Verify();
        }

        [Test]
        public void InstantatesWithADefaultTweetCountIfConfigValueIs0()
        {
            mockTwitterConfig.Setup(cr => cr.TweetCount).Returns(0).Verifiable();

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, mockTweetBuilder.Object);

            Assert.AreEqual(5, objUt.TweetCount);
            mockTwitterConfig.Verify();
        }

        [Test]
        public void TheTweetCountCanBeOverridden()
        {
            mockTwitterConfig.Setup(cr => cr.TweetCount).Returns(3).Verifiable();

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, mockTweetBuilder.Object);
            objUt.TweetCount = 12;

            Assert.AreEqual(12, objUt.TweetCount);
            mockTwitterConfig.Verify();
        }

        [Test]
        public void TheInnerApiGatewayIsPopulatedWithRateLimitVariablesFromApplicationStateOnInstantiation()
        {
            mockLocalCache.Object.TwitterRateLimitHit = true;
            mockLocalCache.Object.TwitterRateLimitResetsOn = DateTime.Now;

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, mockTweetBuilder.Object);

            Assert.AreEqual(mockLocalCache.Object.TwitterRateLimitHit, mockTwitterApiGateway.Object.TwitterRateLimitHit);
            Assert.AreEqual(mockLocalCache.Object.TwitterRateLimitResetsOn, mockTwitterApiGateway.Object.TwitterRateLimitResetsOn);
        }

        [Test]
        public void BuildingATwitterStreamWithInvalidAuthorizationWillNotCauseTheMethodToFallOverButNoTweetsWillReturn()
        {
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle("ministryotech", It.IsAny<int>())).Throws<Exception>();

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle("ministryotech");

            Assert.False(result.Any());
        }

        [Test]
        public void BuildingATwitterStreamWithASingleHandleStoresAMasterHandleAndNoSecondaryHandles()
        {
            var testHandle = "ministryotech";
            mockTwitterConfig.Setup(cr => cr.TweetCount).Returns(6).Verifiable();
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>())).Returns(
                new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus });

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            Assert.AreEqual(testHandle, result.MasterHandle);
            Assert.False(result.SecondaryHandles.Any());
        }

        [Test]
        public void BuildingATwitterStreamWithValidAuthorizationReturnsTweets()
        {
            var testHandle = "ministryotech";
            mockTwitterConfig.Setup(cr => cr.TweetCount).Returns(6).Verifiable();
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, 6)).Returns(
                new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus });

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            Assert.True(result.Any());
            Assert.AreEqual(6, result.Count);
        }

        [Test]
        public void BuildingATwitterStreamWithValidAuthorizationStoresTheRateLimitState()
        {
            var testHandle = "ministryotech";
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>())).Returns(
                new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus });

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            mockLocalCache.VerifyGet(lc => lc.TwitterRateLimitHit, Times.AtLeastOnce);
            mockLocalCache.VerifySet(lc => lc.TwitterRateLimitHit = It.IsAny<bool>(), Times.AtLeastOnce);
            mockLocalCache.VerifySet(lc => lc.TwitterRateLimitResetsOn = It.IsAny<DateTime>(), Times.AtLeastOnce);
        }

        [Test]
        public void BuildingATwitterStreamWithValidAuthorizationAndMultipleHandlesReturnsTweetsButNotRetweetsForSecondaryHandles()
        {
            var testMasterHandle = "ministryotech";
            var testOtherHandles = new string[] { "scrimmers", "nigelebaker" };
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testMasterHandle, It.IsAny<int>())).Returns(
                new List<Status>() {
                    standardTestStatus,
                    new Status() {
                        User = new User() {
                            ScreenNameResponse = testMasterHandle
                        },
                        Text = "RT Test"
                    },
                    standardTestStatus, standardTestStatus
                });

            foreach (var handle in testOtherHandles)
            {
                mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(handle, It.IsAny<int>())).Returns(new List<Status>() {
                    new Status() {
                        Text = "RT Test",
                        User = new User() {
                            ScreenNameResponse = handle
                        }
                    },
                    new Status() {
                        User = new User() {
                            ScreenNameResponse = handle
                        },
                        Text = "Test Tweet"
                    }
                });
            }

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            objUt.TweetCount = 12;
            var result = objUt.BuildForHandles(testMasterHandle, testOtherHandles);

            Assert.True(result.Any());
            Assert.AreEqual(6, result.Count);

            foreach (var item in result)
            {
                if (item.Handle == testMasterHandle || item.RetweetedByHandle == testMasterHandle)
                {
                    Assert.That(item.Handle == testMasterHandle || item.RetweetedByHandle == testMasterHandle);
                }
                else
                {
                    Assert.That(testOtherHandles.Contains(item.Handle));
                    Assert.False(item.Text.StartsWith("RT"));
                }
            }
        }

        [Test]
        public void BuildingATwitterStreamWithMultipleHandlesStoresAMasterHandleAndSecondaryHandles()
        {
            var testMasterHandle = "ministryotech";
            var testOtherHandles = new string[] { "scrimmers", "nigelebaker" };
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testMasterHandle, It.IsAny<int>())).Returns(new List<Status>() { standardTestStatus });

            foreach (var handle in testOtherHandles)
            {
                mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(handle, It.IsAny<int>())).Returns(new List<Status>() { standardTestStatus });
            }

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            objUt.TweetCount = 12;
            var result = objUt.BuildForHandles(testMasterHandle, testOtherHandles);

            Assert.AreEqual(testMasterHandle, result.MasterHandle);
            Assert.AreEqual(testOtherHandles.Count(), result.SecondaryHandles.Count);

            foreach (var handle in testOtherHandles)
            {
                Assert.That(result.SecondaryHandles.Contains(handle));
            }
        }

        [Test]
        public void IfTheRateLimitHasBeenHitAndTheCurrentTimeIsBeforeTheResetTimeThenTheTwitterApiWillNotBeQueried()
        {
            var testHandle = "ministryotech";
            mockLocalCache.Object.TwitterRateLimitHit = true;
            mockLocalCache.Object.TwitterRateLimitResetsOn = DateTime.Now.AddMinutes(12);
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>())).Returns(
                new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus });

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            mockLocalCache.VerifyGet(lc => lc.TwitterRateLimitHit, Times.AtLeastOnce);
            mockLocalCache.VerifyGet(lc => lc.TwitterRateLimitResetsOn, Times.AtLeastOnce);
            mockTwitterApiGateway.Verify(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void IfTheRateLimitHasBeenHitAndTheCurrentTimeIsAfterTheResetTimeThenTheTwitterApiWillBeQueried()
        {
            var testHandle = "ministryotech";
            mockLocalCache.Object.TwitterRateLimitHit = true;
            mockLocalCache.Object.TwitterRateLimitResetsOn = DateTime.Now.Subtract(new TimeSpan(0,1,0));
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>())).Returns(
                new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus });

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            mockLocalCache.VerifyGet(lc => lc.TwitterRateLimitHit, Times.AtLeastOnce);
            mockLocalCache.VerifyGet(lc => lc.TwitterRateLimitResetsOn, Times.AtLeastOnce);
            mockTwitterApiGateway.Verify(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void BuildAloneUsesTwitterConfigToLoadTheStreamLoadingASingleStreamIfSecondaryHandlesAreNotProvided()
        {
            var testHandle = "ministryotech";
            mockTwitterConfig.Setup(cr => cr.TweetCount).Returns(6).Verifiable();
            mockTwitterConfig.Setup(cr => cr.MasterHandle).Returns(testHandle);
            mockTwitterConfig.Setup(cr => cr.SecondaryHandles).Returns(new string[0]);
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, 6)).Returns(
                new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus, standardTestStatus });

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.Build();

            Assert.True(result.Any());
            Assert.AreEqual(6, result.Count);
        }

        [Test]
        public void BuildAloneUsesTwitterConfigToLoadTheStreamLoadingAMultipleHandleStreamIfSecondaryHandlesAreProvided()
        {
            var testMasterHandle = "ministryotech";
            var testOtherHandles = new string[] { "scrimmers", "nigelebaker" };
            mockTwitterConfig.Setup(cr => cr.MasterHandle).Returns(testMasterHandle);
            mockTwitterConfig.Setup(cr => cr.SecondaryHandles).Returns(testOtherHandles);
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testMasterHandle, It.IsAny<int>())).Returns(
                new List<Status>() {
                    standardTestStatus,
                    new Status() {
                        User = new User() {
                            ScreenNameResponse = testMasterHandle
                        },
                        Text = "RT Test"
                    },
                    standardTestStatus, standardTestStatus
                });

            foreach (var handle in testOtherHandles)
            {
                mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(handle, It.IsAny<int>())).Returns(new List<Status>() {
                    new Status() {
                        Text = "RT Test",
                        User = new User() {
                            ScreenNameResponse = handle
                        }
                    },
                    new Status() {
                        User = new User() {
                            ScreenNameResponse = handle
                        },
                        Text = "Test Tweet"
                    }
                });
            }

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            objUt.TweetCount = 12;
            var result = objUt.Build();

            Assert.True(result.Any());
            Assert.AreEqual(6, result.Count);

            foreach (var item in result)
            {
                if (item.Handle == testMasterHandle || item.RetweetedByHandle == testMasterHandle)
                {
                    Assert.That(item.Handle == testMasterHandle || item.RetweetedByHandle == testMasterHandle);
                }
                else
                {
                    Assert.That(testOtherHandles.Contains(item.Handle));
                }
            }
        }

        [Test]
        public void WhenTheTweetsAreLoadedFromTheApiTheyAreSavedToLocalStorageOrMemory()
        {
            var testHandle = "ministryotech";
            var testTweets = new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus };
            mockLocalCache.Object.TwitterRateLimitHit = false;
            mockLocalCache.Object.TwitterRateLimitResetsOn = DateTime.Now.Subtract(new TimeSpan(0, 1, 0));
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>())).Returns(testTweets);

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            mockLocalCache.Verify(lc => lc.SaveTweetsForHandle(testHandle, testTweets), Times.Once);
        }

        [Test]
        public void WhenTheApiRateLimitHasNotBeenHitTheTweetsAreNotLoadedFromLocalStorageOrMemory()
        {
            var testHandle = "ministryotech";
            var testTweets = new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus };
            mockLocalCache.Object.TwitterRateLimitHit = false;
            mockLocalCache.Object.TwitterRateLimitResetsOn = DateTime.Now.AddMinutes(5);
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>())).Returns(testTweets);
            mockLocalCache.Setup(lc => lc.SaveTweetsForHandle(testHandle, testTweets));

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            mockLocalCache.Verify(lc => lc.GetTweetsForHandle(testHandle, It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void WhenTheApiRateLimitIsHitTheTweetsAreLoadedFromLocalStorageOrMemory()
        {
            var testHandle = "ministryotech";
            var testTweets = new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus };
            mockLocalCache.Object.TwitterRateLimitHit = true;
            mockLocalCache.Object.TwitterRateLimitResetsOn = DateTime.Now.AddMinutes(5);
            mockLocalCache.Setup(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>())).Returns(testTweets);

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            mockTwitterApiGateway.Verify(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>()), Times.Never);
            mockLocalCache.Verify(lc => lc.SaveTweetsForHandle(testHandle, testTweets), Times.Never);
            mockLocalCache.Verify(lc => lc.GetTweetsForHandle(testHandle, It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void WhenTheApiRateLimitResetTimePassesThenTheApiRateLimitHasNotBeenHitAnymoreAndTheTweetsAreLoadedFromTheApi()
        {
            var testHandle = "ministryotech";
            var testTweets = new List<Status>() { standardTestStatus, standardTestStatus, standardTestStatus };
            mockLocalCache.Object.TwitterRateLimitHit = true;
            mockLocalCache.Object.TwitterRateLimitResetsOn = DateTime.Now.Subtract(new TimeSpan(0, 1, 0));
            mockTwitterApiGateway.Setup(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>())).Returns(testTweets);
            mockLocalCache.Setup(lc => lc.SaveTweetsForHandle(testHandle, testTweets));

            var objUt = new TweetListBuilder(mockTwitterConfig.Object, mockLocalCache.Object, mockTwitterApiGateway.Object, new TweetBuilder());
            var result = objUt.BuildForHandle(testHandle);

            mockTwitterApiGateway.Verify(gw => gw.GetTweetsForHandle(testHandle, It.IsAny<int>()), Times.Once);
            mockLocalCache.Verify(lc => lc.GetTweetsForHandle(testHandle, It.IsAny<int>()), Times.Never);
        }
    }
}
