using System.Net;
using System.Security.Claims;
using Application.Common.Dtos;
using Application.Common.Dtos.Token;
using Application.Common.Dtos.User;
using Application.Common.MappingProfiles.UserProfiles;
using Application.Common.Utils;
using Application.UseCases.UserCases.Commands.RegisterUserCase;
using AutoFixture;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.IAlgorithms;
using Domain.Interfaces.IRepositories;
using FluentAssertions;
using Infrastructure.Algorithms;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Controllers;

namespace Tests;

public class UsersControllerTests
{
    private const string DatabaseName = "TestDb";
    private const string ConfigurationsFileName = "appsettings.json";
    
    private readonly ShopDbContext _shopDbContext;
    
    private readonly UsersController _usersController;
    
    private readonly IFixture _fixture = new Fixture();
    
    private readonly Guid _userId = Guid.NewGuid();
    
    public UsersControllerTests()
    {
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        var options = new DbContextOptionsBuilder<ShopDbContext>().UseInMemoryDatabase(DatabaseName).Options;
        _shopDbContext = new ShopDbContext(options);

        var services = new ServiceCollection();

        var configurations = new ConfigurationBuilder().AddJsonFile(ConfigurationsFileName).Build();
        
        services
            .AddHttpContextAccessor()
            .AddAutoMapper(typeof(RegisterUserMappingProfile).Assembly)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(RegisterUserHandler).Assembly))
            .AddDbContext<ShopDbContext>(opt => opt.UseInMemoryDatabase(DatabaseName))
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddSingleton<IConfiguration>(configurations)
            .AddScoped<ITokensGenerator, TokensGenerator>();

        var serviceProvider = services.BuildServiceProvider();
        
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        ], "TestAuth"));
        
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var mediatR = serviceProvider.GetRequiredService<IMediator>();
        
        _usersController = new UsersController(httpContextAccessor, mapper, mediatR);
    }
    
    [Fact]
    public async Task RegisterUser_ValidData_ReturnsOk()
    {
        // Arrange
        var registerUserDto = _fixture.Build<RegisterUserDto>().Create();
        
        // Act
        var act = await _usersController.RegisterUser(registerUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
        
        result.Value.Should().BeEquivalentTo(registerUserDto, options => options
            .Excluding(x => x.Password));
    }
    
    [Fact]
    public async Task RegisterUser_UserWithExistingPhoneNumber_ReturnsConflict()
    {
        // Arrange
        var userEntity = _fixture.Build<User>().Without(u => u.Orders).Create();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var registerUserDto = _fixture.Build<RegisterUserDto>()
            .With(x => x.PhoneNumber, userEntity.PhoneNumber)
            .Create();
        
        // Act
        var act = await _usersController.RegisterUser(registerUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ExistingUserPhoneNumberError);
    }
    
    [Fact]
    public async Task GetAllUsers_WithData_ReturnsOk()
    {
        // Arrange
        var userEntities = _fixture.Build<User>().Without(u => u.Orders).CreateMany(3).ToList();
        
        _shopDbContext.Users.AddRange(userEntities);
        await _shopDbContext.SaveChangesAsync();

        // Act
        var act = await _usersController.GetAllUsers(new PageInfoDto(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUsersDto>>();

        CheckSuccessResult(result);
        
        result.Value!.ReadUserDtos.Should().HaveCount(3);
        
        result.Value.ReadUserDtos.Should().BeEquivalentTo(userEntities, options => options
            .Excluding(x => x.Password)
            .Excluding(x => x.RefreshToken)
            .Excluding(x => x.RefreshTokenId)
            .Excluding(x => x.Role)
            .Excluding(x => x.Orders));
        
        result.Value.TotalCount.Should().Be(3);
    }
    
    [Fact]
    public async Task GetAllUsers_WithNoData_ReturnsOk()
    {
        // Arrange

        // Act
        var act = await _usersController.GetAllUsers(new PageInfoDto(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUsersDto>>();

        CheckSuccessResult(result);
        
        result.Value!.ReadUserDtos.Should().HaveCount(0);
        
        result.Value.TotalCount.Should().Be(0);
    }
    
    [Fact]
    public async Task GetUserById_ValidData_ReturnsOk()
    {
        // Arrange
        var userEntity = _fixture.Build<User>().Without(u => u.Orders).Create();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _usersController.GetUserById(userEntity.Id, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserDto>>();

        CheckSuccessResult(result);
        
        result.Value.Should().BeEquivalentTo(userEntity, options => options
            .Excluding(x => x.Password)
            .Excluding(x => x.RefreshToken)
            .Excluding(x => x.RefreshTokenId)
            .Excluding(x => x.Role)
            .Excluding(x => x.Orders));
    }
    
    [Fact]
    public async Task GetUserById_NotExistingUserId_ReturnsNotFound()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        
        // Act
        var act = await _usersController.GetUserById(userId, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserDto>>();
        
        CheckUserIdNotFoundResult(result);
    }
    
    [Fact]
    public async Task AuthenticateUser_ValidData_ReturnsOk()
    {
        // Arrange
        var userEntity = _fixture.Build<RegisterUserDto>().Create();
        
        await _usersController.RegisterUser(userEntity, default);
        
        var authenticationUserDto = _fixture.Build<AuthenticationUserDto>()
            .With(x => x.PhoneNumber ,userEntity.PhoneNumber)
            .With(x => x.Password ,userEntity.Password)
            .Create();
        
        // Act
        var act = await _usersController.AuthenticateUser(authenticationUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadTokenDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
    }
    
    [Fact]
    public async Task AuthenticateUser_WrongPhoneNumber_ReturnsNotFound()
    {
        // Arrange
        var authenticationUserDto = _fixture.Build<AuthenticationUserDto>().Create();
        
        // Act
        var act = await _usersController.AuthenticateUser(authenticationUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadTokenDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.UserPhoneNotFoundError);
    }
    
    [Fact]
    public async Task AuthenticateUser_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var userEntity = _fixture.Build<RegisterUserDto>().Create();
        
        await _usersController.RegisterUser(userEntity, default);
        
        var authenticationUserDto = _fixture.Build<AuthenticationUserDto>()
            .With(x => x.PhoneNumber ,userEntity.PhoneNumber)
            .Create();
        
        // Act
        var act = await _usersController.AuthenticateUser(authenticationUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadTokenDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.WrongPasswordError);
    }
    
    [Fact]
    public async Task UpdateUser_ValidData_ReturnsOk()
    {
        // Arrange
        var userEntity = _fixture.Build<User>()
            .With(u => u.Id, _userId)
            .Without(u => u.Orders)
            .Create();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateUserDto = _fixture.Build<UpdateUserDto>().Create();
        
        // Act
        var act = await _usersController.UpdateUser(updateUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserDto>>();

        CheckSuccessResult(result);

        result.Value.Should().BeEquivalentTo(updateUserDto, options =>
            options.Excluding(x => x.Password));
    }
    
    [Fact]
    public async Task UpdateUser_WrongUserId_ReturnsNotFound()
    {
        var updateUserDto = _fixture.Build<UpdateUserDto>().Create();
        
        // Act
        var act = await _usersController.UpdateUser(updateUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserDto>>();
        
        CheckUserIdNotFoundResult(result);
    }
    
    [Fact]
    public async Task UpdateUser_ExistingPhoneNumber_ReturnsConflict()
    {
        // Arrange
        var userEntity = _fixture.Build<User>()
            .With(u => u.Id, _userId)
            .Without(u => u.Orders)
            .Create();
        
        var userEntity2 = _fixture.Build<User>()
            .Without(u => u.Orders)
            .Create();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.Users.AddAsync(userEntity2);
        await _shopDbContext.SaveChangesAsync();
        
        var updateUserDto = _fixture.Build<UpdateUserDto>()
            .With(u => u.PhoneNumber, userEntity2.PhoneNumber)
            .Create();
        
        // Act
        var act = await _usersController.UpdateUser(updateUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ExistingUserPhoneNumberError);
    }
    
    [Fact]
    public async Task UpdateUserRole_ValidData_ReturnsOk()
    {
        // Arrange
        var userEntity = CreateAdminUser();
        
        var userEntity2 = _fixture.Build<User>()
            .Without(u => u.Orders)
            .With(u => u.Role, Roles.User)
            .Create();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.Users.AddAsync(userEntity2);
        await _shopDbContext.SaveChangesAsync();
        
        var updateUserRoleDto = _fixture.Build<UpdateUserRoleDto>()
            .With(u => u.UserId, userEntity2.Id)
            .With(u => u.Role, Roles.Admin)
            .Create();
        
        // Act
        var act = await _usersController.UpdateUserRole(updateUserRoleDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserRoleDto>>();

        CheckSuccessResult(result);

        result.Value
            .Should().BeEquivalentTo(userEntity2, options =>
                options
                    .Excluding(u => u.Password)
                    .Excluding(u => u.RefreshToken)
                    .Excluding(u => u.RefreshTokenId)
                    .Excluding(u => u.PhoneNumber)
                    .Excluding(u => u.Orders)
                    .Excluding(u => u.Role));
        result.Value.Role.Should().Be(updateUserRoleDto.Role);
    }
    
    [Fact]
    public async Task UpdateUserRole_WrongUserId_ReturnsNotFound()
    {
        // Arrange
        var userEntity = CreateAdminUser();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateUserRoleDto = _fixture.Build<UpdateUserRoleDto>()
            .Create();
        
        // Act
        var act = await _usersController.UpdateUserRole(updateUserRoleDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUserRoleDto>>();
        
        CheckUserIdNotFoundResult(result);
    }
    
    [Fact]
    public async Task DeleteUser_ValidData_ReturnsNoContent()
    {
        // Arrange
        var userEntity = CreateAdminUser();
        
        var userEntity2 = _fixture.Build<User>()
            .Without(u => u.Orders)
            .Create();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.Users.AddAsync(userEntity2);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _usersController.DeleteUser(userEntity2.Id, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<Unit>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Value.Should().BeNull();
        result.Errors.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteUser_WrongUserId_ReturnsNotFound()
    {
        // Arrange
        var userEntity = CreateAdminUser();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _usersController.DeleteUser(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<Unit>>();
        
        CheckUserIdNotFoundResult(result);
    }
    
    [Fact]
    public async Task DeleteMyself_ValidData_ReturnsNoContent()
    {
        // Arrange
        var userEntity = CreateAdminUser();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _usersController.DeleteUser(default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<Unit>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Value.Should().BeNull();
        result.Errors.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteMyself_WrongUserId_ReturnsNoContent()
    {
        // Arrange
        var userEntity = _fixture.Build<User>()
            .With(u => u.Role, Roles.Admin)
            .Without(u => u.Orders)
            .Create();
        
        await _shopDbContext.Users.AddAsync(userEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _usersController.DeleteUser(default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<Unit>>();
        
        CheckUserIdNotFoundResult(result);
    }

    private User CreateAdminUser() => _fixture.Build<User>()
        .With(u => u.Id, _userId)
        .With(u => u.Role, Roles.Admin)
        .Without(u => u.Orders)
        .Create();

    private static void CheckSuccessResult<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
    }
    
    private static void CheckUserIdNotFoundResult<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.UserIdNotFoundError);
    }
}