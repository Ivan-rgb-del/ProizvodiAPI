using static ProizvodiAPI.Application.DTOs.AuthDTO;

namespace ProizvodiAPI.Application.Interfaces
{
    public interface IAuthServices
    {
        TokenResponseDTO? Login(LoginDTO dto);
    }
}
