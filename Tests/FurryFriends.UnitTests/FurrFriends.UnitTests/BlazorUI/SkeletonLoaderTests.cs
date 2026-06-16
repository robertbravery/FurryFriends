using Bunit;
using FurryFriends.BlazorUI.Client.Components.Common;

namespace FurryFriends.UnitTests.BlazorUI;

public class SkeletonLoaderTests : TestContext
{
    [Fact]
    public void RendersExpectedTableSkeletonCellCount()
    {
        var cut = RenderComponent<SkeletonLoader>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Table)
            .Add(p => p.Rows, 2)
            .Add(p => p.Columns, 3));

        cut.FindAll(".skeleton-cell").Should().HaveCount(9);
        cut.Markup.Should().Contain("skeleton-table");
    }

    [Fact]
    public void RendersCardSkeletonGrid()
    {
        var cut = RenderComponent<SkeletonLoader>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Card)
            .Add(p => p.Count, 2));

        cut.FindAll(".skeleton-card").Should().HaveCount(2);
        cut.Markup.Should().Contain("skeleton-card-grid");
    }
}
