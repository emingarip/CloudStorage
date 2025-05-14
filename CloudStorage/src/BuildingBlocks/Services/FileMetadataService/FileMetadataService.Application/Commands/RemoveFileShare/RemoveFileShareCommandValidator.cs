using FluentValidation;

namespace FileMetadataService.Application.Commands.RemoveFileShare;

public class RemoveFileShareCommandValidator : AbstractValidator<RemoveFileShareCommand>
{
    public RemoveFileShareCommandValidator()
    {
        RuleFor(v => v.FileId)
            .NotEmpty().WithMessage("File ID is required");

        RuleFor(v => v.ShareId)
            .NotEmpty().WithMessage("Share ID is required");
    }
}