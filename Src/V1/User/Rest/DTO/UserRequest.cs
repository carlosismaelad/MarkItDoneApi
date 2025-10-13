namespace MarkItDoneApi.V1.User.Rest.DTO;

public record UserRequest(string? username, string? email, string? password);