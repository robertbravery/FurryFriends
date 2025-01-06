﻿using FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

namespace FurryFriends.FunctionalTests.ApiEndpoints.PetWalker;

//[Collection("Sequential")]
public class PetWalkerListTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task ReturnsTwoUsers()
  {
    //arrange
    var url = "/petwalker/list";
    var page = 1;
    var pageSize = 2;
    var expectedCount = 2;
    var endpoint = $"{url}?page={page}&pageSize={pageSize}";

    //act
    var result = await _client.GetAndDeserializeAsync<ListPetWalkerResponse>(endpoint);

    //assert
    Assert.NotNull(result);
    Assert.Equal(expectedCount, result.RowsData.Count);
  }
}
