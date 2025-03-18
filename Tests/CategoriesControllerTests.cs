using System.Net;
using Application.Common.Dtos;
using Application.Common.Dtos.Category;
using Application.Common.MappingProfiles.UserProfiles;
using Application.Common.Utils;
using Application.UseCases.CategoryCases.Commands.CreateCategoryCase;
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
using Microsoft.Extensions.DependencyInjection;
using Presentation.Controllers;

namespace Tests;

public class CategoriesControllerTests
{
    private const string DatabaseName = "TestDb";
    
    private readonly ShopDbContext _shopDbContext;
    
    private readonly CategoriesController _categoriesController;
    
    private readonly IFixture _fixture = new Fixture();
    
    public CategoriesControllerTests()
    {
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        var options = new DbContextOptionsBuilder<ShopDbContext>().UseInMemoryDatabase(DatabaseName).Options;
        _shopDbContext = new ShopDbContext(options);

        var services = new ServiceCollection();

        services
            .AddHttpContextAccessor()
            .AddAutoMapper(typeof(RegisterUserMappingProfile).Assembly)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateCategoryHandler).Assembly))
            .AddDbContext<ShopDbContext>(opt => opt.UseInMemoryDatabase(DatabaseName))
            .AddScoped<IUnitOfWork, UnitOfWork>();

        var serviceProvider = services.BuildServiceProvider();
        
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var mediatR = serviceProvider.GetRequiredService<IMediator>();
        
        _categoriesController = new CategoriesController(httpContextAccessor, mapper, mediatR);
    }

    [Fact]
    public async Task CreateCategory_ValidData_ReturnsOk()
    {
        // Arrange
        var createCategoryDto = _fixture.Create<CreateCategoryDto>();
        
        // Act
        var act = await _categoriesController.CreateCategory(createCategoryDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoryDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
        
        result.Value.Should().BeEquivalentTo(createCategoryDto);
    }

    [Fact]
    public async Task CreateCategory_ExistingCategory_ReturnsConflict()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();
        
        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var createCategoryDto = _fixture.Build<CreateCategoryDto>()
            .With(c => c.Name, categoryEntity.Name)
            .Create();
        
        // Act
        var act = await _categoriesController.CreateCategory(createCategoryDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoryDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllCategories_WithData_ReturnsOk()
    {
        // Arrange
        var categoryEntities = _fixture.Build<Category>()
            .Without(p => p.Products)
            .CreateMany(3).ToList();

        _shopDbContext.Categories.AddRange(categoryEntities);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _categoriesController.GetAllCategories(new PageInfoDto(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoriesDto>>();
        
        CheckSuccessResult(result);
        
        result.Value!.Categories.Should().BeEquivalentTo(categoryEntities, options => 
            options.Excluding(c => c.Products));
        result.Value!.TotalCount.Should().Be(categoryEntities.Count());
    }
    
    [Fact]
    public async Task GetAllCategories_WithNoData_ReturnsOk()
    {
        // Arrange
        
        // Act
        var act = await _categoriesController.GetAllCategories(new PageInfoDto(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoriesDto>>();
        
        CheckSuccessResult(result);

        result.Value!.Categories.Should().BeEmpty();
        result.Value!.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetCategoryById_ValidData_ReturnsOk()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();

        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _categoriesController.GetCategoryById(categoryEntity.Id, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoryDto>>();
        
        CheckSuccessResult(result);
        
        result.Value.Should().BeEquivalentTo(categoryEntity, options => options
            .Excluding(p => p.Products));
    }
    
    [Fact]
    public async Task GetCategoryById_WrongCategoryId_ReturnsNotFound()
    {
        // Arrange
        
        // Act
        var act = await _categoriesController.GetCategoryById(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoryDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.CategoryIdNotFoundError);
    }
    
    [Fact]
    public async Task UpdateCategory_ValidData_ReturnsOk()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();

        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();
        
        var updateCategoryDto = _fixture.Build<UpdateCategoryDto>()
            .With(c => c.Id, categoryEntity.Id)
            .Create();
        
        // Act
        var act = await _categoriesController.UpdateCategory(updateCategoryDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoryDto>>();
        
        CheckSuccessResult(result);

        result.Value.Should().BeEquivalentTo(updateCategoryDto);
    }
    
    [Fact]
    public async Task UpdateCategory_WrongCategoryId_ReturnsOk()
    {
        // Arrange
        var updateCategoryDto = _fixture.Build<UpdateCategoryDto>()
            .Create();
        
        // Act
        var act = await _categoriesController.UpdateCategory(updateCategoryDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoryDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.CategoryIdNotFoundError);
    }
    
    [Fact]
    public async Task UpdateCategory_ExistingCategoryData_ReturnsConflict()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();

        var categoryEntity2 = CreateCategoryEntity();
        
        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.Categories.AddAsync(categoryEntity2);
        await _shopDbContext.SaveChangesAsync();
        
        var updateCategoryDto = _fixture.Build<UpdateCategoryDto>()
            .With(c => c.Id, categoryEntity.Id)
            .With(c => c.Name, categoryEntity2.Name)
            .Create();
        
        // Act
        var act = await _categoriesController.UpdateCategory(updateCategoryDto, default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<ReadCategoryDto>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.ExistingCategoryError);
    }

    [Fact]
    public async Task DeleteCategory_ValidData_ReturnsOk()
    {
        // Arrange
        var categoryEntity = CreateCategoryEntity();
        
        await _shopDbContext.Categories.AddAsync(categoryEntity);
        await _shopDbContext.SaveChangesAsync();
        
        // Act
        var act = await _categoriesController.DeleteCategory(categoryEntity.Id, default);
        
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
    public async Task DeleteCategory_WrongCategoryId_ReturnsNotFound()
    {
        // Arrange
        
        // Act
        var act = await _categoriesController.DeleteCategory(Guid.NewGuid(), default);
        
        // Assert
        act.Should().BeOfType<ObjectResult>();
        
        var result = act.As<ObjectResult>().Value.As<Result<Unit>>();
        
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();
        result.Errors.Should().Contain(ErrorMessages.CategoryIdNotFoundError);
    }
    
    private Category CreateCategoryEntity() => _fixture.Build<Category>()
        .Without(p => p.Products)
        .Create();
    
    private static void CheckSuccessResult<T>(Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeNull();
    }
}