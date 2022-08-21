using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TweetPicker.Library.DataAccess;

public abstract class DataAccess
{
  private readonly IConfiguration _config;

  protected DataAccess(IConfiguration config)
  {
    _config = config;
  }

  private SqlConnection SqlConnection() => new(_config.GetConnectionString("Database"));

  protected IDbConnection CreateConnection()
  {
    var connection = SqlConnection();
    connection.Open();
    return connection;
  }
}