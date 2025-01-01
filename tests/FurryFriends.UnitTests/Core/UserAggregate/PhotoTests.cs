using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UnitTests.Core.UserAggregate;

public class PhotoTests
{
  [Theory]
  [InlineData("https://example.com/photo.jpg")]
  [InlineData("http://test.com/image.png")]
  [InlineData("https://photos.site.com/pic.jpeg")]
  [InlineData("http://mysite.com/photo.gif")]
  public void Constructor_WithValidUrl_CreatesPhoto(string validUrl)
  {
    var photo = new Photo(validUrl);

    photo.Url.Should().Be(validUrl);
  }

  [Theory]
  [InlineData("")]
  [InlineData("not-a-url")]
  [InlineData("http://bad-url")]
  [InlineData("https://example.com/photo.txt")]//Only allow jpg, jpeg, png, gif
  [InlineData("ftp://example.com/photo.jpg")]//No ftp
  public void Constructor_WithInvalidUrl_ThrowsException(string invalidUrl)
  {
    var act = () => new Photo(invalidUrl);

    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void Constructor_WithValidHttpsUrl_CreatesPhoto()
  {
    const string validUrl = "https://example.com/photo.jpg";

    var photo = new Photo(validUrl);

    photo.Should().NotBeNull();
    photo.Url.Should().Be(validUrl);
  }
}
