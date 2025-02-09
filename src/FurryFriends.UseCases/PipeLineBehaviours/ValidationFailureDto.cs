namespace FurryFriends.UseCases.PipeLineBehaviours;

public class ValidationFailureDto //ToDp: Check if we need this
{
  public string FieldName { get; set; } = default!;
  public string ErrorMessage { get; set; } = default!;
  public string ErrorCode { get; set; } = default!; // Optional (can include for more context)
}
