using MarkItDoneApi.Infra.Data;
using MarkItDoneApi.Src.V1.Core.DomainExceptions;
using MarkItDoneApi.V1.Core.DomainExceptions;
using MarkItDoneApi.V1.User.Entity;
using MarkItDoneApi.V1.User.Rest.DTO;
using Npgsql;

namespace MarkItDoneApi.V1.User.Repository;

public class UserRepository
{
    private readonly ConnectionFactory _connectionFactory;
    
    public UserRepository(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task ValidateUniqueUsernameAsync(string username)
    {
        await using var connection = (NpgsqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        
        await using var selectQuery = new NpgsqlCommand(
            "SELECT * FROM users WHERE username = @username", connection);
        selectQuery.Parameters.AddWithValue("username", username);
        
        var count = (long)await selectQuery.ExecuteScalarAsync();

        if (count > 0)
        {
            throw new BusinessException("Não é possível utilizar esse nome de usuário. Escolha outro nome e tente novamente.");
        }
    }

    public async Task ValidateUniqueEmailAsync(string email)
    {
        await using var connection = (NpgsqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        
        await using var selectQuery = new NpgsqlCommand(
            "SELECT * FROM users WHERE email = @email", connection);
        selectQuery.Parameters.AddWithValue("email", email);
        
        var count = (long)await selectQuery.ExecuteScalarAsync();

        if (count > 0)
        {
            throw new BusinessException("Não foi possível utilizar esse e-mail. Escolha outro e-mail e tente novamente.");
        }
    }

    public async Task<UserEntity> CreateAsync(CreateUserRequestDto request)
    {
        await using var connection = (NpgsqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var query = """
                    INSERT INTO 
                        users (username, email, password)
                    VALUES 
                        (@username, @email, @password)
                    RETURNING 
                    *;
                """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("username", request.Username);
        command.Parameters.AddWithValue("email", request.Email);
        command.Parameters.AddWithValue("password", request.Password);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new UserEntity(
                id: reader.GetGuid(reader.GetOrdinal("id")),
                username: reader.GetString(reader.GetOrdinal("username")),
                email: reader.GetString(reader.GetOrdinal("email")),
                password: reader.GetString(reader.GetOrdinal("password")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
                updatedAt: reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
            );
        }

        throw new ServiceException("Falha durante a criação de novo usuário.");
    }    
}