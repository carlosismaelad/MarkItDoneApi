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

    public async Task<UserResponseDto> CreateAsync(CreateUserRequestDto request)
    {
        await _userRepository.ValidateUniqueUsernameAsync(request.Username);
        await _userRepository.ValidateUniqueEmailAsync(request.Email);
        var hashedPassword = PasswordService.Hash(request.Password);

        var userToCreate = new CreateUserRequestDto(
            Username: request.Username,
            Email: request.Email,
            Password: hashedPassword
        );

        var newUser = await _userRepository.CreateAsync(userToCreate);

        return new UserResponseDto(
            Id: newUser.Id,
            Username: newUser.Username
        );
    }

    public async Task<UserResponseDto> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetOneByUsername(username);

        return new UserResponseDto(
            Id: user.Id,
            Username: user.Username
        );
    }
    
    public async Task<UserResponseDto> UpdateUserAsync(string username, UpdateUserRequestDto user) 
    {
        var currentUser = await _userRepository.GetOneByUsername(username);

        if (!string.IsNullOrEmpty(user.Username) && user.Username != currentUser.Username)
            await _userRepository.ValidateUniqueUsernameAsync(user.Username);

        if (!string.IsNullOrEmpty(user.Email) && user.Email != currentUser.Email)
            await _userRepository.ValidateUniqueEmailAsync(user.Email);

        var userToUpdate = new UpdateUserRequestDto(
            Username: user.Username ?? currentUser.Username,
            Email: user.Email ?? currentUser.Email,
            Password: !string.IsNullOrEmpty(user.Password) 
                ? PasswordService.Hash(user.Password) 
                : currentUser.Password
        );
            
        var updatedUser = await _userRepository.UpdateUser(username, userToUpdate);
        
        return new UserResponseDto(
            Id: updatedUser.Id,
            Username: updatedUser.Username
        );
    }
}