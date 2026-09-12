using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

public class DeletePetWalkerDateExceptionHandler
    : ICommandHandler<DeletePetWalkerDateExceptionCommand, Result>
{
  private readonly IRepository<PetWalkerDateException> _exceptionRepo;
  private readonly ILogger<DeletePetWalkerDateExceptionHandler> _logger;

  public DeletePetWalkerDateExceptionHandler(
      IRepository<PetWalkerDateException> exceptionRepo,
      ILogger<DeletePetWalkerDateExceptionHandler> logger)
  {
    _exceptionRepo = exceptionRepo;
    _logger = logger;
  }

  public async Task<Result> Handle(
      DeletePetWalkerDateExceptionCommand request,
      CancellationToken cancellationToken)
  {
    var date = DateOnly.FromDateTime(request.Date);

    var existing = await _exceptionRepo.ListAsync(
        new PetWalkerDateExceptionByPetWalkerAndDateSpec(request.PetWalkerId, date),
        cancellationToken);

    var current = existing.FirstOrDefault();

    if (current == null)
    {
      return Result.NotFound($"No date exception for {date:yyyy-MM-dd}");
    }

    await _exceptionRepo.DeleteAsync(current, cancellationToken);

    _logger.LogInformation(
        "Removed date exception for PetWalker {PetWalkerId} on {Date:yyyy-MM-dd}",
        request.PetWalkerId, date);

    return Result.Success();
  }
}
