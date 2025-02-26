using Domain.Entities;

namespace Domain.Interfaces.IAlgorithms;

public interface ITokensGenerator
{
    string GenerateAccessToken(User user);

    RefreshToken GenerateRefreshToken(User user);
}