using FluentAssertions;
using TodoApi.DTOs;
using TodoApi.Validators;
using Xunit;

namespace TodoApi.UnitTests.Validators;

public class CreateTodoTaskValidatorTests
{
    private readonly CreateTodoTaskValidator _validator;

    public CreateTodoTaskValidatorTests()
    {
        _validator = new CreateTodoTaskValidator();
    }

    [Fact]
    public void Validate_ValidDto_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Task",
            Description = "Valid description",
            Priority = "High",
            DueDate = DateTime.UtcNow.AddDays(5),
            Tags = "test,valid"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyTitle_ShouldHaveError(string? title)
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = title!,
            Priority = "High"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_TitleExceedsMaxLength_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = new string('a', 201),
            Priority = "High"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Title" && 
            e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_DescriptionExceedsMaxLength_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Title",
            Description = new string('a', 1001),
            Priority = "High"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Description" && 
            e.ErrorMessage.Contains("500"));
    }

    [Theory]
    [InlineData("Low")]
    [InlineData("Medium")]
    [InlineData("High")]
    [InlineData("Urgent")]
    [InlineData("low")]
    [InlineData("HIGH")]
    public void Validate_ValidPriority_ShouldNotHaveError(string priority)
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Task",
            Priority = priority
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "Priority");
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("SuperUrgent")]
    [InlineData("123")]
    [InlineData("")]
    public void Validate_InvalidPriority_ShouldHaveError(string priority)
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Task",
            Priority = priority
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Priority");
    }

    [Fact]
    public void Validate_DueDateInPast_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Task",
            Priority = "High",
            DueDate = DateTime.UtcNow.AddDays(-1)  // Ayer
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "DueDate" && 
            e.ErrorMessage.Contains("future"));
    }

    [Fact]
    public void Validate_DueDateInFuture_ShouldNotHaveError()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Task",
            Priority = "High",
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "DueDate");
    }

    [Fact]
    public void Validate_NullDueDate_ShouldNotHaveError()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Task",
            Priority = "High",
            DueDate = null
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "DueDate");
    }

    [Fact]
    public void Validate_TagsExceedMaxLength_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Task",
            Priority = "High",
            Tags = new string('a', 501)
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Tags" && 
            e.ErrorMessage.Contains("100"));
    }
}