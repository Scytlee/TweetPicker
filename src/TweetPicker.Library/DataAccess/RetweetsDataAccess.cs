using Dapper;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TweetPicker.Library.Models;

namespace TweetPicker.Library.DataAccess;

public class RetweetsDataAccess : DataAccess
{
  public RetweetsDataAccess(IConfiguration config) : base(config)
  {
  }

  public async Task<Retweet?> GetLatestRetweet(long giveawayTweetId, long userId)
  {
    const string query = "SELECT TOP 1 * FROM [dbo].[Retweets] WHERE [GiveawayTweetId] = @giveawayTweetId AND [UserId] = @userId ORDER BY [Id] DESC";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<Retweet?>(query, new { giveawayTweetId, userId });
    return output;
  }

  public async Task<bool> ExistsRetweet(long retweetId)
  {
    const string query = "SELECT TOP 1 1 FROM [dbo].[Retweets] WHERE [Id] = @retweetId";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<int>(query, new { retweetId });
    return output == 1;
  }

  public async Task InsertRetweet(Retweet retweet)
  {
    const string query = "INSERT INTO [dbo].[Retweets] (Id, UserId, GiveawayTweetId, RetweetedAt) VALUES (@Id, @UserId, @GiveawayTweetId, @RetweetedAt)";

    using var connection = CreateConnection();
    await connection.ExecuteAsync(query, retweet);
  }
}