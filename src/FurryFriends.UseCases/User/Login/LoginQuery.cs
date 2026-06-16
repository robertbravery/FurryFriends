using Ardalis.Result;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.UserAggregate.Specifications;
using FurryFriends.Infrastructure.Services;

namespace FurryFriends.UseCases.User.Login;

public class LoginQuery : IRequest<Result<LoginResponse>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    private readonly IRepository<User> _repository;
    private readonly JwtTokenService _jwtTokenService;

    public LoginQueryHandler(IRepository<User> repository, JwtTokenService jwtTokenService)
    {
        _repository = repository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(request.Email, nameof(request.Email));
        Guard.Against.NullOrWhiteSpace(request.Password, nameof(request.Password));

        var spec = new UserByEmailSpec(request.Email);
        var user = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

        if (user == null)
        {
            return Result.Unauthorized("Invalid credentials");
        }

        // TODO: Verify password hash
        // if (!VerifyPassword(request.Password, user.PasswordHash))
        //     return Result.Unauthorized("Invalid credentials");

        var token = _jwtTokenService.GenerateToken(user.Email, user.Role);

        return new LoginResponse(token, user.Role, user.Id);
    }
}

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}