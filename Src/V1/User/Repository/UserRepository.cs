using MarkItDoneApi.Infra.Data;
using MarkItDoneApi.V1.User.Entity;
using MarkItDoneApi.V1.User.Rest.DTO;
using MarkItDoneApi.V1.Core.DomainExceptions;
using Dapper;

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
        using var connection = _connectionFactory.CreateConnection();
        
        var count = await connection.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM users WHERE username = @username", 
            new { username });

        if (count > 0)
        {
            throw new BusinessException("Não é possível utilizar esse nome de usuário. Escolha outro nome e tente novamente.");
        }
    }

    public async Task ValidateUniqueEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var count = await connection.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM users WHERE email = @email", 
            new { email });

        if (count > 0)
        {
            throw new BusinessException("Não foi possível utilizar esse e-mail. Escolha outro e-mail e tente novamente.");
        }
    }

    public async Task<UserEntity> CreateAsync(UserRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        var query = """
                    INSERT INTO users (username, email, password_digest)
                    VALUES (@username, @email, @password)
                    RETURNING *
                    """;

        var newUser = await connection.QuerySingleAsync<UserEntity>(query, new
        {
            username = request.username,
            email = request.email,
            password = request.password
        });

        return newUser;
    }

    public async Task<UserEntity> GetOneByUsername(string username)
    {
        using var connection = _connectionFactory.CreateConnection();

        var selectQuery = """
        SELECT * FROM users WHERE username = @username
        """;

        var userFounded = await connection.QuerySingleOrDefaultAsync<UserEntity>(selectQuery, new
        {
            username
        }) ?? throw new NotFoundException("Usuário não encontrado.");

        return userFounded;
    }

    public async Task<UserEntity> UpdateUser(string username, UserRequest user) 
    {
        using var connection = _connectionFactory.CreateConnection();

        var updateQuery = """
            UPDATE users 
            SET username = @username, 
                email = @email, 
                password_digest = @password, 
                updated_at = NOW()
            WHERE username = @currentUsername
            RETURNING *
            """;

        var updatedUser = await connection.QuerySingleAsync<UserEntity>(updateQuery, new
        {
            username = user.username,
            email = user.email,
            password = user.password,
            currentUsername = username
        });

        return updatedUser;
    }
}