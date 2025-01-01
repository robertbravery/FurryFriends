using FluentValidation;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UnitTests.UseCases.Contributors;

public class CreateContributorHandlerHandle
{
  private readonly Name _testName = Name.Create("test", "test", new NameValidator()).Value;
  private readonly IRepository<Contributor> _repository = Substitute.For<IRepository<Contributor>>();
private readonly IValidator<PhoneNumber> _phoneNumberValidator = Substitute.For<IValidator<PhoneNumber>>();

  private CreateContributorHandler _handler;

  public CreateContributorHandlerHandle()
  {
    _handler = new CreateContributorHandler(_repository, _phoneNumberValidator);
  }

  private Contributor CreateContributor()
  {
    return new Contributor(_testName);
  }

  [Fact]
  public async Task ReturnsSuccessGivenValidName()
  {
    _repository.AddAsync(Arg.Any<Contributor>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(CreateContributor()));
    var result = await _handler.Handle(new CreateContributorCommand(_testName.FullName, null), CancellationToken.None);

    result.IsSuccess.Should().BeTrue();
  }
}
