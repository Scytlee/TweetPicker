using System;

namespace TweetPicker.Library.Models;

public class Retweet
{
  public long Id { get; set; }
  public long UserId { get; set; }
  public long GiveawayTweetId { get; set; }
  public DateTime RetweetedAt { get; set; }

  public Retweet(long id, long userId, long giveawayTweetId, DateTime retweetedAt)
  {
    Id = id;
    UserId = userId;
    GiveawayTweetId = giveawayTweetId;
    RetweetedAt = retweetedAt;
  }
}
