using Microsoft.Extensions.Configuration;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;
using TweetPicker.Service.Interfaces;

namespace TweetPicker.Service.Services;

public class TwitterApiService : ITwitterApiService
{
  private readonly TwitterClient _apiClient;

  public TwitterApiService(IConfiguration config)
  {
    var twitter = config.GetSection("Twitter");
    _apiClient = new TwitterClient(twitter["ConsumerKey"],
                                   twitter["ConsumerSecret"],
                                   twitter["AccessToken"],
                                   twitter["AccessSecret"]);
  }

  public async Task<IEnumerable<TweetV2>> GetRepliesToTweetAsync(long tweetId, string? tweetAuthor, long sinceId)
  {
    var searchParameters = new SearchTweetsV2Parameters($"to:{tweetAuthor}") { SinceId = sinceId.ToString() };

    var response = await _apiClient.SearchV2.SearchTweetsAsync(searchParameters);
    var output = response.Tweets.Where(x => x.ReferencedTweets is not null && x.ReferencedTweets.Any(y => y.Id == tweetId.ToString()));
  
    while (response.SearchMetadata.NextToken is not null)
    {
      searchParameters.NextToken = response.SearchMetadata.NextToken;
      response = await _apiClient.SearchV2.SearchTweetsAsync(searchParameters);
      output = output.Concat(response.Tweets.Where(x => x.ReferencedTweets is not null && x.ReferencedTweets.Any(y => y.Id == tweetId.ToString())));
    }

    return output;
  }

  public async Task<IEnumerable<ITweet>> GetLatestRetweetsOfTweetAsync(long tweetId)
  {
    var response = await _apiClient.Tweets.GetRetweetsAsync(tweetId);
    return response;
  }
  
  public async Task<UserV2> GetUserAsync(long userId)
  {
    var response = await _apiClient.UsersV2.GetUserByIdAsync(userId);
    return response.User;
  }
}