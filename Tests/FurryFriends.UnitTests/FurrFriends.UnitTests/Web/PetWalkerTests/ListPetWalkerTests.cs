using Ardalis.Result;
using Bogus;
using FastEndpoints;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.PetWalkers.ListPetWalker;
using FurryFriends.UseCases.Services.PetWalkerService;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FurryFriends.UnitTests.Web.PetWalkerTests;

public class ListPetWalkerTests
{
  private readonly Mock<IMediator> _mediatorMock;
  private readonly Faker _faker;
  private ILogger<ListPetWalker> _logggerMock;

  public ListPetWalkerTests()
  {
    _mediatorMock = new Mock<IMediator>();
    _faker = new Faker();
    _logggerMock = Mock.Of<ILogger<ListPetWalker>>();
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnUsers_WhenUsersExist()
  {
    // Arrange
    var request = new ListPetWalkerRequest { SearchTerm = "test", Page = 1, PageSize = 10 };
    var users = await GetFakeUsers();
    var userListDto = new PetWalkerListDto(users, users.Count);
    var result = Result<PetWalkerListDto>.Success(userListDto);

    _mediatorMock.Setup(m => m.Send(It.IsAny<ListPetWalkerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);
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

    var phoneNumber1 = (await PhoneNumber.Create("1", "011-123-4567")).Value;
    var phoneNumber2 = (await PhoneNumber.Create("1", "011-123-4567")).Value;
    var address1 = Address.Create(_faker.Address.StreetAddress(), _faker.Address.City(), _faker.Address.State(), _faker.Address.Country(), _faker.Address.ZipCode());
    var address2 = Address.Create(_faker.Address.StreetAddress(), _faker.Address.City(), _faker.Address.State(), _faker.Address.Country(), _faker.Address.ZipCode());
    var name1 = Name.Create(_faker.Name.FirstName(), _faker.Name.LastName());
    var name2 = Name.Create(_faker.Name.FirstName(), _faker.Name.LastName());
    var emaail1 = Email.Create(_faker.Internet.Email());
    var emaail2 = Email.Create(_faker.Internet.Email());

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
    var request = new ListPetWalkerRequest { SearchTerm = null, Page = 1, PageSize = 10 };
    var expectedResult = Result<PetWalkerListDto>.Error("Failed to retrieve users");

    _mediatorMock.Setup(m => m.Send(It.IsAny<ListPetWalkerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);
    var ep = Factory.Create<ListPetWalker>(_mediatorMock.Object, _logggerMock);

    // Act
    await ep.HandleAsync(request, CancellationToken.None);

    // Assert
    _mediatorMock.Verify(m => m.Send(It.IsAny<ListPetWalkerQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    ep.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

  }
}
