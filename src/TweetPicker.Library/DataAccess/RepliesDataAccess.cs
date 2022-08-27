using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TweetPicker.Library.Models;

namespace TweetPicker.Library.DataAccess;

public class RepliesDataAccess : DataAccess
{
  public RepliesDataAccess(IConfiguration config) : base(config)
  {
  }

  public async Task<Reply?> GetFirstReplyToGiveawaySinceAsync(long giveawayTweetId, DateTime sinceDate)
  {
    const string query = "SELECT TOP 1 * FROM [dbo].[Replies] WHERE [GiveawayTweetId] = @giveawayTweetId AND [RepliedAt] >= @repliedAt ORDER BY [Id]";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<Reply?>(query, new { giveawayTweetId, repliedAt = sinceDate.ToString("O") });
    return output;
  }

  public async Task<Reply?> GetLatestReplyAsync(long giveawayTweetId, long userId)
  {
    const string query = "SELECT TOP 1 * FROM [dbo].[Replies] WHERE [GiveawayTweetId] = @giveawayTweetId AND [UserId] = @userId ORDER BY [RepliedAt] DESC";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<Reply?>(query, new { giveawayTweetId, userId });
    return output;
  }

  public async Task<bool> ExistsReplyAsync(long replyId)
  {
    const string query = "SELECT TOP 1 1 FROM [dbo].[Replies] WHERE [Id] = @replyId";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<int>(query, new { replyId });
    return output == 1;
  }

  public async Task<bool> ExistsReplyAsync(long giveawayTweetId, long userId)
  {
    const string query = "SELECT TOP 1 1 FROM [dbo].[Replies] WHERE [GiveawayTweetId] = @giveawayTweetId AND [UserId] = @userId";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<int>(query, new { giveawayTweetId, userId });
    return output == 1;
  }

  public async Task InsertReplyAsync(Reply reply)
  {
    const string query = "INSERT INTO [dbo].[Replies] (Id, UserId, Text, GiveawayTweetId, RepliedAt) VALUES (@Id, @UserId, @Text, @GiveawayTweetId, @RepliedAt)";

    using var connection = CreateConnection();
    await connection.ExecuteAsync(query, reply);
  }
}