using System;

namespace TweetPicker.Library.Models;

public class Reply
{
  public long Id { get; set; }
  public long UserId { get; set; }
  public string Text { get; set; }
  public long GiveawayTweetId { get; set; }
  public DateTime RepliedAt { get; set; }

  public Reply(long id, long userId, long giveawayTweetId, DateTime repliedAt, string text = "")
  {
    Id = id;
    UserId = userId;
    Text = text;
    GiveawayTweetId = giveawayTweetId;
    RepliedAt = repliedAt;
  }
} 