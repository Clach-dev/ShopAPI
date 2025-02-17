namespace Application.Common.Dtos.Token;

public record ReadTokenDto(
    string AccessToken,
    string RefreshToken);