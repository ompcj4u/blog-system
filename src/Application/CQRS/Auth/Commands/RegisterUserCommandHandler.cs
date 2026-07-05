using Application.Common;
using Application.Common.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Auth.Commands;

public record RegisterUserCommand(string FullName, string Email, string Password) : IRequest<Result<TokenResponse>>;

public class RegisterUserCommandHandler(IUserRepository repoUser, IJwtService repoJWT, IUnitOfWork _unitOfWork) : IRequestHandler<RegisterUserCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var isUserExist = await repoUser.GetUserByEmailAsync(email.Value);
        if (isUserExist != null)
        {
            return Result<TokenResponse>.Fail("ایمیل تکراری است، کاربر در سیستم وجود دارد");
        }
        var password = Password.Create(request.Password);
        var user = new User(request.FullName, email, password);

        var token = repoJWT.GenerateToken(user);
        var refreshToken = repoJWT.GenerateRefreshToken();
        var result = new TokenResponse(token,refreshToken,DateTime.Now.AddDays(7));

        user.SetRefreshToken(result.RefreshToken, result.ExpiresAt);
        var userId = await repoUser.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return Result<TokenResponse>.Success(result, "ثبت نام با موفقیت انجام شد");

    }
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        // پسورد و ایمیل چون ولیو ابجکست هستن، جداگانه ولیدیت میشن
        RuleFor(x => x.Password).NotEmpty().WithMessage("کلمه عبور الزامی است")
            .MinimumLength(6).WithMessage("حداقل طول پسورد 6 کلمه است")
            .MaximumLength(128).WithMessage("حداکثر طول پسورد 128 کاراکتر است");
        RuleFor(x => x.Email).NotEmpty().WithMessage("ایمیل اجباری است")
            .Must(BeAValidEmail).WithMessage("فرمت ایمیل صحیح نیست"); // اگه فرمت ایمیل درست نباشه یا با استانداردی که تعریف کردیم مطابقت نداشته باشه، خطا میده
        RuleFor(x => x.FullName).NotEmpty().WithMessage("نام کاربر اجباری است");

    }

    private bool BeAValidEmail(string email)
    {
        try
        {
            new Email(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
