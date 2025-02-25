using Application.Common.Dtos.User;
using Application.UseCases.UserCases.Commands.AuthenticationUserCase;
using Application.UseCases.UserCases.Commands.DeleteUserCase;
using Application.UseCases.UserCases.Commands.RegisterUserCase;
using Application.UseCases.UserCases.Queries.GetUserByIdCase;
using AutoFixture;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IAlgorithms;
using Domain.Interfaces.IRepositories;
using Moq;

namespace Tests;

public class UserTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokensGenerator> _tokensGeneratorMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task AuthenticateUser_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var command = new AuthenticationUserCommand(user.PhoneNumber, "password");
        _unitOfWorkMock.Setup(u => u.Users.GetByPredicateAsync(It.IsAny<Func<User, bool>>(), It.IsAny<PageInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { user }, 1));
        _passwordHasherMock.Setup(p => p.VerifyHashedPassword(user.Password, command.Password)).Returns(true);
        _tokensGeneratorMock.Setup(t => t.GenerateAccessToken(user)).Returns("accessToken");
        _tokensGeneratorMock.Setup(t => t.GenerateRefreshToken(user)).Returns(new RefreshToken { Token = Guid.NewGuid() });
        var handler = new AuthenticationUserHandler(_unitOfWorkMock.Object, _passwordHasherMock.Object, _tokensGeneratorMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task RegisterUser_ShouldCreateUser_WhenPhoneNumberIsUnique()
    {
        // Arrange
        var command = _fixture.Create<RegisterUserCommand>();
        var user = _fixture.Create<User>();
        _unitOfWorkMock.Setup(u => u.Users.GetByPredicateAsync(It.IsAny<Func<User, bool>>(), It.IsAny<PageInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<User>(), 0));
        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        _passwordHasherMock.Setup(p => p.HashPassword(command.Password)).Returns("hashedPassword");
        var handler = new RegisterUserHandler(_unitOfWorkMock.Object, _passwordHasherMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNoContent_WhenUserExists()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var command = new DeleteUserCommand(user.Id);
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var handler = new DeleteUserHandler(_unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var command = new GetUserByIdQuery(user.Id);
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<ReadUserDto>(user)).Returns(new ReadUserDto(user.Id, user.PhoneNumber));
        var handler = new GetUserByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}