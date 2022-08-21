namespace TweetPicker.Library.Models;

public class GiveawayEntry
{
  public long GiveawayTweetId { get; set; }
  public long UserId { get; set; }
  public long RetweetId { get; set; }
  public long ReplyId { get; set; }

  public GiveawayEntry(long giveawayTweetId, long userId, long retweetId, long replyId)
  {
    GiveawayTweetId = giveawayTweetId;
    UserId = userId;
    RetweetId = retweetId;
    ReplyId = replyId;
  }
}