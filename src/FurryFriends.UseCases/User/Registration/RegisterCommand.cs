using Ardalis.Result;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using FurryFriends.Core.Interfaces;

namespace FurryFriends.UseCases.Userauthentication.Registration;

public class RegisterCommand : IRequest<Result<Guid>>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

