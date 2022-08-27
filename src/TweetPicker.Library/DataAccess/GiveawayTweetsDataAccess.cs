using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetPicker.Library.Models;

namespace TweetPicker.Library.DataAccess;

public class GiveawayTweetsDataAccess : DataAccess
{
  public GiveawayTweetsDataAccess(IConfiguration config) : base(config)
  {
  }

  public async Task<IEnumerable<GiveawayTweet>> GetOpenGiveawayTweetsAsync()
  {
    const string query = "SELECT * FROM [dbo].[GiveawayTweets] WHERE [Closed] = 0";

    using var connection = CreateConnection();
    var output = await connection.QueryAsync<GiveawayTweet>(query);
    return output;
  }
}