using FluentValidation;
using TodoApi.Core.Entities.Enums;
using TodoApi.DTOs;

namespace TodoApi.Validators;

public class UpdateTodoTaskValidator : AbstractValidator<UpdateTodoTaskDto>
{
    public UpdateTodoTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title length must be less than 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description length must be less than 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
        
        RuleFor(x => x.Tags)
            .MaximumLength(100).WithMessage("Tags length must be less than 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Tags));

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in future")
            .When(x => x.DueDate.HasValue);
    }
}