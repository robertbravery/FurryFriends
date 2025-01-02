using Ardalis.Result;
using FastEndpoints;
using FastEndpoints.Testing;
using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Users.ListUser;
using FurryFriends.Web.Endpoints.UserEndpoints.List;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FurryFriends.UnitTests.Web;

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
    response.Should().NotBeNull();
    if (response != null)
    {
      response.TotalCount.Should().Be(users.Count);
      response.RowsData.Should().HaveCount(users.Count);
      response.RowsData[0].Name.Should().Be(users[0].Name.FullName);
      response.RowsData[1].Name.Should().Be(users[1].Name.FullName);
    }
  }

  private async Task<List<User>> GetFakeUsers()
  {
    var phoneNumberValidator = new PhoneNumberValidator();
    var nameValidator = new NameValidator();
    
    var phoneNumber1 = (await PhoneNumber.Create("1", "011-123-4567", phoneNumberValidator)).Value;
    var phoneNumber2 = (await PhoneNumber.Create("1", "011-123-4567", phoneNumberValidator)).Value;
    var address1 = Address.Create(Fake.Address.StreetAddress(), Fake.Address.City(), Fake.Address.State(), Fake.Address.Country(), Fake.Address.ZipCode());
    var address2 = Address.Create(Fake.Address.StreetAddress(), Fake.Address.City(), Fake.Address.State(), Fake.Address.Country(), Fake.Address.ZipCode());
    var name1 = Name.Create(Fake.Name.FirstName(), Fake.Name.LastName(), nameValidator);
    var name2 = Name.Create(Fake.Name.FirstName(), Fake.Name.LastName(), nameValidator);

    var users = new List<User>
      {
          User.Create (name1, Fake.Internet.Email(), phoneNumber1, address1),
          User.Create(name2, Fake.Internet.Email(), phoneNumber2, address2),
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
    await ep.HandleAsync(request, CancellationToken.None);

    // Assert
    _mediatorMock.Verify(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    ep.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

  }
}
