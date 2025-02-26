using System.Net;
using Application.Common.Dtos;
using Application.Common.Dtos.User;
using Application.Common.MappingProfiles.UserProfiles;
using Application.Common.Utils;
using Application.UseCases.UserCases.Commands.RegisterUserCase;
using AutoFixture;
using AutoMapper;
using Domain.Entities;
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
    
    private readonly ShopDbContext _shopDbContext;
    
    private readonly UsersController _usersController;
    
    private readonly IFixture _fixture = new Fixture();
    
    public UsersControllerTests()
    {
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        var options = new DbContextOptionsBuilder<ShopDbContext>().UseInMemoryDatabase(DatabaseName).Options;
        _shopDbContext = new ShopDbContext(options);

        var services = new ServiceCollection();

        services
            .AddHttpContextAccessor()
            .AddAutoMapper(typeof(RegisterUserMappingProfile).Assembly)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(RegisterUserHandler).Assembly))
            .AddDbContext<ShopDbContext>(opt => opt.UseInMemoryDatabase(DatabaseName))
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
            .AddScoped<ITokensGenerator, TokensGenerator>();

        var serviceProvider = services.BuildServiceProvider();
        
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
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
        
        _shopDbContext.Users.Add(userEntity);
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
    public async Task GetAllUsers_ReturnsOk()
    {
        // Arrange
        var userEntities = _fixture.Build<User>().Without(u => u.Orders).CreateMany(3).ToList();
        
        _shopDbContext.Users.AddRange(userEntities);
        await _shopDbContext.SaveChangesAsync();

        // Act
        var act = await _usersController.GetAllUsers(new PageInfoDto(),default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadUsersDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
        
        result.Value.ReadUserDtos.Should().HaveCount(3);
        
        result.Value.ReadUserDtos.Should().BeEquivalentTo(userEntities, options => options
            .Excluding(x => x.Password)
            .Excluding(x => x.RefreshToken)
            .Excluding(x => x.RefreshTokenId)
            .Excluding(x => x.Role)
            .Excluding(x => x.Orders));
        
        result.Value.TotalCount.Should().Be(3);
    }
}