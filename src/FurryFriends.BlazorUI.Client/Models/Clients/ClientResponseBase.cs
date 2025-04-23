namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ClientResponseBase
{
  public bool Success { get; set; }
  public string Message { get; set; } = default!;
  public ClientData Data { get; set; } = default!;
  public object Errors { get; set; } = default!;
  public object ErrorCode { get; set; } = default!;
  public DateTime timestamp { get; set; }
}
