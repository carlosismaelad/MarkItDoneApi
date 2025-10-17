using MarkItDoneApi.Src.V1.User.Entity;

namespace MarkItDoneApi.Src.V1.User.Rest.DTO;

public record UserResponseDto(Guid Id, string Username)
{
  public static UserResponseDto FromEntity(UserEntity user)
  {
    return new UserResponseDto(user.Id, user.Username);
  }

};