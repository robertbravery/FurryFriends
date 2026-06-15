using System.IO;
using FluentAssertions;

namespace FurryFriends.FunctionalTests;

public class UIThemeTests
{
    [Fact]
    public void HslThemeVariablesDefineRequiredLightAndDarkBases()
    {
        var colorTheme = ReadFeatureFile("src/FurryFriends.BlazorUI/wwwroot/css/color-theme.css");

        colorTheme.Should().Contain("--primary-dark: hsl(200,100%,50%);");
        colorTheme.Should().Contain("--primary-light: hsl(340,100%,90%);");
        colorTheme.Should().Contain("[data-theme=\"dark\"]");
        colorTheme.Should().Contain("[data-theme=\"light\"]");
    }

    [Fact]
    public void AppStylesheetImportsModernizedTheme()
    {
        var appCss = ReadFeatureFile("src/FurryFriends.BlazorUI/wwwroot/app.css");

        appCss.Should().Contain("@import \"color-theme.css\";");
        appCss.Should().Contain("--primary-500");
    }

    [Fact]
    public void FontAwesomeSixStylesheetIsLoadedInAppShell()
    {
        var appRazor = ReadFeatureFile("src/FurryFriends.BlazorUI/Components/App.razor");

        appRazor.Should().Contain("font-awesome/6.4.0/css/all.min.css");
    }

    private static string ReadFeatureFile(string relativePath)
    {
        var root = FindRepoRoot();
        return File.ReadAllText(Path.Combine(root, relativePath));
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FurryFriends.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root.");
    }
}
