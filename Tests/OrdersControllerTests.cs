using System.Net;
using Application.Common.Dtos;
using Application.Common.Dtos.Order;
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

namespace Tests;

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
    public async Task CreateOrder_OrderWithNotExistingUserId_ReturnsNotFound() 
    {
        // Arrange
        var createOrderDto = _fixture.Build<CreateOrderDto>().Create();
        
        // Act
        var act = await _ordersController.CreateOrder(createOrderDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
    
    [Fact]
    public async Task GetAllOrders_WithNoData_ReturnsOk()
    {
        // Arrange

        // Act
        var act = await _ordersController.GetAllOrders(new PageInfoDto(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrdersDto>>();

        CheckSuccessResult(result);
        
        result.Value!.Orders.Should().HaveCount(0);
        
        result.Value.TotalCount.Should().Be(0);
    }
    
    [Fact]
    public async Task GetOrderById_ValidData_ReturnsOk()
    {
        // Arrange
        var orderEntity = _fixture.Build<Order>().Without(u => u.User).Without(x => x.OrderItems).Create();
        
        _shopDbContext.Orders.Add(orderEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _ordersController.GetOrderById(orderEntity.Id, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderDto>>();

        CheckSuccessResult(result);
        
        result.Value.Should().BeEquivalentTo(orderEntity, options => options
            .Excluding(x => x.TotalPrice)
            .Excluding(x => x.Status)
            .Excluding(x => x.DeliveryDate)
            .Excluding(x => x.UserId)
            .Excluding(x => x.OrderItems)
            .Excluding(x => x.User));
    }
    
    [Fact]
    public async Task GetOrderById_NotExistingOrderId_ReturnsNotFound() 
    {
        // Arrange
        
        // Act
        var act = await _ordersController.GetOrderById(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderDto>>();
        
        CheckOrderIdNotFoundResult(result);
    }

    [Fact]
    public async Task GetOrdersByFilterHandler_ValidData_ReturnsOk()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .Without(u => u.Orders).Create();
        
        await _shopDbContext.Users.AddAsync(user);
        await _shopDbContext.SaveChangesAsync();
        
        var orderEntity1 = _fixture.Build<Order>()
            .With(u => u.User, user)
            .With(u => u.UserId, user.Id)
            .With(o => o.Status, "Ready")
            .Without(x => x.OrderItems)
            .Create();
        var orderEntity2 = _fixture.Build<Order>()
            .With(u => u.User, user)
            .With(u => u.UserId, user.Id)
            .With(o => o.Status, "ChipiChapa")
            .Without(x => x.OrderItems)
            .Create();
        
        await _shopDbContext.Orders.AddAsync(orderEntity1);
        await _shopDbContext.Orders.AddAsync(orderEntity2);
        await _shopDbContext.SaveChangesAsync();
        
        var getOrdersByFilterDto = _fixture.Build<GetOrderByFilterDto>()
            .With(o => o.Status, orderEntity1.Status)
            .With(o => o.DeliveryDate, orderEntity1.DeliveryDate)
            .With(o => o.PageInfoDto, new PageInfoDto())
            .Create();
        
        // Act
        var act = await _ordersController.GetOrdersByFilter(getOrdersByFilterDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrdersDto>>();

        CheckSuccessResult(result);

        result.Value!.Orders.Should().BeEquivalentTo(new List<Order>() { orderEntity1 }, option => 
            option
                .Excluding(o => o.OrderItems)
                .Excluding(o => o.User)
                .Excluding(o => o.UserId));
    }
    
    [Fact]
    public async Task UpdateOrder_ValidData_ReturnsOk() 
    {
        // Arrange
        var user = _fixture.Build<User>()
            .Without(u => u.Orders).Create();
        
        await _shopDbContext.Users.AddAsync(user);
        await _shopDbContext.SaveChangesAsync();
        
        var orderEntity = _fixture.Build<Order>()
            .With(u => u.User, user)
            .With(u => u.UserId, user.Id)
            .Without(u => u.OrderItems)
            .Create();
        
        await _shopDbContext.Orders.AddAsync(orderEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateOrderDto = _fixture.Build<UpdateOrderDto>()
            .With(o => o.Id, orderEntity.Id)
            .With(o => o.UserId, user.Id)
            .Create();
        
        // Act
        var act = await _ordersController.UpdateOrder(updateOrderDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderDto>>();

        CheckSuccessResult(result);

        result.Value.Should().BeEquivalentTo(updateOrderDto, option => 
            option.Excluding(o => o.UserId));
    }
    
    [Fact]
    public async Task UpdateOrder_WrongOrderId_ReturnsNotFound()
    {
        var updateOrderDto = _fixture.Build<UpdateOrderDto>().Create();
        
        // Act
        var act = await _ordersController.UpdateOrder(updateOrderDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderDto>>();
        
        CheckOrderIdNotFoundResult(result);
    }
    
    [Fact]
    public async Task UpdateOrder_WrongUserId_ReturnsNotFound()
    {
        var orderEntity = _fixture.Build<Order>()
            .Without(o => o.OrderItems)
            .Without(o => o.User)
            .Without(o => o.UserId)
            .Create();

        await _shopDbContext.Orders.AddAsync(orderEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateOrderDto = _fixture.Build<UpdateOrderDto>()
            .With(o => o.Id, orderEntity.Id)
            .Create();
        
        // Act
        var act = await _ordersController.UpdateOrder(updateOrderDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.UserIdNotFoundError);
    }
    
    [Fact]
    public async Task DeleteOrder_ValidData_ReturnsNoContent()
    {
        // Arrange
        var orderEntity = _fixture.Build<Order>()
            .Without(u => u.User)
            .Without(u => u.OrderItems)
            .Create();
        
        _shopDbContext.Orders.Add(orderEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _ordersController.DeleteOrder(orderEntity.Id, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<byte?>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Value.Should().BeNull();
        result.Errors.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteOrder_WrongOrderId_ReturnsNotFound()
    {
        // Arrange
        
        // Act
        var act = await _ordersController.DeleteOrder(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<byte?>>();
        
        CheckOrderIdNotFoundResult(result);
    }
    
    private static void CheckSuccessResult<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
    }
    
    private static void CheckOrderIdNotFoundResult<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.OrderIdNotFoundError);
    }
}