using Dapper;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TweetPicker.Library.Models;

namespace TweetPicker.Library.DataAccess;

public class UsersDataAccess : DataAccess
{
  public UsersDataAccess(IConfiguration config) : base(config)
  {
  }

  public async Task<User?> GetUser(long userId)
  {
    const string query = "SELECT * FROM [dbo].[Users] WHERE [Id] = @userId";

    using var connection = CreateConnection();
    var output = await connection.QuerySingleOrDefaultAsync<User?>(query, new { userId });
    return output;
  }

  public async Task InsertUser(User user)
  {
    const string query = "INSERT INTO [dbo].[Users] (Id, Username) VALUES (@Id, @Username)";

    using var connection = CreateConnection();
    await connection.ExecuteAsync(query, user);
  }

  public async Task UpdateUser(User user)
  {
    const string query = "UPDATE [dbo].[Users] SET [Username] = @Username WHERE [Id] = @Id";

    using var connection = CreateConnection();
    await connection.ExecuteAsync(query, user);
  }
}