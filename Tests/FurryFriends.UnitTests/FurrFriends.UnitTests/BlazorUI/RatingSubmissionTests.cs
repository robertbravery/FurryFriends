using Bunit;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using FurryFriends.BlazorUI.Components.Pages.Ratings;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FurryFriends.UnitTests.BlazorUI;

public class RatingSubmissionTests : TestContext
{
  private readonly Mock<IRatingService> _mockRatingService;
  private readonly Mock<ILogger<RatingSubmission>> _mockLogger;
  private readonly Guid _petWalkerId;
  private readonly Guid _clientId;

  public RatingSubmissionTests()
  {
    _mockRatingService = new Mock<IRatingService>();
    _mockLogger = new Mock<ILogger<RatingSubmission>>();
    _petWalkerId = Guid.NewGuid();
    _clientId = Guid.NewGuid();

    Services.AddSingleton(_mockRatingService.Object);
    Services.AddSingleton(_mockLogger.Object);
  }

  [Fact]
  public void ShouldRenderStarSelector()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId));

    // Assert - 5 star buttons
    var starButtons = cut.FindAll(".star-btn");
    Assert.Equal(5, starButtons.Count);

    // Default label text
    Assert.Contains("Select a rating", cut.Markup);
  }

  [Fact]
  public void ShouldRenderCommentTextarea()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId));

    // Assert
    var textarea = cut.Find("textarea#comment");
    Assert.NotNull(textarea);
    Assert.Equal("1000", textarea.GetAttribute("maxlength"));
    Assert.Equal("Share your experience...", textarea.GetAttribute("placeholder"));
  }

  [Fact]
  public void ShouldRenderBookingIdFieldByDefault()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId));

    // Assert
    var bookingField = cut.Find("input#bookingId");
    Assert.NotNull(bookingField);
  }

  [Fact]
  public void ShouldHideBookingFieldWhenShowBookingFieldIsFalse()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ShowBookingField, false));

    // Assert
    Assert.Equal(0, cut.FindAll("input#bookingId").Count);
  }

  [Fact]
  public void ShouldShowSubmitButton()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId));

    // Assert
    var submitButton = cut.Find("button.btn.btn-primary");
    Assert.NotNull(submitButton);
    Assert.Contains("Submit Rating", submitButton.TextContent);
  }

  [Fact]
  public void ShouldShowCancelButtonWhenShowCancelButtonIsTrue()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ShowCancelButton, true));

    // Assert
    var cancelButton = cut.Find("button.btn-outline-secondary");
    Assert.NotNull(cancelButton);
    Assert.Contains("Cancel", cancelButton.TextContent);
  }

  [Fact]
  public void ShouldHideCancelButtonByDefault()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId));

    // Assert
    Assert.Equal(0, cut.FindAll("button.btn-outline-secondary").Count);
  }

  [Fact]
  public void ShouldValidateRatingSelection()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId));

    // Submit without selecting a rating
    var submitButton = cut.Find("button.btn.btn-primary");
    submitButton.Click();

    // Assert - error message should appear
    Assert.Contains("Please select a rating between 1 and 5 stars.", cut.Markup);
  }

  [Fact]
  public void ShouldSelectRatingOnStarClick()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId));

    // Click the 4th star
    var starButtons = cut.FindAll(".star-btn");
    starButtons[3].Click();

    // Assert - 4 stars should be active, rating label should show "Good"
    var activeStars = cut.FindAll(".star-active");
    Assert.Equal(4, activeStars.Count);
    Assert.Contains("Good", cut.Markup);
  }

  [Fact]
  public void ShouldShowSuccessMessageOnSuccessfulCreate()
  {
    // Arrange
    _mockRatingService
      .Setup(s => s.CreateRatingAsync(It.IsAny<CreateRatingRequest>()))
      .ReturnsAsync(new RatingResult(true, null));

    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ShowBookingField, false));

    // Select a rating
    var starButtons = cut.FindAll(".star-btn");
    starButtons[4].Click(); // 5 stars

    // Submit
    var submitButton = cut.Find("button.btn.btn-primary");
    submitButton.Click();

    // Assert
    Assert.Contains("Your rating has been submitted successfully!", cut.Markup);
  }

  [Fact]
  public void ShouldShowErrorMessageOnFailedCreate()
  {
    // Arrange
    _mockRatingService
      .Setup(s => s.CreateRatingAsync(It.IsAny<CreateRatingRequest>()))
      .ReturnsAsync(new RatingResult(false, "Service unavailable"));

    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ShowBookingField, false));

    // Select a rating
    var starButtons = cut.FindAll(".star-btn");
    starButtons[2].Click(); // 3 stars

    // Submit
    var submitButton = cut.Find("button.btn.btn-primary");
    submitButton.Click();

    // Assert
    Assert.Contains("Service unavailable", cut.Markup);
  }

  [Fact]
  public void ShouldShowDeleteButtonWhenExistingRatingIdIsSet()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ExistingRatingId, Guid.NewGuid())
      .Add(p => p.CanDelete, true));

    // Assert
    var deleteButton = cut.Find("button.btn-outline-danger");
    Assert.NotNull(deleteButton);
    Assert.Contains("Delete Rating", deleteButton.TextContent);
  }

  [Fact]
  public void ShouldShowEditModeHeaderAndButtonText()
  {
    // Act
    var existingId = Guid.NewGuid();
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ExistingRatingId, existingId));

    // Assert - edit mode header and submit button text
    Assert.Contains("Edit Your Rating", cut.Markup);
    Assert.Contains("Save Changes", cut.Markup);
  }

  [Fact]
  public void ShouldPrepopulateExistingValuesInEditMode()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ExistingRatingId, Guid.NewGuid())
      .Add(p => p.ExistingRatingValue, 3)
      .Add(p => p.ExistingComment, "Good walker"));

    // Assert - 3 stars active, comment pre-filled
    var activeStars = cut.FindAll(".star-active");
    Assert.Equal(3, activeStars.Count);

    var textarea = cut.Find("textarea#comment");
    Assert.Equal("Good walker", textarea.GetAttribute("value"));
  }

  [Fact]
  public void ShouldInvokeOnCancelWhenCancelClicked()
  {
    // Arrange
    var cancelInvoked = false;
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ShowCancelButton, true)
      .Add(p => p.OnCancel, EventCallback.Factory.Create(this, () => cancelInvoked = true)));

    // Act
    var cancelButton = cut.Find("button.btn-outline-secondary");
    cancelButton.Click();

    // Assert
    Assert.True(cancelInvoked);
  }

  [Fact]
  public void ShouldShowErrorWhenBookingIdIsEmpty()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ShowBookingField, true));

    // Select rating
    var starButtons = cut.FindAll(".star-btn");
    starButtons[0].Click();

    // Submit without entering booking ID
    var submitButton = cut.Find("button.btn.btn-primary");
    submitButton.Click();

    // Assert
    Assert.Contains("Please enter a Booking ID.", cut.Markup);
  }

  [Fact]
  public void ShouldShowErrorWhenBookingIdIsInvalid()
  {
    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ShowBookingField, true));

    // Select rating
    var starButtons = cut.FindAll(".star-btn");
    starButtons[1].Click();

    // Enter invalid booking ID
    var bookingInput = cut.Find("input#bookingId");
    bookingInput.Change("not-a-guid");

    // Submit
    var submitButton = cut.Find("button.btn.btn-primary");
    submitButton.Click();

    // Assert
    Assert.Contains("Please enter a valid Booking ID.", cut.Markup);
  }

  [Fact]
  public void ShouldShowSuccessMessageOnSuccessfulDelete()
  {
    // Arrange
    var existingRatingId = Guid.NewGuid();
    _mockRatingService
      .Setup(s => s.DeleteRatingAsync(existingRatingId, _clientId))
      .ReturnsAsync(new RatingResult(true, null));

    // Act
    var cut = RenderComponent<RatingSubmission>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.ClientId, _clientId)
      .Add(p => p.ExistingRatingId, existingRatingId)
      .Add(p => p.CanDelete, true));

    // Click delete
    var deleteButton = cut.Find("button.btn-outline-danger");
    deleteButton.Click();

    // Assert
    Assert.Contains("Your rating has been deleted.", cut.Markup);
  }
}
