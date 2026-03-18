using Ardalis.Result;
using Ardalis.Specification;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using Microsoft.Extensions.Logging;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

internal class DeleteWorkingHoursHandler : ICommandHandler<DeleteWorkingHoursCommand, Result<bool>>
{
    private readonly IRepository<WorkingHoursEntity> _workingHoursRepository;
    private readonly ILogger<DeleteWorkingHoursHandler> _logger;

    public DeleteWorkingHoursHandler(IRepository<WorkingHoursEntity> workingHoursRepository, ILogger<DeleteWorkingHoursHandler> logger)
    {
        _workingHoursRepository = workingHoursRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteWorkingHoursCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var spec = new WorkingHoursByIdSpec(request.Id);
            var workingHours = await _workingHoursRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (workingHours == null)
            {
                return Result<bool>.NotFound("Working hours not found");
            }

            await _workingHoursRepository.DeleteAsync(workingHours, cancellationToken);

            _logger.LogInformation("Deleted working hours {Id}", request.Id);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting working hours {Id}", request.Id);
            return Result<bool>.Error(ex.Message);
        }
    }
}
