using FluentValidation;

namespace AuthService.Application.Queries.GetUserById;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}