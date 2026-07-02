using Application.Common;
using Application.Common.DTOs.Auth;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<TokenResponse>>;
public class LoginCommandHandler(IUserRepository _userRepo, IJwtService _jwtService) : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetUserByEmailAsync(request.Email);
        if (user == null || !user.Password.Verify(request.Password))
            return Result<TokenResponse>.Fail("Invalid email or password");

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepo.UpdateAsync(user);

        return Result<TokenResponse>.Success(
            new TokenResponse(accessToken, refreshToken, DateTime.UtcNow.AddDays(7)),
            "Login successful");
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
