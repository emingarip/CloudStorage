using FluentValidation;

namespace FileStorageService.Application.Queries.GetFileUrl;

public class GetFileUrlQueryValidator : AbstractValidator<GetFileUrlQuery>
{
    public GetFileUrlQueryValidator()
    {
        RuleFor(v => v.FileId)
            .NotEmpty().WithMessage("File ID is required");
    }
}