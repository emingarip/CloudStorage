using FluentValidation;

namespace FileStorageService.Application.Commands.UploadFile;

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(v => v.FileId)
            .NotEmpty().WithMessage("File ID is required");

        RuleFor(v => v.File)
            .NotNull().WithMessage("File is required");

        RuleFor(v => v.File.Length)
            .GreaterThan(0).WithMessage("File cannot be empty")
            .When(v => v.File != null);

        RuleFor(v => v.File.FileName)
            .NotEmpty().WithMessage("File name is required")
            .When(v => v.File != null);
    }
}