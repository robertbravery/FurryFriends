using Ardalis.Result;
using FastEndpoints;
using FastEndpoints.Testing;
using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Services.DataTransferObjects;
using FurryFriends.UseCases.Users.ListUser;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;
using FurryFriends.Web.Endpoints.UserEndpoints.List;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FurryFriends.UnitTests.Web.PetWalkerTests;

public class ListPetWalkerTests : TestBase
{
  private readonly Mock<IMediator> _mediatorMock;
  private ILogger<ListPetWalker> _logggerMock;

  public ListPetWalkerTests()
  {
    _mediatorMock = new Mock<IMediator>();

    _logggerMock = Mock.Of<ILogger<ListPetWalker>>();
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnUsers_WhenUsersExist()
  {
    // Arrange
    var request = new ListUsersRequest { SearchTerm = "test", Page = 1, PageSize = 10 };
    var users = await GetFakeUsers();
    var userListDto = new UserListDto(users, users.Count);
    var result = Result<UserListDto>.Success(userListDto);

    _mediatorMock.Setup(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);
    var ep = Factory.Create<ListPetWalker>(_mediatorMock.Object, _logggerMock);
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

  private async Task<List<PetWalker>> GetFakeUsers()
  {
    var phoneNumberValidator = new PhoneNumberValidator();
    var nameValidator = new NameValidator();

    var phoneNumber1 = (await PhoneNumber.Create("1", "011-123-4567", phoneNumberValidator)).Value;
    var phoneNumber2 = (await PhoneNumber.Create("1", "011-123-4567", phoneNumberValidator)).Value;
    var address1 = Address.Create(Fake.Address.StreetAddress(), Fake.Address.City(), Fake.Address.State(), Fake.Address.Country(), Fake.Address.ZipCode());
    var address2 = Address.Create(Fake.Address.StreetAddress(), Fake.Address.City(), Fake.Address.State(), Fake.Address.Country(), Fake.Address.ZipCode());
    var name1 = Name.Create(Fake.Name.FirstName(), Fake.Name.LastName(), nameValidator);
    var name2 = Name.Create(Fake.Name.FirstName(), Fake.Name.LastName(), nameValidator);
    var emaail1 = Email.Create(Fake.Internet.Email());
    var emaail2 = Email.Create(Fake.Internet.Email());

    var users = new List<PetWalker>
      {
          PetWalker.Create (name1,emaail1 , phoneNumber1, address1),
          PetWalker.Create(name2, emaail2, phoneNumber2, address2),
      };
    return users;
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnError_WhenUsersDoNotExist()
  {
    // Arrange
    var request = new ListUsersRequest { SearchTerm = null, Page = 1, PageSize = 10 };
    var expectedResult = Result<UserListDto>.Error("Failed to retrieve users");

    _mediatorMock.Setup(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);
    var ep = Factory.Create<ListPetWalker>(_mediatorMock.Object, _logggerMock);

    // Act
    await ep.HandleAsync(request, CancellationToken.None);

    // Assert
    _mediatorMock.Verify(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    ep.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

  }
}
