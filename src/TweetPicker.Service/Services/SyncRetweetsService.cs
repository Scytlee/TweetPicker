using TweetPicker.Library.DataAccess;
using TweetPicker.Library.Models;
using TweetPicker.Service.Interfaces;

namespace TweetPicker.Service.Services;

public class SyncRetweetsService : ISyncRetweetsService
{
  private readonly ITwitterApiService _twitterApiService;
  
  private readonly GiveawayEntriesDataAccess _giveawayEntriesDataAccess;
  private readonly GiveawayTweetsDataAccess _giveawayTweetsDataAccess;
  private readonly RepliesDataAccess _repliesDataAccess;
  private readonly RetweetsDataAccess _retweetsDataAccess;
  private readonly UsersDataAccess _usersDataAccess;

  public SyncRetweetsService(ITwitterApiService twitterApiService, GiveawayEntriesDataAccess giveawayEntriesDataAccess, GiveawayTweetsDataAccess giveawayTweetsDataAccess, RepliesDataAccess repliesDataAccess, RetweetsDataAccess retweetsDataAccess, UsersDataAccess usersDataAccess)
  {
    _twitterApiService = twitterApiService;
    _giveawayEntriesDataAccess = giveawayEntriesDataAccess;
    _giveawayTweetsDataAccess = giveawayTweetsDataAccess;
    _repliesDataAccess = repliesDataAccess;
    _retweetsDataAccess = retweetsDataAccess;
    _usersDataAccess = usersDataAccess;
  }

  public async Task SyncRetweetsAsync(CancellationToken cancellationToken)
  {
    var openGiveaways = await _giveawayTweetsDataAccess.GetOpenGiveawayTweetsAsync();

    foreach (var giveaway in openGiveaways)
    {
      await SyncGiveawayRetweetsAsync(giveaway, cancellationToken);
    }
  }

  private async Task SyncGiveawayRetweetsAsync(GiveawayTweet giveaway, CancellationToken cancellationToken)
  {
    var creator = await _usersDataAccess.GetUserAsync(giveaway.UserId);

    if (creator is null)
    {
      // should never happen
      return;
    }
    
    var retweets = (await _twitterApiService.GetLatestRetweetsOfTweetAsync(giveaway.Id)).OrderBy(x => x.CreatedAt.UtcDateTime);

    foreach (var tweet in retweets)
    {
      var retweeter = await _usersDataAccess.GetUserAsync(tweet.CreatedBy.Id);
      if (retweeter is null)
      {
        retweeter = new User(tweet.CreatedBy.Id, tweet.CreatedBy.ScreenName);
        await _usersDataAccess.InsertUserAsync(retweeter);
      }
      else if (retweeter.Username is null)
      {
        retweeter.Username = tweet.CreatedBy.ScreenName;
        await _usersDataAccess.UpdateUserAsync(retweeter);
      }

      if (await _retweetsDataAccess.ExistsRetweetAsync(tweet.Id))
      {
        continue;
      }

      var retweet = new Retweet(tweet.Id, retweeter.Id, giveaway.Id, tweet.CreatedAt.UtcDateTime);
      await _retweetsDataAccess.InsertRetweetAsync(retweet);
        
      var giveawayEntry = await _giveawayEntriesDataAccess.GetGiveawayEntryAsync(giveaway.Id, retweeter.Id);
      if (giveawayEntry is null)
      {
        // check if ready to be created
        var reply = await _repliesDataAccess.GetLatestReplyAsync(giveaway.Id, retweeter.Id);
        if (reply is null)
        {
          continue;
        }
        giveawayEntry = new GiveawayEntry(giveaway.Id, retweeter.Id, retweet.Id, reply.Id);
        await _giveawayEntriesDataAccess.InsertGiveawayEntryAsync(giveawayEntry);
      }
      else
      {
        giveawayEntry.RetweetId = retweet.Id;
        await _giveawayEntriesDataAccess.UpdateGiveawayEntryAsync(giveawayEntry);
      }
    }
  }
}