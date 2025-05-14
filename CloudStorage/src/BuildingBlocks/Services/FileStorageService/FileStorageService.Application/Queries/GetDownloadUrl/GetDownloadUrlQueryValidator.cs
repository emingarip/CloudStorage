using FluentValidation;

namespace FileStorageService.Application.Queries.GetDownloadUrl;

public class GetDownloadUrlQueryValidator : AbstractValidator<GetDownloadUrlQuery>
{
    public GetDownloadUrlQueryValidator()
    {
        RuleFor(v => v.FileId)
            .NotEmpty().WithMessage("File ID is required");
    }
}