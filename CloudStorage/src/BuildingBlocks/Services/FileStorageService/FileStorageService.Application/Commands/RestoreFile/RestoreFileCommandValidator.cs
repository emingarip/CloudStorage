using FluentValidation;

namespace FileStorageService.Application.Commands.RestoreFile
{
    public class RestoreFileCommandValidator : AbstractValidator<RestoreFileCommand>
    {
        public RestoreFileCommandValidator()
        {
            RuleFor(v => v.FileId)
                .NotEmpty().WithMessage("File ID is required");
        }
    }
}