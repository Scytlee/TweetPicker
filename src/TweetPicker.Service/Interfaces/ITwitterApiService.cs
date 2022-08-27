using Tweetinvi.Models;
using Tweetinvi.Models.V2;

namespace TweetPicker.Service.Interfaces;

public interface ITwitterApiService
{
  Task<IEnumerable<TweetV2>> GetRepliesToTweetAsync(long tweetId, string? tweetAuthor, long sinceId);

  Task<IEnumerable<ITweet>> GetLatestRetweetsOfTweetAsync(long tweetId);
  
  Task<UserV2> GetUserAsync(long userId);
}
