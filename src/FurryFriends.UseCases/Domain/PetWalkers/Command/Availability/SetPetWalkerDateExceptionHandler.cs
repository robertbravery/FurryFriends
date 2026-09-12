using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

public class SetPetWalkerDateExceptionHandler
    : ICommandHandler<SetPetWalkerDateExceptionCommand, Result<PetWalkerDateExceptionDto>>
{
  private readonly IRepository<PetWalker> _walkerRepo;
  private readonly IRepository<PetWalkerDateException> _exceptionRepo;
  private readonly ILogger<SetPetWalkerDateExceptionHandler> _logger;

  public SetPetWalkerDateExceptionHandler(
      IRepository<PetWalker> walkerRepo,
      IRepository<PetWalkerDateException> exceptionRepo,
      ILogger<SetPetWalkerDateExceptionHandler> logger)
  {
    _walkerRepo = walkerRepo;
    _exceptionRepo = exceptionRepo;
    _logger = logger;
  }

  public async Task<Result<PetWalkerDateExceptionDto>> Handle(
      SetPetWalkerDateExceptionCommand request,
      CancellationToken cancellationToken)
  {
    if (request.PetWalkerId == Guid.Empty)
    {
      return Result<PetWalkerDateExceptionDto>.Error("PetWalker ID is required");
    }

    var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken);
    if (walker == null)
    {
      return Result<PetWalkerDateExceptionDto>.NotFound($"PetWalker {request.PetWalkerId} not found");
    }

    var date = DateOnly.FromDateTime(request.Date);

    var createResult = request.IsUnavailable
        ? PetWalkerDateException.CreateUnavailable(request.PetWalkerId, date, request.Reason)
        : PetWalkerDateException.CreateCustomHours(
            request.PetWalkerId,
            date,
            request.StartTime ?? default,
            request.EndTime ?? default,
            request.Reason);

    if (!createResult.IsSuccess)
    {
      return Result<PetWalkerDateExceptionDto>.Error(
          createResult.Errors.FirstOrDefault() ?? "Invalid date exception");
    }

    var replacement = createResult.Value;

    // One exception per walker per date: replace whatever is already there.
    var existing = await _exceptionRepo.ListAsync(
        new PetWalkerDateExceptionByPetWalkerAndDateSpec(request.PetWalkerId, date),
        cancellationToken);

    var current = existing.FirstOrDefault();

    if (current != null)
    {
      await _exceptionRepo.DeleteAsync(current, cancellationToken);

      _logger.LogInformation(
          "Replaced date exception for PetWalker {PetWalkerId} on {Date:yyyy-MM-dd}",
          request.PetWalkerId, date);
    }

    await _exceptionRepo.AddAsync(replacement, cancellationToken);

    _logger.LogInformation(
        "Set date exception for PetWalker {PetWalkerId} on {Date:yyyy-MM-dd}: {Kind}",
        request.PetWalkerId, date,
        replacement.IsUnavailable
            ? "unavailable all day"
            : $"{replacement.StartTime:HH\\:mm}-{replacement.EndTime:HH\\:mm}");

    return Result<PetWalkerDateExceptionDto>.Success(replacement.ToDto());
  }
}
