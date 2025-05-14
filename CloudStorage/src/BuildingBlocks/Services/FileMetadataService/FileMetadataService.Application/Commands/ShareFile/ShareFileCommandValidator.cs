using FluentValidation;

namespace FileMetadataService.Application.Commands.ShareFile;

public class ShareFileCommandValidator : AbstractValidator<ShareFileCommand>
{
    public ShareFileCommandValidator()
    {
        RuleFor(v => v.FileId)
            .NotEmpty().WithMessage("File ID is required");

        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(v => v.Permission)
            .IsInEnum().WithMessage("Invalid permission value");
    }
}