using System.Security.Cryptography;
using Dapper;
using MarkItDoneApi.Src.Infra.Data;
using MarkItDoneApi.Src.V1.Session.Entity;

namespace MarkItDoneApi.Src.V1.Session.Repository;

public class SessionRepository
{
    private readonly ConnectionFactory _connectionFactory;
    private static readonly double ExpirationInMilliseconds = TimeSpan.FromDays(30).TotalMilliseconds;

    public SessionRepository(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SessionEntity> CreateAsync(SessionEntity session)
    {
        session.Token = GenerateToken();
        session.ExpiresAt = DateTime.UtcNow.AddMilliseconds(ExpirationInMilliseconds);

        const string sql = @"
            INSERT INTO sessions (user_id, token, expires_at)
            VALUES (@UserId, @Token, @ExpiresAt)
            RETURNING *;
        ";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SessionEntity>(sql, session);
    }



    private static string GenerateToken()
    {
        var bytes = new byte[48];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLower();
    }
}