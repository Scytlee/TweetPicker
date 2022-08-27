using TweetPicker.Library.DataAccess;
using TweetPicker.Library.Models;
using TweetPicker.Service.Interfaces;

namespace TweetPicker.Service.Services;

public class SyncRepliesService : ISyncRepliesService
{
  private readonly ITwitterApiService _twitterApiService;
  
  private readonly GiveawayEntriesDataAccess _giveawayEntriesDataAccess;
  private readonly GiveawayTweetsDataAccess _giveawayTweetsDataAccess;
  private readonly RepliesDataAccess _repliesDataAccess;
  private readonly RetweetsDataAccess _retweetsDataAccess;
  private readonly UsersDataAccess _usersDataAccess;

  public SyncRepliesService(ITwitterApiService twitterApiService, GiveawayEntriesDataAccess giveawayEntriesDataAccess, GiveawayTweetsDataAccess giveawayTweetsDataAccess,
    RepliesDataAccess repliesDataAccess, RetweetsDataAccess retweetsDataAccess, UsersDataAccess usersDataAccess)
  {
    _twitterApiService = twitterApiService;

    _giveawayEntriesDataAccess = giveawayEntriesDataAccess;
    _giveawayTweetsDataAccess = giveawayTweetsDataAccess;
    _repliesDataAccess = repliesDataAccess;
    _retweetsDataAccess = retweetsDataAccess;
    _usersDataAccess = usersDataAccess;
  }

  public async Task SyncRepliesAsync(CancellationToken cancellationToken)
  {
    var openGiveaways = await _giveawayTweetsDataAccess.GetOpenGiveawayTweetsAsync();

    foreach (var giveaway in openGiveaways)
    {
      await SyncGiveawayRepliesAsync(giveaway, cancellationToken);
    }
  }

  private async Task SyncGiveawayRepliesAsync(GiveawayTweet giveaway, CancellationToken cancellationToken)
  {
    var creator = await _usersDataAccess.GetUserAsync(giveaway.UserId);

    if (creator is null)
    {
      // should never happen
      return;
    }
    
    // get tweets from last 24 hours, starting from first tweet from last 24 hours existing in database
    var sinceReply = await _repliesDataAccess.GetFirstReplyToGiveawaySinceAsync(giveaway.Id, DateTime.UtcNow - TimeSpan.FromDays(1));
    var sinceId = sinceReply?.Id ?? giveaway.Id;
    
    // TODO: creator's username might be null
    var unorderedTweets = await _twitterApiService.GetRepliesToTweetAsync(giveaway.Id, creator.Username, sinceId);
    var tweets = unorderedTweets.OrderBy(x => x.CreatedAt.UtcDateTime);

    foreach (var tweet in tweets)
    {
      var authorId = long.Parse(tweet.AuthorId);
      var replyingUser = await _usersDataAccess.GetUserAsync(authorId);

      if (replyingUser?.Username is null)
      {
        var twitterUser = await _twitterApiService.GetUserAsync(authorId);
        if (replyingUser is null)
        {
          replyingUser = new User(long.Parse(twitterUser.Id), twitterUser.Username);
          await _usersDataAccess.InsertUserAsync(replyingUser);
        }
        else
        {
          replyingUser.Username = twitterUser.Username;
          await _usersDataAccess.UpdateUserAsync(replyingUser);
        }
      }

      var tweetId = long.Parse(tweet.Id);

      if (await _repliesDataAccess.ExistsReplyAsync(tweetId))
      {
        continue;
      }

      var reply = new Reply(tweetId, authorId, giveaway.Id, tweet.CreatedAt.UtcDateTime, tweet.Text ?? string.Empty);
      await _repliesDataAccess.InsertReplyAsync(reply);

      var giveawayEntry = await _giveawayEntriesDataAccess.GetGiveawayEntryAsync(giveaway.Id, authorId);
      if (giveawayEntry is null)
      {
        // check if ready to be created
        var retweet = await _retweetsDataAccess.GetLatestRetweetAsync(giveaway.Id, authorId);
        if (retweet is null)
        {
          continue;
        }
        giveawayEntry = new GiveawayEntry(giveaway.Id, authorId, retweet.Id, reply.Id);
        await _giveawayEntriesDataAccess.InsertGiveawayEntryAsync(giveawayEntry);
      }
      else
      {
        giveawayEntry.ReplyId = reply.Id;
        await _giveawayEntriesDataAccess.UpdateGiveawayEntryAsync(giveawayEntry);
      }
    }
  }
}