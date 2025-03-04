using System.Net;
using Application.Common.Dtos;
using Application.Common.Dtos.OrderItem;
using Application.Common.MappingProfiles.OrderItemProfiles;
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

public class OrderItemsControllerTests
{
    private const string DatabaseName = "TestDb";
    
    private readonly ShopDbContext _shopDbContext;
    
    private readonly OrderItemsController _orderItemsController;
    
    private readonly IFixture _fixture = new Fixture();
    
    public OrderItemsControllerTests()
    {
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        var options = new DbContextOptionsBuilder<ShopDbContext>().UseInMemoryDatabase(DatabaseName).Options;
        _shopDbContext = new ShopDbContext(options);

        var services = new ServiceCollection();

        services
            .AddHttpContextAccessor()
            .AddAutoMapper(typeof(CreateOrderItemMappingProfile).Assembly)
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
        
        _orderItemsController = new OrderItemsController(httpContextAccessor, mapper, mediatR);
    }
    
    [Fact]
    public async Task CreateOrder_ValidData_ReturnsOk()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .Without(u => u.User)
            .Without(u => u.UserId)
            .Without(u => u.OrderItems)
            .Create();
        var product = _fixture.Build<Product>()
            .Without(u => u.Categories)
            .Without(u => u.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(product);
        await _shopDbContext.Orders.AddAsync(order);
        await _shopDbContext.SaveChangesAsync();
        
        var createOrderItemDto = _fixture.Build<CreateOrderItemDto>()
            .With(x => x.OrderId, order.Id)
            .With(x => x.ProductId, product.Id)
            .Create();
        
        // Act
        var act = await _orderItemsController.CreateOrderItem(createOrderItemDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
        
        result.Value.Should().BeEquivalentTo(createOrderItemDto, options => options
            .Excluding(x => x.OrderId)
            .Excluding(x => x.ProductId));
    }
    
    [Fact]
    public async Task CreateOrderItem_ExistingOrderItem_ReturnsConflict() 
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .Without(u => u.User)
            .Without(u => u.UserId)
            .Without(u => u.OrderItems)
            .Create();
        var product = _fixture.Build<Product>()
            .Without(u => u.Categories)
            .Without(u => u.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(product);
        await _shopDbContext.Orders.AddAsync(order);
        await _shopDbContext.SaveChangesAsync();

        var orderItem = _fixture.Build<OrderItem>()
            .With(u => u.Order, order)
            .With(u => u.Product, product)
            .Create();
        
        await _shopDbContext.OrderItems.AddAsync(orderItem);
        await _shopDbContext.SaveChangesAsync();
        
        var createOrderItemDto = _fixture.Build<CreateOrderItemDto>()
            .With(x => x.OrderId, order.Id)
            .With(x => x.ProductId, product.Id)
            .With(x => x.Amount, orderItem.Amount)
            .Create();
        
        // Act
        var act = await _orderItemsController.CreateOrderItem(createOrderItemDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ExistingOrderItemError);
    }
    
    [Fact]
    public async Task CreateOrderItem_WrongOrderItemData_ReturnsOrderItemDataNotFoundError()
    {
        // Arrange
        var product = _fixture.Build<Product>()
            .Without(u => u.Categories)
            .Without(u => u.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(product);
        await _shopDbContext.SaveChangesAsync();

        var orderItem = _fixture.Build<OrderItem>()
            .Without(u => u.Order)
            .Without(u => u.Product)
            .Create();
        
        await _shopDbContext.OrderItems.AddAsync(orderItem);
        await _shopDbContext.SaveChangesAsync();
        
        var createOrderItemDto = _fixture.Build<CreateOrderItemDto>()
            .Without(x => x.OrderId)
            .With(x => x.ProductId, product.Id)
            .With(x => x.Amount, orderItem.Amount)
            .Create();
        
        // Act
        var act = await _orderItemsController.CreateOrderItem(createOrderItemDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.OrderItemDataNotFoundError);
    }
    
    [Fact]
    public async Task DeleteOrderItem_ValidData_ReturnsNoContent()
    {
        // Arrange
        var orderItemEntity = _fixture.Build<OrderItem>()
            .Without(u => u.Order)
            .Without(u => u.Product)
            .Create();

        await _shopDbContext.OrderItems.AddAsync(orderItemEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _orderItemsController.DeleteOrderItem(orderItemEntity.Id, default);
        
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
    public async Task DeleteOrderItem_WrongOrderItemId_ReturnsNotFound()
    {
        // Arrange
        
        // Act
        var act = await _orderItemsController.DeleteOrderItem(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<byte?>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.OrderItemDataNotFoundError);
    }
    
    [Fact]
    public async Task GetAllOrderItems_WithData_ReturnsOk()
    {
        // Arrange
        var orderItemEntities = _fixture.Build<OrderItem>()
            .Without(p => p.Product)
            .Without(p => p.Order)
            .CreateMany(3).ToList();

        _shopDbContext.OrderItems.AddRange(orderItemEntities);
        await _shopDbContext.SaveChangesAsync();

        // Act
        var act = await _orderItemsController.GetAllOrderItems(new PageInfoDto(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemsDto>>();
        
        CheckSuccessResult(result);
        
        result.Value!.ReadOrderItemDtos.Should().BeEquivalentTo(orderItemEntities, options => options
            .Excluding(p => p.Order)
            .Excluding(p => p.Product));
        
        result.Value.TotalCount.Should().Be(orderItemEntities.Count());
    }
    
    [Fact]
    public async Task GetOrderItemById_ValidData_ReturnsOk()
    {
        // Arrange
        var orderItemEntities = _fixture.Build<OrderItem>()
            .Without(p => p.Product)
            .Without(p => p.Order)
            .Create();

        await _shopDbContext.OrderItems.AddAsync(orderItemEntities);
        await _shopDbContext.SaveChangesAsync();

        // Act
        var act = await _orderItemsController.GetOrderItemById(orderItemEntities.Id, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();

        CheckSuccessResult(result);
        
        result.Value.Should().BeEquivalentTo(orderItemEntities, options => options
            .Excluding(p => p.Product)
            .Excluding(p => p.Order));
    }
    
    [Fact]
    public async Task GetOrderItemById_WrongOrderItemId_ReturnsNotFound()
    {
        // Arrange

        // Act
        var act = await _orderItemsController.GetOrderItemById(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.OrderItemDataNotFoundError);
    }
    
    [Fact]
    public async Task UpdateOrderItem_ValidData_ReturnsOk()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .Without(u => u.User)
            .Without(u => u.UserId)
            .Without(u => u.OrderItems)
            .Create();
        var product = _fixture.Build<Product>()
            .Without(u => u.Categories)
            .Without(u => u.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(product);
        await _shopDbContext.Orders.AddAsync(order);
        await _shopDbContext.SaveChangesAsync();

        var orderItem = _fixture.Build<OrderItem>()
            .Without(u => u.Order)
            .Without(u => u.Product)
            .Create();
        
        await _shopDbContext.OrderItems.AddAsync(orderItem);
        await _shopDbContext.SaveChangesAsync();
        
        var updateOrderItemDto = _fixture.Build<UpdateOrderItemDto>()
            .With(p => p.Id, orderItem.Id)
            .With(p => p.ProductId, product.Id)
            .With(p => p.OrderId, order.Id)
            .Create();
        
        // Act
        var act = await _orderItemsController.UpdateOrderItem(updateOrderItemDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();
        
        CheckSuccessResult(result);

        result.Value.Should().BeEquivalentTo(updateOrderItemDto);
    }
    
    [Fact]
    public async Task UpdateOrderItem_ExistingOrderItem_ReturnsConflict()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .Without(u => u.User)
            .Without(u => u.UserId)
            .Without(u => u.OrderItems)
            .Create();
        var product = _fixture.Build<Product>()
            .Without(u => u.Categories)
            .Without(u => u.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(product);
        await _shopDbContext.Orders.AddAsync(order);
        await _shopDbContext.SaveChangesAsync();

        var orderItem = _fixture.Build<OrderItem>()
            .With(u => u.Order, order)
            .With(u => u.Product, product)
            .Create();
        
        await _shopDbContext.OrderItems.AddAsync(orderItem);
        await _shopDbContext.SaveChangesAsync();
        
        var updateOrderItemDto = _fixture.Build<UpdateOrderItemDto>()
            .With(p => p.Id, orderItem.Id)
            .With(p => p.Amount, orderItem.Amount)
            .With(p => p.ProductId, product.Id)
            .With(p => p.OrderId, order.Id)
            .Create();
        
        // Act
        var act = await _orderItemsController.UpdateOrderItem(updateOrderItemDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ExistingOrderItemError);
    }
    
    [Fact]
    public async Task UpdateOrderItem_WrongOrderItemId_ReturnsNotFound()
    {
        // Arrange
        var updateUserDto = _fixture.Build<UpdateOrderItemDto>()
            .Without(u => u.OrderId)
            .Without(u => u.ProductId)
            .Create();
        
        // Act
        var act = await _orderItemsController.UpdateOrderItem(updateUserDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.OrderItemDataNotFoundError);
    }
    
    [Fact]
    public async Task UpdateOrderItem_OrderIdNotFound_ReturnOrderIdNotFoundError()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .Without(u => u.User)
            .Without(u => u.UserId)
            .Without(u => u.OrderItems)
            .Create();
        var product = _fixture.Build<Product>()
            .Without(u => u.Categories)
            .Without(u => u.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(product);
        await _shopDbContext.Orders.AddAsync(order);
        await _shopDbContext.SaveChangesAsync();

        var orderItem = _fixture.Build<OrderItem>()
            .With(u => u.Order, order)
            .With(u => u.Product, product)
            .Create();
        
        await _shopDbContext.OrderItems.AddAsync(orderItem);
        await _shopDbContext.SaveChangesAsync();
        
        var updateOrderItemDto = _fixture.Build<UpdateOrderItemDto>()
            .With(x => x.Id, orderItem.Id)
            .With(x => x.OrderId, Guid.NewGuid())
            .With(x => x.ProductId, product.Id)
            .With(x => x.Amount, orderItem.Amount)
            .Create();
        
        // Act
        var act = await _orderItemsController.UpdateOrderItem(updateOrderItemDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.OrderIdNotFoundError);
    }
    
    [Fact]
    public async Task UpdateOrder_ProductIdNotFound_ReturnProductIdNotFoundError()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .Without(u => u.User)
            .Without(u => u.UserId)
            .Without(u => u.OrderItems)
            .Create();
        var product = _fixture.Build<Product>()
            .Without(u => u.Categories)
            .Without(u => u.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(product);
        await _shopDbContext.Orders.AddAsync(order);
        await _shopDbContext.SaveChangesAsync();

        var orderItem = _fixture.Build<OrderItem>()
            .With(u => u.Order, order)
            .With(u => u.Product, product)
            .Create();
        
        await _shopDbContext.OrderItems.AddAsync(orderItem);
        await _shopDbContext.SaveChangesAsync();
        
        var updateOrderItemDto = _fixture.Build<UpdateOrderItemDto>()
            .With(x => x.Id, orderItem.Id)
            .With(x => x.OrderId, order.Id)
            .With(x => x.ProductId, Guid.NewGuid())
            .With(x => x.Amount, orderItem.Amount)
            .Create();
        
        // Act
        var act = await _orderItemsController.UpdateOrderItem(updateOrderItemDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadOrderItemDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ProductIdNotFound);
    }
    
    private static void CheckSuccessResult<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
    }
}