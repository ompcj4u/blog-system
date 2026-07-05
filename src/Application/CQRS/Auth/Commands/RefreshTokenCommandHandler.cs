using Application.Common;
using Application.Common.DTOs.Auth;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Auth.Commands;
public record RefreshTokenCommand(string AccessToken,string RefreshToken) : IRequest<Result<TokenResponse>>;
public class RefreshTokenCommandHandler(IUserRepository _userRepo, IJwtService _jwtService, IUnitOfWork _unitOfWork) : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _jwtService.GetUserIdFromToken(request.AccessToken);
        if (userId == null)
            return Result<TokenResponse>.Fail("Invalid access token");

        var user = await _userRepo.GetUserByRefreshTokenAsync(request.RefreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<TokenResponse>.Fail("Invalid or expired refresh token");

        var newAccessToken = _jwtService.GenerateToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepo.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return Result<TokenResponse>.Success(
            new TokenResponse(newAccessToken, newRefreshToken, DateTime.UtcNow.AddDays(7)),"توکن جدید");

    }
}
