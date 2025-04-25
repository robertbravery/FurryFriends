using System;
using System.Collections.Generic;

namespace FurryFriends.BlazorUI.Client.Models.PetWalkers;

public class PetWalkerRecord
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public IEnumerable<string>? Locations { get; set; }
    public PhotoRecordDto? BioPicture { get; set; }
    public IEnumerable<PhotoRecordDto>? Photos { get; set; }
}

public class PhotoRecordDto
{
    public string Url { get; set; } = string.Empty;
    public string? Desciption { get; set; }
}
