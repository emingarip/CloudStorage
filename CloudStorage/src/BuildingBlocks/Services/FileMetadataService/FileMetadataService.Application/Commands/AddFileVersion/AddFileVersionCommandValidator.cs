using FluentValidation;

namespace FileMetadataService.Application.Commands.AddFileVersion;

public class AddFileVersionCommandValidator : AbstractValidator<AddFileVersionCommand>
{
    public AddFileVersionCommandValidator()
    {
        RuleFor(v => v.FileId)
            .NotEmpty().WithMessage("File ID is required");

        RuleFor(v => v.Path)
            .NotEmpty().WithMessage("File path is required");

        RuleFor(v => v.Size)
            .GreaterThan(0).WithMessage("File size must be greater than 0");

        RuleFor(v => v.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .MaximumLength(100).WithMessage("Content type must not exceed 100 characters");
    }
}