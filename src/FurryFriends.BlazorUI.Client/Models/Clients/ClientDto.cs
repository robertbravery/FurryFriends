namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ClientDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? EmailAddress { get; set; }
    public string? City { get; set; }
    public int TotalPets { get; set; }
    public Dictionary<string, int>? PetsBySpecies { get; set; }
}

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
