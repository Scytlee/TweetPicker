using Dapper;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TweetPicker.Library.Models;

namespace TweetPicker.Library.DataAccess;

public class GiveawayEntriesDataAccess : DataAccess
{
  public GiveawayEntriesDataAccess(IConfiguration config) : base(config)
  {
  }

  public async Task<GiveawayEntry?> GetGiveawayEntry(long giveawayTweetId, long userId)
  {
    const string query = "SELECT TOP 1 * FROM [dbo].[GiveawayEntries] WHERE [GiveawayTweetId] = @giveawayTweetId AND [UserId] = @userId";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<GiveawayEntry?>(query, new { giveawayTweetId, userId });
    return output;
  }

  public async Task<bool> ExistsGiveawayEntry(long giveawayTweetId, long userId)
  {
    const string query = "SELECT TOP 1 1 FROM [dbo].[GiveawayEntries] WHERE [GiveawayTweetId] = @giveawayTweetId AND [UserId] = @userId";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<int>(query, new { giveawayTweetId, userId });
    return output == 1;
  }

  public async Task InsertGiveawayEntry(GiveawayEntry giveawayEntry)
  {
    const string query = "INSERT INTO [dbo].[GiveawayEntries] (GiveawayTweetId, UserId, RetweetId, ReplyId) VALUES (@GiveawayTweetId, @UserId, @RetweetId, @ReplyId)";

    using var connection = CreateConnection();
    await connection.ExecuteAsync(query, giveawayEntry);
  }

  public async Task UpdateGiveawayEntry(GiveawayEntry giveawayEntry)
  {
    const string query = "UPDATE [dbo].[GiveawayEntries] SET [RetweetId] = @RetweetId, [ReplyId] = @ReplyId WHERE [GiveawayTweetId] = @GiveawayTweetId AND [UserId] = @UserId";

    using var connection = CreateConnection();
    await connection.ExecuteAsync(query, giveawayEntry);
  }
}