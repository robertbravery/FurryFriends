using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FurryFriends.BlazorUI.Client.Components;

public class ObjectGraphDataAnnotationsValidatorOld : ComponentBase
{
  [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;
  [CascadingParameter] private EditContext? CurrentEditContext { get; set; }

  protected override void OnInitialized()
  {
    if (CurrentEditContext == null)
    {
      throw new InvalidOperationException($"{nameof(ObjectGraphDataAnnotationsValidator)} requires a cascading " +
          $"parameter of type {nameof(EditContext)}. For example, you can use this component inside an " +
          $"EditForm.");
    }
    CurrentEditContext.EnableDataAnnotationsValidation(ServiceProvider);
  }
}

