using System.Net;
using Application.Common.Dtos;
using Application.Common.Dtos.Order;
using Application.Common.Dtos.User;
using Application.Common.MappingProfiles.OrderProfiles;
using Application.Common.Utils;
using Application.UseCases.OrderCases.Commands.CreateOrderCase;
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

public class OrdersControllerTests
{
    private const string DatabaseName = "TestDb";
    
    private readonly ShopDbContext _shopDbContext;
    
    private readonly OrdersController _ordersController;
    
    private readonly IFixture _fixture = new Fixture();
    
    public OrdersControllerTests()
    {
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        var options = new DbContextOptionsBuilder<ShopDbContext>().UseInMemoryDatabase(DatabaseName).Options;
        _shopDbContext = new ShopDbContext(options);

        var services = new ServiceCollection();

        services
            .AddHttpContextAccessor()
            .AddAutoMapper(typeof(CreateOrderMappingProfile).Assembly)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateOrderHandler).Assembly))
            .AddDbContext<ShopDbContext>(opt => opt.UseInMemoryDatabase(DatabaseName))
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
            .AddScoped<ITokensGenerator, TokensGenerator>();

        var serviceProvider = services.BuildServiceProvider();
        
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var mediatR = serviceProvider.GetRequiredService<IMediator>();
        
        _ordersController = new OrdersController(httpContextAccessor, mapper, mediatR);
    }
    
    [Fact]
    public async Task CreateOrder_ValidData_ReturnsOk()
    {
        // Arrange
        var user = _fixture.Build<User>().Without(u => u.Orders).Create();
        await _shopDbContext.Users.AddAsync(user);
        await _shopDbContext.SaveChangesAsync();
        
        var createOrderDto = _fixture.Build<CreateOrderDto>()
            .With(x => x.UserId, user.Id)
            .Create();
        
        // Act
        var act = await _ordersController.CreateOrder(createOrderDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
        
        result.Value.Should().BeEquivalentTo(createOrderDto, options => options
            .Excluding(x => x.UserId));
    }
    
    [Fact]
    public async Task CreateOrder_OrderWithNotExistingUserId_ReturnsConflict()
    {
        // Arrange
        var user = _fixture.Build<User>().Without(u => u.Orders).Create();
        
        _shopDbContext.Users.Add(user);
        await _shopDbContext.SaveChangesAsync();
        
        var createOrderDto = _fixture.Build<CreateOrderDto>()
            .With(x => x.UserId, user.Id)
            .Create();
        
        // Act
        var act = await _ordersController.CreateOrder(createOrderDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.UserIdNotFoundError);
    }
    
    [Fact]
    public async Task GetAllOrders_ReturnsOk()
    {
        // Arrange
        var user = _fixture.Build<User>().Without(u => u.Orders).Create();

        var orders = _fixture.Build<Order>().With(x => x.UserId, user.Id).Without(x => x.OrderItems).Without(x => x.User).CreateMany(3).ToList();
        await _shopDbContext.Users.AddAsync(user);
        await _shopDbContext.Orders.AddRangeAsync(orders);
        await _shopDbContext.SaveChangesAsync();

        // Act
        var act = await _ordersController.GetAllOrders(new PageInfoDto(),default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrdersDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
        
        result.Value.Orders.Should().HaveCount(3);
        
        result.Value.Orders.Should().BeEquivalentTo(orders, options => options
            .Excluding(x => x.TotalPrice)
            .Excluding(x => x.Status)
            .Excluding(x => x.DeliveryDate)
            .Excluding(x => x.UserId)
            .Excluding(x => x.OrderItems)
            .Excluding(x => x.User));
        
        result.Value.TotalCount.Should().Be(3);
    }
}