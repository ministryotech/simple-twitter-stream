# Simple Twitter Stream #
This library is designed to provide a simple, stromgly typed, twitter feed. It sits on top of the LinqToTwitter library.

# Configuration #
In order to be able to use the library to load your feed you need to provide configuration. You can do this however you want, the library is non-prescriptive about it, but you need to wire in your configuration by creating an implementation of **ITwitterConfig**.

This is a sample implementation which loads the required configuration from AppSettings...

    public class ConfigReader : ITwitterConfig
    {
        public string MasterHandle => GetValueAsString("twitterMasterHandle");

        public string[] SecondaryHandles => GetValueAsString("twitterSecondaryHandles").Split(',');

        public int TweetCount 
            => WebConfigurationManager.AppSettings["twitterTweetCount"] == null
                ? 0
                : int.Parse(WebConfigurationManager.AppSettings["twitterTweetCount"]);

        public int TwitterTimeout 
            => WebConfigurationManager.AppSettings["twitterTimeout"] == null
                ? 0
                : int.Parse(WebConfigurationManager.AppSettings["twitterTimeout"]) * 1000;

        public string ConsumerKey => GetValueAsString("twitterConsumerKey");

        public string ConsumerSecret => GetValueAsString("twitterConsumerSecret");

        public string AccessToken => GetValueAsString("twitterAccessToken");

        public string AccessTokenSecret => GetValueAsString("twitterAccessTokenSecret");

        #region | Private Methods |

        private string GetValueAsString(string key)
            => WebConfigurationManager.AppSettings[key] == null
                ? string.Empty
                : WebConfigurationManager.AppSettings[key];

        #endregion
    }

## Dependency Injection / Other Implementations ##
You should configure your ITwitterConfig implementation in your IoC / DI Container such as Autofac, Unity or Ninject. In total, you will need to configure implementations for the following interfaces which you can write your own for, if you like...

- **ITwitterConfig** (see above)
- **ITwitterLocalCacheGateway** (NullLocalCacheGateway without Cache or TwitterAppStateLocalCacheGateway if using the cache library)
- **ITwitterApiGateway** (TwitterApiGateway)
- **ITweetBuilder** (TweetBuilder)

If you aren't using DI then you will have to instantiate these classes yourself and pass them into the **TweetListBuilder** class, which is responsible for constructing the feed.

## Usage ##
Once you have DI set up, or an instance of **TweetListBuilder** manually created then usage is easy...

    public class MyClass()
    {
        private readonly ITweetListBuilder twitter;

        public MyClass(ITweetListBuilder tweetListBuilder)
        {
            twitter = tweetListBuilder;
        }

        public GetTweets()
        {
            // Get tweets for a single handle or group of handles as specified in ITwitterConfig.
            var configuredTweets = twitter.Build();

            // Get tweets for a single handle.
            var myTweets = twitter.BuildForHandle("ministryotech");

            // Get tweets for a group of handles.
            var myTweetsAndOtherTweets = twitter.BuildForHandles("ministryotech", new[] { "pragilecom", "agilerodent" })
        }
    }

### Caching ###
If you are using the library without a cache implementation of some sort (**NullLocalCacheGateway**) then the library will return no tweets when the Twitter rate limit is hit. If you are using .net Coore you can write your own cache implementation to handle this or you can contribute to the project and provide a .net Core version of the **Ministry.SimpleTwitterStream.Cache** library.

## Upgrade Notes ##
If you are upgrading from version 1, the new **Ministry.SimpleTwitterStream** package no longer includes the **TwitterAppStateLocalCacheGateway** class. Version 2 onwards is compatible with .net Standard 2.0 and the cache makes use of features that only make sense for .net Framework users. This class is now available as an add-on package called **Ministry.SimpleTwitterStream.Cache**. Currently the cache only supports .net Framework but I hope to add a .net Core version in the near future - It is blocked at the moment until an alternative dependent project can be developed for .net Core.

## The Ministry of Technology Open Source Products ##
Welcome to The Ministry of Technology open source products. All open source Ministry of Technology products are distributed under the MIT License for maximum re-usability. Details on more of our products and services can be found on our website at http://www.minotech.co.uk

Our other open source repositories can be found here...

* [https://github.com/ministryotech](https://github.com/ministryotech)
* [https://github.com/tiefling](https://github.com/tiefling)

### Where can I get it? ###
You can download the package for this project from any of the following package managers...

- **NUGET (standard)** - [https://www.nuget.org/packages/Ministry.SimpleTwitterStream](https://www.nuget.org/packages/Ministry.SimpleTwitterStream)
- **NUGET (cache)** - [https://www.nuget.org/packages/Ministry.SimpleTwitterStream.Cache](https://www.nuget.org/packages/Ministry.SimpleTwitterStream.Cache)

### Contribution guidelines ###
If you would like to contribute to the project, please contact me.

### Who do I talk to? ###
* Keith Jackson - keith@minotech.co.uk
