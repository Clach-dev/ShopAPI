using System.Net;
using System.Security.Claims;
using Application.Common.Dtos;
using Application.Common.Dtos.Product;
using Application.Common.MappingProfiles.UserProfiles;
using Application.Common.Utils;
using Application.UseCases.UserCases.Commands.RegisterUserCase;
using AutoFixture;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using FluentAssertions;
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

public class ProductsControllerTests
{
    private const string DatabaseName = "TestDb";
    private const string ConfigurationsFileName = "appsettings.json";
    
    private readonly ShopDbContext _shopDbContext;
    
    private readonly ProductsController _productsController;
    
    private readonly IFixture _fixture = new Fixture();
    
    private readonly Guid _userId = Guid.NewGuid();
    
    public ProductsControllerTests()
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
            .AddSingleton<IConfiguration>(configurations);

        var serviceProvider = services.BuildServiceProvider();
        
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        ], "TestAuth"));
        
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var mediatR = serviceProvider.GetRequiredService<IMediator>();
        
        _productsController = new ProductsController(httpContextAccessor, mapper, mediatR);
    }
    
    [Fact]
    public async Task CreateProduct_ValidData_ReturnsOk()
    {
        // Arrange
        var createProductDto = _fixture.Build<CreateProductDto>()
            .With(p => p.CategoryIds, new List<Guid>())
            .Create();
        
        // Act
        var act = await _productsController.CreateProduct(createProductDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
        
        result.Value.Should().BeEquivalentTo(createProductDto, options =>
            options.Excluding(x => x.CategoryIds));
    }
    
    [Fact]
    public async Task CreateProduct_ExistingProduct_ReturnsConflict()
    {
        // Arrange
        var productEntity = CreateProductEntity();
        
        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var createProductDto = _fixture.Build<CreateProductDto>()
            .With(p => p.Name, productEntity.Name)
            .With(p => p.Description, productEntity.Description)
            .With(p => p.Price, productEntity.Price)
            .With(p => p.CategoryIds, new List<Guid>())
            .Create();
        
        // Act
        var act = await _productsController.CreateProduct(createProductDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ExistingProductError);
    }
    
    [Fact]
    public async Task CreateProduct_WrongCategory_ReturnsConflict()
    {
        // Arrange
        var createProductDto = _fixture.Build<CreateProductDto>().Create();
        
        // Act
        var act = await _productsController.CreateProduct(createProductDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        CheckCategoryConflictResult(result);
    }
    
    [Fact]
    public async Task CreateProduct_WrongCategories_ReturnsConflict()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();

        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var createProductDto = _fixture.Build<CreateProductDto>()
            .With(p => p.CategoryIds, new List<Guid> { categoryEntity.Id, new Guid() })
            .Create();
        
        // Act
        var act = await _productsController.CreateProduct(createProductDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        CheckCategoryConflictResult(result);
    }
    
    [Fact]
    public async Task GetAllProducts_WithData_ReturnsOk()
    {
        // Arrange
        var productEntities = _fixture.Build<Product>()
            .Without(p => p.Categories)
            .Without(p => p.OrderItems)
            .CreateMany(3).ToList();

        _shopDbContext.Products.AddRange(productEntities);
        await _shopDbContext.SaveChangesAsync();

        // Act
        var act = await _productsController.GetAllProducts(new PageInfoDto(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductsDto>>();
        
        CheckSuccessResult(result);
        
        result.Value!.ReadProductDtos.Should().BeEquivalentTo(productEntities, options => options
            .Excluding(p => p.OrderItems)
            .Excluding(p => p.Categories));
        
        result.Value.TotalCount.Should().Be(productEntities.Count());
    }
    
    [Fact]
    public async Task GetAllProducts_WithNoData_ReturnsOk()
    {
        // Arrange

        // Act
        var act = await _productsController.GetAllProducts(new PageInfoDto(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductsDto>>();
        
        CheckSuccessResult(result);
        
        result.Value!.ReadProductDtos.Count().Should().Be(0);
        
        result.Value.TotalCount.Should().Be(0);
    }
    
    [Fact]
    public async Task GetProductById_ValidData_ReturnsOk()
    {
        // Arrange
        var productEntity = CreateProductEntity();

        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.SaveChangesAsync();

        // Act
        var act = await _productsController.GetProductById(productEntity.Id, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();

        CheckSuccessResult(result);
        
        result.Value.Should().BeEquivalentTo(productEntity, options => options
            .Excluding(p => p.OrderItems)
            .Excluding(p => p.Categories));
    }
    
    [Fact]
    public async Task GetProductById_WrongProductId_ReturnsNotFound()
    {
        // Arrange

        // Act
        var act = await _productsController.GetProductById(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ProductIdNotFoundError);
    }
    
    [Fact]
    public async Task GetProductsByFilter_ByPrice_ReturnsOk()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();
        
        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var productEntity = _fixture.Build<Product>()
            .With(p => p.Name, "Product")
            .With(p => p.Price, 1000)
            .Without(p => p.Categories)
            .Without(p => p.OrderItems)
            .Create();
    
        var productEntity2 = _fixture.Build<Product>()
            .With(p => p.Name, "Car")
            .With(p => p.Price, 2000)
            .With(p => p.Categories, new List<Category>() { categoryEntity })
            .Without(p => p.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.Products.AddAsync(productEntity2);
        await _shopDbContext.SaveChangesAsync();
    
        var getProductsByFilterDto = _fixture.Build<GetProductsByFilterDto>()
            .With(p => p.Name, "")
            .With(p => p.MaxPrice, 1500)
            .With(p => p.MinPrice, 500)
            .With(p => p.CategoryIds, new List<Guid>())
            .With(p  => p.PageInfoDto, new PageInfoDto())
            .Create();
        
        // Act
        var act = await _productsController.GetFilteredProducts(getProductsByFilterDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductsDto>>();
        
        CheckSuccessResult(result);
        
        result.Value!.ReadProductDtos.Should().BeEquivalentTo(new List<Product>() { productEntity }, options => options
            .Excluding(p => p.OrderItems)
            .Excluding(p => p.Categories));
    }
    
    [Fact]
    public async Task GetProductsByFilter_ByPrice2_ReturnsOk()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();
        
        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var productEntity = _fixture.Build<Product>()
            .With(p => p.Name, "Product")
            .With(p => p.Price, 1000)
            .Without(p => p.Categories)
            .Without(p => p.OrderItems)
            .Create();
    
        var productEntity2 = _fixture.Build<Product>()
            .With(p => p.Name, "Car")
            .With(p => p.Price, 2000)
            .With(p => p.Categories, new List<Category>() { categoryEntity })
            .Without(p => p.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.Products.AddAsync(productEntity2);
        await _shopDbContext.SaveChangesAsync();
    
        var getProductsByFilterDto = _fixture.Build<GetProductsByFilterDto>()
            .With(p => p.Name, "")
            .With(p => p.MaxPrice, 1500)
            .With(p => p.MinPrice, 1100)
            .With(p => p.CategoryIds, new List<Guid>())
            .With(p  => p.PageInfoDto, new PageInfoDto())
            .Create();
        
        // Act
        var act = await _productsController.GetFilteredProducts(getProductsByFilterDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductsDto>>();
        
        CheckSuccessResult(result);

        result.Value!.ReadProductDtos.Should().BeEquivalentTo(new List<Product>());
    }
    
    [Fact]
    public async Task GetProductsByFilter_ByName_ReturnsOk()
    {
        // Arrange
        var categoryEntity = _fixture.Build<Category>()
            .Without(p => p.Products)
            .Create();
        
        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var productEntity = _fixture.Build<Product>()
            .With(p => p.Name, "Product")
            .With(p => p.Price, 1000)
            .Without(p => p.Categories)
            .Without(p => p.OrderItems)
            .Create();
    
        var productEntity2 = _fixture.Build<Product>()
            .With(p => p.Name, "Car")
            .With(p => p.Price, 2000)
            .With(p => p.Categories, new List<Category>() { categoryEntity })
            .Without(p => p.OrderItems)
            .Create();
        
        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.Products.AddAsync(productEntity2);
        await _shopDbContext.SaveChangesAsync();
    
        var getProductsByFilterDto = _fixture.Build<GetProductsByFilterDto>()
            .With(p => p.Name, "Car")
            .With(p => p.MaxPrice, 2200)
            .With(p => p.MinPrice, 100)
            .With(p => p.CategoryIds, new List<Guid>())
            .With(p  => p.PageInfoDto, new PageInfoDto())
            .Create();
        
        // Act
        var act = await _productsController.GetFilteredProducts(getProductsByFilterDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductsDto>>();
        
        CheckSuccessResult(result);

        result.Value!.ReadProductDtos.Should().BeEquivalentTo(new List<Product>() { productEntity2 } , options => options
            .Excluding(p => p.OrderItems)
            .Excluding(p => p.Categories));
    }
    
    [Fact]
    public async Task UpdateProduct_ValidData_ReturnsOk()
    {
        // Arrange
        var productEntity = CreateProductEntity();

        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateProductDto = _fixture.Build<UpdateProductDto>()
            .With(p => p.Id, productEntity.Id)
            .With(p => p.CategoryIds, new List<Guid>())
            .Create();
        
        // Act
        var act = await _productsController.UpdateProduct(updateProductDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        CheckSuccessResult(result);

        result.Value.Should().BeEquivalentTo(updateProductDto, options => 
            options.Excluding(p => p.CategoryIds));
    }
    
    [Fact]
    public async Task UpdateProduct_ExistingProduct_ReturnsConflict()
    {
        // Arrange
        var productEntity = CreateProductEntity();

        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateProductDto = _fixture.Build<UpdateProductDto>()
            .With(p => p.Id, productEntity.Id)
            .With(p => p.Name, productEntity.Name)
            .With(p => p.Description, productEntity.Description)
            .With(p => p.Price, productEntity.Price)
            .Create();
        
        // Act
        var act = await _productsController.UpdateProduct(updateProductDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ExistingProductError);
    }
    
    [Fact]
    public async Task UpdateProduct_WrongCategories_ReturnsConflict()
    {
        // Arrange
        var productEntity = CreateProductEntity();

        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateProductDto = _fixture.Build<UpdateProductDto>()
            .With(p => p.Id, productEntity.Id)
            .Create();
        
        // Act
        var act = await _productsController.UpdateProduct(updateProductDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        CheckCategoryConflictResult(result);
    }
    
    [Fact]
    public async Task UpdateProduct_WrongCategory_ReturnsConflict()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();
        
        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();

        var productEntity = CreateProductEntity();

        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateProductDto = _fixture.Build<UpdateProductDto>()
            .With(p => p.Id, productEntity.Id)
            .With(p => p.CategoryIds, new List<Guid> { categoryEntity.Id, Guid.NewGuid() })
            .Create();
        
        // Act
        var act = await _productsController.UpdateProduct(updateProductDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadProductDto>>();
        
        CheckCategoryConflictResult(result);
    }
    
    [Fact]
    public async Task DeleteProduct_ValidData_ReturnsNoContent()
    {
        // Arrange
        var productEntity = CreateProductEntity();

        await _shopDbContext.Products.AddAsync(productEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _productsController.DeleteProduct(productEntity.Id, default);
        
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
    public async Task DeleteProduct_WrongProductId_ReturnsNotFound()
    {
        // Arrange
        
        // Act
        var act = await _productsController.DeleteProduct(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<byte?>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ProductIdNotFoundError);
    }
    
    private Product CreateProductEntity() => _fixture.Build<Product>()
        .Without(p => p.Categories)
        .Without(p => p.OrderItems)
        .Create();
    
    private Category CreateCategoryEntity() => _fixture.Build<Category>()
        .Without(p => p.Products)
        .Create();
    
    private static void CheckCategoryConflictResult<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.CategoryConflictError);
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