using FluentValidation;

namespace FileMetadataService.Application.Commands.UpdateFileShare
{
    public class UpdateFileShareCommandValidator : AbstractValidator<UpdateFileShareCommand>
    {
        public UpdateFileShareCommandValidator()
        {
            RuleFor(v => v.FileId)
                .NotEmpty().WithMessage("File ID is required");

            RuleFor(v => v.ShareId)
                .NotEmpty().WithMessage("Share ID is required");

            RuleFor(v => v.Permission)
                .IsInEnum().WithMessage("Invalid permission value");
        }
    }
}