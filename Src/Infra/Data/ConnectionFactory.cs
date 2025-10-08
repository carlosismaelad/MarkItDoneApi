using System.Data;
using Npgsql;

namespace MarkItDoneApi.Infra.Data;

public interface IConnectionFactory
{
  IDbConnection CreateConnection();
}

public class ConnectionFactory : IConnectionFactory
{
  private readonly string _connectionString;

  public ConnectionFactory(IConfiguration configuration)
  {
    _connectionString = configuration.GetConnectionString("DefaultConnection");
  }

  public IDbConnection CreateConnection()
  {
    return new NpgsqlConnection(_connectionString);
  }
}