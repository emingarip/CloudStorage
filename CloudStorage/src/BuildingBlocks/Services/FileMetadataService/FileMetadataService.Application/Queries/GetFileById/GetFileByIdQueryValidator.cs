using FluentValidation;

namespace FileMetadataService.Application.Queries.GetFileById
{
    public class GetFileByIdQueryValidator : AbstractValidator<GetFileByIdQuery>
    {
        public GetFileByIdQueryValidator()
        {
            RuleFor(v => v.FileId)
                .NotEmpty().WithMessage("File ID is required");
        }
    }
}