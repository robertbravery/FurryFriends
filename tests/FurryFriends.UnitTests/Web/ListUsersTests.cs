using System.Reflection;
using Ardalis.Result;
using FastEndpoints;
using FurryFriends.Core.Entities;
using FurryFriends.Core.ValueObjects;
using FurryFriends.Core.ValueObjects.Validators;
using FurryFriends.UseCases.Users.List;
using FurryFriends.Web.Endpoints.UserEndpoints.List;
using Microsoft.AspNetCore.Http;
using Moq;

public class ListUsersTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ListUser _listUsers;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<HttpContext> _httpContextMock;
    private readonly Mock<HttpResponse> _httpResponseMock;

    public ListUsersTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextMock = new Mock<HttpContext>();
        _httpResponseMock = new Mock<HttpResponse>();

        _httpContextMock.SetupGet(x => x.Response).Returns(_httpResponseMock.Object);
        _httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(_httpContextMock.Object);

        _listUsers = new ListUser(_httpContextAccessorMock.Object, _mediatorMock.Object);
         var httpContextField = typeof(BaseEndpoint).GetProperty("HttpContext", BindingFlags.Instance | BindingFlags.NonPublic);
        httpContextField?.SetValue(_listUsers, _httpContextMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUsers_WhenUsersExist()
    {
        // Arrange
        var request = new ListUsersRequest { SearchTerm = "test", Page = 1, PageSize = 10 };
        var phoneNumberValidator = new PhoneNumberValidator();
        var phoneNumber1 = (await PhoneNumber.Create("1", "123", "4567890", phoneNumberValidator)).Value;
        var phoneNumber2 = (await PhoneNumber.Create("1", "098", "7654321", phoneNumberValidator)).Value;
        var users = new List<User>
        {
            new User("John Doe", "john.doe@example.com", phoneNumber1, new Address("123 Street", "City", "State", "12345")),
            new User("Jane Doe", "jane.doe@example.com", phoneNumber2, new Address("456 Avenue", "City", "State", "67890"))
        };
        var result = Result<(List<User> Users, int TotalCount)>.Success((users, users.Count));

        _mediatorMock.Setup(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        await _listUsers.HandleAsync(request, CancellationToken.None);

        // Assert
        var response = _listUsers.Response.Value;
        Assert.NotNull(response);
        if (response != null)
        {
            Assert.Equal(users.Count, response.TotalCount);
            Assert.Equal(users.Count, response.RowsData.Count);
            Assert.Equal(users[0].Name, response.RowsData[0].Name);
            Assert.Equal(users[1].Name, response.RowsData[1].Name);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenUsersDoNotExist()
    {
        // Arrange
        var request = new ListUsersRequest { SearchTerm = null, Page = 1, PageSize = 10 };
        var result = Result<(List<User> Users, int TotalCount)>.Error("Failed to retrieve users");

        _mediatorMock.Setup(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        await _listUsers.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<ListUsersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(_listUsers.Response);
        Assert.False(_listUsers.Response.IsSuccess);
        Assert.Equal("Failed to retrieve users", _listUsers.Response.Errors.First());
    }
}
