namespace TweetPicker.Library.Models;

public class GiveawayTweet
{
  public long Id { get; set; }
  public long UserId { get; set; }
  public string Name { get; set; } = string.Empty;
  public bool Closed { get; set; }
  public bool ReplyRequired { get; set; }
  public bool RetweetRequired { get; set; }
}