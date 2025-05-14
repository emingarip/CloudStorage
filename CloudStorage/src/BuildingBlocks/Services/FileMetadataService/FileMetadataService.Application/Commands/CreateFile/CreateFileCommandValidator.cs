using FluentValidation;

namespace FileMetadataService.Application.Commands.CreateFile;

public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    public CreateFileCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters");

        RuleFor(v => v.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .MaximumLength(100).WithMessage("Content type must not exceed 100 characters");

        RuleFor(v => v.Size)
            .GreaterThan(0).WithMessage("File size must be greater than 0");

        RuleFor(v => v.Path)
            .NotEmpty().WithMessage("File path is required");

        RuleFor(v => v.OwnerId)
            .NotEmpty().WithMessage("Owner ID is required");

        RuleFor(v => v.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
    }
}