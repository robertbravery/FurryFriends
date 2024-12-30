using FurryFriends.Web.Endpoints.UserEndpoints.List;

namespace FurryFriends.FunctionalTests.ApiEndpoints.User;

//[Collection("Sequential")]
public class UserList(CustomWebApplicationFactory<Program> factory) :  IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task ReturnsTwoUsers()
  {
    //arrange
    var url = "/users";
    var page = 1;
    var pageSize = 2;
    var expectedCount = 2;
    var endpoint = $"{url}/{ page}/{pageSize}";

    //act
    var result =  await _client.GetAndDeserializeAsync<ListUsersResponse>(endpoint);

    //assert
    Assert.NotNull(result);
    Assert.Equal(expectedCount, result.RowsData.Count);
  }
}
