namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ListResponse
{
  public List<ClientDto>? RowsData { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public int TotalCount { get; set; }
  public int TotalPages { get; set; }
  public bool HasPreviousPage { get; set; }
  public bool HasNextPage { get; set; }
  public string[]? HideColumns { get; set; }
}



public class ClientResponseBase
{
  public bool Success { get; set; }
  public string Message { get; set; } = default!;
  public ClientData Data { get; set; } = default!;
  public object Errors { get; set; } = default!;
  public object ErrorCode { get; set; } = default!;
  public DateTime timestamp { get; set; }
}

public class ClientData
{
  public Guid Id { get; set; }
  public string Name { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Street { get; set; } = default!;
  public string City { get; set; } = default!;
  public string State { get; set; } = default!;
  public string ZipCode { get; set; } = default!;
  public int ClientType { get; set; }
  public string PreferredContactTime { get; set; } = default!;
  public int ReferralSource { get; set; }
  public Pet[] Pets { get; set; } = default!;

  public static ClientModel MapToModel(ClientData clientData)
  {
    return new ClientModel
    {
      FirstName = clientData.Name.Split(' ')[0],
      LastName = clientData.Name.Split(' ')[1],
      EmailAddress = clientData.Email,
      PhoneNumber = clientData.PhoneNumber,
      Address = new Address
      {
        Street = clientData.Street,
        City = clientData.City,
        State = clientData.State,
        ZipCode = clientData.ZipCode
      },
      //Pets = clientData.Pets
    };
  }
}

public class Pet
{
  public string Id { get; set; } = default!;
  public string Name { get; set; } = default!;
  public string Species { get; set; } = default!;
  public string Breed { get; set; } = default!;
  public int Age { get; set; }
  public int Weight { get; set; }
  public string SpecialNeeds { get; set; } = default!;
  public string MedicalConditions { get; set; } = default!;
  public bool isActive { get; set; }
}
