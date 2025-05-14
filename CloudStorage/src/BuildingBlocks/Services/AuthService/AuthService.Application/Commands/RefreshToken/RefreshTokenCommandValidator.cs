﻿using FluentValidation;

namespace AuthService.Application.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(v => v.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}