using MarkItDoneApi.V1.User.Repository;
using MarkItDoneApi.V1.User.Rest.DTO;
using MarkItDoneApi.Src.V1.Core.Security;

namespace MarkItDoneApi.V1.User.Service;

public class UserService
{
    private readonly UserRepository _userRepository;
    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> CreateAsync(UserRequest request)
    {
        await _userRepository.ValidateUniqueUsernameAsync(request.Username);
        await _userRepository.ValidateUniqueEmailAsync(request.Email);
        var hashedPassword = PasswordService.Hash(request.Password);
        
        var userToCreate = new UserRequest(
            Username: request.Username,
            Email: request.Email,
            Password: hashedPassword
        );
        
        var newUser = await _userRepository.CreateAsync(userToCreate);
        
        return new UserResponseDto(
            id: newUser.Id,
            username: newUser.Username
        );
    }
}