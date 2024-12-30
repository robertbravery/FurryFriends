using Ardalis.Result;
using FastEndpoints;
using FastEndpoints.Testing;
using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Users.List;
using FurryFriends.Web.Endpoints.UserEndpoints.List;
using Microsoft.AspNetCore.Http;
using Moq;

public class ListUsersTests : TestBase
{
  private readonly Mock<IMediator> _mediatorMock;  
  private ILogger<ListUser> _logggerMock;

  public ListUsersTests()
  {
    _mediatorMock = new Mock<IMediator>();

    _logggerMock = Mock.Of<ILogger<ListUser>>();
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnUsers_WhenUsersExist()
  {
    // Arrange

    var request = new ListUsersRequest { SearchTerm = "test", Page = 1, PageSize = 10 };
    var users = await GetFakeUsers();
    var result = Result<(List<User> Users, int TotalCount)>.Success((users, users.Count));

    _mediatorMock.Setup(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);
    var ep = Factory.Create<ListUser>(_mediatorMock.Object, _logggerMock);
    // Act
    await ep.HandleAsync(request, CancellationToken.None);


    // Assert
    //var response = _listUsers.Response;
    var response = ep.Response;
    Assert.NotNull(response);
    if (response != null)
    {
      Assert.Equal(users.Count, response.TotalCount);
      Assert.Equal(users.Count, response.RowsData.Count);
      Assert.Equal(users[0].Name, response.RowsData[0].Name);
      Assert.Equal(users[1].Name, response.RowsData[1].Name);
    }
  }

  private async Task<List<User>> GetFakeUsers()
  {
    var phoneNumberValidator = new PhoneNumberValidator();
    ;
    var phoneNumber1 = (await PhoneNumber.Create("1", "123", "4567890", phoneNumberValidator)).Value;
    var phoneNumber2 = (await PhoneNumber.Create("1", "098", "7654321", phoneNumberValidator)).Value;
    var address1 = new Address(Fake.Address.StreetAddress(), Fake.Address.City(), Fake.Address.State(), Fake.Address.ZipCode());
    var address2 = new Address(Fake.Address.StreetAddress(), Fake.Address.City(), Fake.Address.State(), Fake.Address.ZipCode());
    var users = new List<User>
        {
            new (Fake.Name.FullName(), Fake.Internet.Email(), phoneNumber1, address1),
            new (Fake.Name.FullName(), Fake.Internet.Email(), phoneNumber2, address1),
        };
    return users;
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnError_WhenUsersDoNotExist()
  {
    // Arrange
    var request = new ListUsersRequest { SearchTerm = null, Page = 1, PageSize = 10 };
    var expectedResult = Result<(List<User> Users, int TotalCount)>.Error("Failed to retrieve users");

    _mediatorMock.Setup(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);
    var ep = Factory.Create<ListUser>(_mediatorMock.Object, _logggerMock);

    // Act
    //await _listUsers.HandleAsync(request, CancellationToken.None);
    await ep.HandleAsync(request, CancellationToken.None);



    // Assert
    _mediatorMock.Verify(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    Assert.True(ep.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound);

  }
}
