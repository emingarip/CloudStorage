using FluentValidation;

namespace FileMetadataService.Application.Commands.UpdateFile
{
    public class UpdateFileCommandValidator : AbstractValidator<UpdateFileCommand>
    {
        public UpdateFileCommandValidator()
        {
            RuleFor(v => v.FileId)
                .NotEmpty().WithMessage("File ID is required");

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("File name is required")
                .MaximumLength(255).WithMessage("File name must not exceed 255 characters");

            RuleFor(v => v.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
        }
    }
}