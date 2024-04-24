using API.DTO;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task<RegistrationResponse> RegisterAsync(RegisterUserDTO registerUserDTO);
        Task<LoginResponse> LoginAsync(LoginDTO loginDTO);
    }
}
