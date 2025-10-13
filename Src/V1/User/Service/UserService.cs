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
        await _userRepository.ValidateUniqueUsernameAsync(request.username);
        await _userRepository.ValidateUniqueEmailAsync(request.email);
        var hashedPassword = PasswordService.Hash(request.password);

        var userToCreate = new UserRequest(
            username: request.username,
            email: request.email,
            password: hashedPassword
        );

        var newUser = await _userRepository.CreateAsync(userToCreate);

        return new UserResponseDto(
            id: newUser.Id,
            username: newUser.Username
        );
    }

    public async Task<UserResponseDto> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetOneByUsername(username);

        return new UserResponseDto(
            id: user.Id,
            username: user.Username
        );
    }
    
    public async Task<UserResponseDto> UpdateUserAsync(string username, UserRequest user) 
    {
        var currentUser = await _userRepository.GetOneByUsername(username);

        if (!string.IsNullOrEmpty(user.username) && user.username != currentUser.Username)
            await _userRepository.ValidateUniqueUsernameAsync(user.username);

        if (!string.IsNullOrEmpty(user.email) && user.email != currentUser.Email)
            await _userRepository.ValidateUniqueEmailAsync(user.email);

        var userToUpdate = new UserRequest(
            username: user.username ?? currentUser.Username,
            email: user.email ?? currentUser.Email,
            password: !string.IsNullOrEmpty(user.password) 
                ? PasswordService.Hash(user.password) 
                : currentUser.Password
        );
            
        var updatedUser = await _userRepository.UpdateUser(username, userToUpdate);
        
        return new UserResponseDto(
            id: updatedUser.Id,
            username: updatedUser.Username
        );
    }
}