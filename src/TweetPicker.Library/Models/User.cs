namespace TweetPicker.Library.Models;

public class User
{
  public long Id { get; set; }
  public string? Username { get; set; }

  private User()
  {
    // needed by Dapper
  }
  
  public User(long id, string? username)
  {
    Id = id;
    Username = username;
  }
}