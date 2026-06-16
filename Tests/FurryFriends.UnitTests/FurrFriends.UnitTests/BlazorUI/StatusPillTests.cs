using Bunit;
using FurryFriends.BlazorUI.Client.Components.Common;

namespace FurryFriends.UnitTests.BlazorUI;

public class StatusPillTests : TestContext
{
    [Fact]
    public void RendersStatusClassAndLabel()
    {
        var cut = RenderComponent<StatusPill>(parameters => parameters
            .Add(p => p.Status, "Active")
            .Add(p => p.Label, "4"));

        cut.Markup.Should().Contain("status-pill status-pill-active");
        cut.Markup.Should().Contain("4");
    }

    [Theory]
    [InlineData("Active", "status-pill-active")]
    [InlineData("Inactive", "status-pill-inactive")]
    [InlineData("Verified", "status-pill-verified")]
    [InlineData("", "status-pill-default")]
    public void NormalizesStatusClass(string status, string expectedClass)
    {
        var cut = RenderComponent<StatusPill>(parameters => parameters
            .Add(p => p.Status, status));

        cut.Markup.Should().Contain(expectedClass);
    }
}
