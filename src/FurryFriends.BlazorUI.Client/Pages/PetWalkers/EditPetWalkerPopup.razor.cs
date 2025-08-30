using FurryFriends.BlazorUI.Client.Models.Locations;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Client.Pages.PetWalkers;

public partial class EditPetWalkerPopup
{
  [Parameter]
  public string PetWalkerEmail { get; set; } = default!;

  [Inject]
  public IPetWalkerService PetWalkerService { get; set; } = default!;

  [Inject]
  public ILocationService LocationService { get; set; } = default!;

  [Inject]
  public IScheduleService ScheduleService { get; set; } = default!;

  [Inject]
  public IPopupService PopupService { get; set; } = default!;

  [Parameter]
  public EventCallback OnPetWalkerUpdated { get; set; }

  private PetWalkerDetailDto petWalkerModel = default!;
  private bool isLoading = true;
  private string? loadError;
  private bool showSuccessMessage = false;

  // Location data
  private List<RegionDto> regions = new();
  private List<LocalityDto> localities = new();
  private Guid selectedRegionId = Guid.Empty;
  private Guid selectedLocalityId = Guid.Empty;

  // Schedule management
  private List<ScheduleItemDto> scheduleItems = new();

  protected override async Task OnInitializedAsync()
  {
    // Load pet walker data
    await LoadPetWalker();
  }

  private async Task LoadPetWalker()
  {
    try
    {
      isLoading = true;
      var result = await PetWalkerService.GetPetWalkerDetailsByEmailAsync(PetWalkerEmail);

      if (result.Success && result.Data != null)
      {
        petWalkerModel = result.Data;

        // Initialize service areas if null
        if (petWalkerModel.ServiceAreas == null)
        {
          petWalkerModel.ServiceAreas = new List<string>();
        }
        // If service areas come as a single string, split them
        else if (petWalkerModel.ServiceAreas.Count == 1 && petWalkerModel.ServiceAreas[0].Contains(","))
        {
          var areas = petWalkerModel.ServiceAreas[0]
            .Split(',')
            .Select(a => a.Trim())
            .Where(a => !string.IsNullOrEmpty(a))
            .ToList();
          petWalkerModel.ServiceAreas = areas;
        }

        // Initialize structured service areas
        petWalkerModel.StructuredServiceAreas = new List<ServiceAreaDto>();

        // Load regions first to prepare for mapping service areas
        await LoadRegions();

        // Convert string service areas to structured service areas
        await ConvertServiceAreasToStructured();

        // Load schedule data
        await LoadScheduleData(petWalkerModel.Id);
      }
      else
      {
        loadError = result.Message ?? "Failed to load pet walker details.";
      }
    }
    catch (Exception ex)
    {
      loadError = $"Error: {ex.Message}";
    }
    finally
    {
      isLoading = false;
    }
  }

  private async Task ConvertServiceAreasToStructured()
  {
    if (petWalkerModel == null || petWalkerModel?.ServiceAreas == null || !petWalkerModel.ServiceAreas.Any())
      return;

    // Clear existing structured service areas
    petWalkerModel.StructuredServiceAreas.Clear();

    // Create a dictionary to cache localities by region to avoid multiple API calls
    var localitiesByRegion = new Dictionary<Guid, List<LocalityDto>>();

    // For each service area (locality name), try to find the matching locality
    foreach (var serviceAreaName in petWalkerModel.ServiceAreas)
    {
      if (string.IsNullOrWhiteSpace(serviceAreaName))
        continue;

      bool found = false;

      // Find the locality in any region
      foreach (var region in regions)
      {
        // Load localities for this region (from cache if available)
        if (!localitiesByRegion.TryGetValue(region.Id, out var regionLocalities))
        {
          regionLocalities = await LocationService.GetLocalitiesByRegionAsync(region.Id);
          localitiesByRegion[region.Id] = regionLocalities;
        }

        // Find locality by name
        var locality = regionLocalities.FirstOrDefault(l =>
          l.LocalityName.Equals(serviceAreaName, StringComparison.OrdinalIgnoreCase));

        if (locality != null)
        {
          // Add to structured service areas
          petWalkerModel.StructuredServiceAreas.Add(new ServiceAreaDto
          {
            Id = Guid.NewGuid(), // Temporary ID
            LocalityId = locality.Id,
            LocalityName = locality.LocalityName,
            RegionId = region.Id,
            RegionName = region.RegionName
          });

          found = true;
          break; // Break the inner loop once we've found the locality
        }
      }

      // If we couldn't find a matching locality, create a placeholder
      if (!found && !string.IsNullOrWhiteSpace(serviceAreaName))
      {
        // Add a placeholder with just the name for display purposes
        petWalkerModel.StructuredServiceAreas.Add(new ServiceAreaDto
        {
          Id = Guid.NewGuid(), // Temporary ID
          LocalityId = Guid.Empty,
          LocalityName = serviceAreaName,
          RegionId = Guid.Empty,
          RegionName = "Unknown Region"
        });
      }
    }
  }

  private async Task HandleValidSubmit()
  {
    try
    {
      // First, update the basic pet walker information
      var result = await PetWalkerService.UpdatePetWalkerAsync(petWalkerModel!);
      if (!result.Success)
      {
        loadError = result.Message ?? "Failed to update pet walker.";
        StateHasChanged();
        return;
      }

      // Then, update the service areas
      if (petWalkerModel.StructuredServiceAreas.Any())
      {
        var serviceAreasResult = await PetWalkerService.UpdateServiceAreasAsync(
          petWalkerModel.Id,
          petWalkerModel.StructuredServiceAreas);

        if (!serviceAreasResult.Success)
        {
          loadError = serviceAreasResult.Message ?? "Failed to update service areas.";
          StateHasChanged();
          return;
        }
      }

      // Save schedule changes
      await SaveScheduleChanges();

      // Show success message and close popup
      // Notify parent that update was successful
      await OnPetWalkerUpdated.InvokeAsync();

      showSuccessMessage = true;
      StateHasChanged();

      // Auto-close the popup after a short delay
      await Task.Delay(1000); // Reduce delay to make update feel snappier
      PopupService.CloseEditPetWalkerPopup();
    }
    catch (Exception ex)
    {
      loadError = $"Error: {ex.Message}";
      StateHasChanged();
    }
  }

  private void OnCancel()
  {
    PopupService.CloseEditPetWalkerPopup();
  }

  private async Task LoadRegions()
  {
    try
    {
      regions = await LocationService.GetRegionsAsync();
    }
    catch (Exception ex)
    {
      loadError = $"Error loading regions: {ex.Message}";
    }
  }

  private async Task LoadLocalitiesByRegion(Guid regionId)
  {
    try
    {
      if (regionId != Guid.Empty)
      {
        localities = await LocationService.GetLocalitiesByRegionAsync(regionId);
      }
      else
      {
        localities = new List<LocalityDto>();
      }
    }
    catch (Exception ex)
    {
      loadError = $"Error loading localities: {ex.Message}";
    }
  }

  private async Task OnRegionChanged(ChangeEventArgs e)
  {
    if (Guid.TryParse(e.Value?.ToString(), out Guid regionId))
    {
      selectedRegionId = regionId;
      selectedLocalityId = Guid.Empty;
      await LoadLocalitiesByRegion(regionId);
    }
    else
    {
      selectedRegionId = Guid.Empty;
      localities = new List<LocalityDto>();
    }
  }

  private void OnLocalityChanged(ChangeEventArgs e)
  {
    if (Guid.TryParse(e.Value?.ToString(), out Guid localityId))
    {
      selectedLocalityId = localityId;
    }
    else
    {
      selectedLocalityId = Guid.Empty;
    }
  }

  private void AddServiceArea()
  {
    // Legacy service areas (for backward compatibility)
    if (petWalkerModel!.ServiceAreas.Count < 10) // Limit to 10 service areas
    {
      petWalkerModel.ServiceAreas.Add(string.Empty);
      StateHasChanged();
    }
  }

  private void RemoveServiceArea(int index)
  {
    if (index >= 0 && index < petWalkerModel!.ServiceAreas.Count)
    {
      petWalkerModel.ServiceAreas.RemoveAt(index);
      StateHasChanged();
    }
  }

  private void AddStructuredServiceArea()
  {
    if (selectedRegionId == Guid.Empty || selectedLocalityId == Guid.Empty)
    {
      loadError = "Please select both a region and a locality";
      return;
    }

    if (petWalkerModel!.StructuredServiceAreas.Count < 10) // Limit to 10 service areas
    {
      var region = regions.FirstOrDefault(r => r.Id == selectedRegionId);
      var locality = localities.FirstOrDefault(l => l.Id == selectedLocalityId);

      if (region != null && locality != null)
      {
        // Check if this locality is already added
        if (petWalkerModel.StructuredServiceAreas.Any(sa => sa.LocalityId == selectedLocalityId))
        {
          loadError = $"The locality '{locality.LocalityName}' is already added";
          return;
        }

        petWalkerModel.StructuredServiceAreas.Add(new ServiceAreaDto
        {
          Id = Guid.NewGuid(), // Temporary ID until saved
          LocalityId = selectedLocalityId,
          LocalityName = locality.LocalityName,
          RegionId = selectedRegionId,
          RegionName = region.RegionName
        });

        // Also add to legacy service areas for backward compatibility
        petWalkerModel.ServiceAreas.Add(locality.LocalityName);

        // Reset selection
        selectedLocalityId = Guid.Empty;

        StateHasChanged();
      }
    }
  }

  private void RemoveStructuredServiceArea(Guid serviceAreaId)
  {
    var serviceArea = petWalkerModel!.StructuredServiceAreas.FirstOrDefault(sa => sa.Id == serviceAreaId);
    if (serviceArea != null)
    {
      petWalkerModel.StructuredServiceAreas.Remove(serviceArea);

      // Also remove from legacy service areas
      var index = petWalkerModel.ServiceAreas.IndexOf(serviceArea.LocalityName);
      if (index >= 0)
      {
        petWalkerModel.ServiceAreas.RemoveAt(index);
      }
      else
      {
        // Try case-insensitive search if exact match not found
        index = petWalkerModel.ServiceAreas.FindIndex(sa =>
          sa.Equals(serviceArea.LocalityName, StringComparison.OrdinalIgnoreCase));
        if (index >= 0)
        {
          petWalkerModel.ServiceAreas.RemoveAt(index);
        }
      }

      StateHasChanged();
    }
  }

  private async Task LoadScheduleData(Guid petWalkerId)
  {
    try
    {
      var scheduleResponse = await ScheduleService.GetScheduleAsync(petWalkerId);

      if (scheduleResponse.Success && scheduleResponse.Data != null)
      {
        scheduleItems = scheduleResponse.Data.Schedules.ToList();
        petWalkerModel.Schedules = scheduleResponse.Data.Schedules;
      }
      else
      {
        scheduleItems = new List<ScheduleItemDto>();
        petWalkerModel.Schedules = new List<ScheduleItemDto>();
      }

      // Ensure we have entries for all days of the week
      foreach (var day in ScheduleHelper.WeekDays)
      {
        if (!scheduleItems.Any(s => s.DayOfWeek == day))
        {
          scheduleItems.Add(new ScheduleItemDto
          {
            DayOfWeek = day,
            StartTime = new TimeOnly(9, 0), // Default 9 AM
            EndTime = new TimeOnly(17, 0),   // Default 5 PM
            IsActive = false
          });
        }
      }

      // Sort by day of week (Monday first)
      scheduleItems = scheduleItems.OrderBy(s => (int)s.DayOfWeek == 0 ? 7 : (int)s.DayOfWeek).ToList();
    }
    catch (Exception ex)
    {
      // Log error but don't fail the entire load process
      scheduleItems = ScheduleHelper.CreateEmptyWeeklySchedule();
      petWalkerModel.Schedules = new List<ScheduleItemDto>();
      loadError = $"Error loading schedule: {ex.Message}";
    }
  }

  private async Task SaveScheduleChanges()
  {
    try
    {
      // Validate all active schedule items
      var activeSchedules = scheduleItems.Where(s => s.IsActive).ToList();
      var invalidSchedules = activeSchedules.Where(s => !ScheduleHelper.IsValidScheduleItem(s)).ToList();

      if (invalidSchedules.Any())
      {
        loadError = "Please fix the schedule validation errors before saving.";
        return;
      }

      var response = await ScheduleService.SetScheduleAsync(petWalkerModel.Id, scheduleItems);

      if (!response.Success)
      {
        loadError = response.Message ?? "Failed to save schedule changes.";
        return;
      }

      // Update the petWalkerModel.Schedules to reflect the saved changes
      petWalkerModel.Schedules = scheduleItems.Where(s => s.IsActive).ToList();
    }
    catch (Exception ex)
    {
      loadError = $"Error saving schedule: {ex.Message}";
    }
  }
}
