using System.Globalization;
using FluentAssertions;
using mostlylucid.pagingtaghelper.Services;

namespace mostlylucid.pagingtaghelper.tests.Services;

public class PagingLocalizerTests
{
    [Fact]
    public void FirstPageText_ReturnsDefaultValue()
    {
        // Arrange
        var localizer = new PagingLocalizer();

        // Act
        var result = localizer.FirstPageText;

        // Assert
        result.Should().Be("«");
    }

    [Fact]
    public void PreviousPageText_WithEnglishCulture_ReturnsEnglishText()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("en"));

        // Act
        var result = localizer.PreviousPageText;

        // Assert
        result.Should().Be("‹ Previous");
    }

    [Fact]
    public void NextPageText_WithSpanishCulture_ReturnsSpanishText()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("es"));

        // Act
        var result = localizer.NextPageText;

        // Assert
        result.Should().Be("Siguiente ›");
    }

    [Fact]
    public void PreviousPageText_WithFrenchCulture_ReturnsFrenchText()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("fr"));

        // Act
        var result = localizer.PreviousPageText;

        // Assert
        result.Should().Be("‹ Précédent");
    }

    [Fact]
    public void PageSizeLabel_WithGermanCulture_ReturnsGermanText()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("de"));

        // Act
        var result = localizer.PageSizeLabel;

        // Assert
        result.Should().Be("Seitengröße:");
    }

    [Fact]
    public void PageSizeLabel_WithItalianCulture_ReturnsItalianText()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("it"));

        // Act
        var result = localizer.PageSizeLabel;

        // Assert
        result.Should().Be("Dimensione pagina:");
    }

    [Fact]
    public void NextPageText_WithPortugueseCulture_ReturnsPortugueseText()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("pt"));

        // Act
        var result = localizer.NextPageText;

        // Assert
        result.Should().Be("Próximo ›");
    }

    [Fact]
    public void NextPageText_WithJapaneseCulture_ReturnsJapaneseText()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("ja"));

        // Act
        var result = localizer.NextPageText;

        // Assert
        result.Should().Be("次へ ›");
    }

    [Fact]
    public void PreviousPageText_WithChineseCulture_ReturnsChineseText()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("zh-Hans"));

        // Act
        var result = localizer.PreviousPageText;

        // Assert
        result.Should().Be("‹ 上一页");
    }

    [Fact]
    public void GetPageSummary_WithEnglishCulture_ReturnsFormattedEnglishString()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("en"));

        // Act
        var result = localizer.GetPageSummary(1, 10, 100);

        // Assert
        result.Should().Be("Page 1 of 10 (Total items: 100)");
    }

    [Fact]
    public void GetPageSummary_WithSpanishCulture_ReturnsFormattedSpanishString()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("es"));

        // Act
        var result = localizer.GetPageSummary(2, 5, 50);

        // Assert
        result.Should().Be("Página 2 de 5 (Total de elementos: 50)");
    }

    [Fact]
    public void GetPageSummary_WithFrenchCulture_ReturnsFormattedFrenchString()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("fr"));

        // Act
        var result = localizer.GetPageSummary(3, 7, 70);

        // Assert
        result.Should().Be("Page 3 sur 7 (Total des éléments: 70)");
    }

    [Fact]
    public void LastPageText_ReturnsDefaultValue()
    {
        // Arrange
        var localizer = new PagingLocalizer();

        // Act
        var result = localizer.LastPageText;

        // Assert
        result.Should().Be("»");
    }

    [Fact]
    public void SkipBackText_ReturnsDefaultValue()
    {
        // Arrange
        var localizer = new PagingLocalizer();

        // Act
        var result = localizer.SkipBackText;

        // Assert
        result.Should().Be("..");
    }

    [Fact]
    public void SkipForwardText_ReturnsDefaultValue()
    {
        // Arrange
        var localizer = new PagingLocalizer();

        // Act
        var result = localizer.SkipForwardText;

        // Assert
        result.Should().Be("..");
    }

    [Fact]
    public void NextPageAriaLabel_WithEnglishCulture_ReturnsEnglishLabel()
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo("en"));

        // Act
        var result = localizer.NextPageAriaLabel;

        // Assert
        result.Should().Be("go to next page");
    }

    [Theory]
    [InlineData("en", "Page size:")]
    [InlineData("es", "Tamaño de página:")]
    [InlineData("fr", "Taille de page:")]
    [InlineData("de", "Seitengröße:")]
    [InlineData("it", "Dimensione pagina:")]
    [InlineData("pt", "Tamanho da página:")]
    [InlineData("ja", "ページサイズ:")]
    [InlineData("zh-Hans", "页面大小:")]
    public void PageSizeLabel_WithVariousCultures_ReturnsCorrectTranslation(string culture, string expected)
    {
        // Arrange
        var localizer = new PagingLocalizer();
        localizer.SetCulture(CultureInfo.GetCultureInfo(culture));

        // Act
        var result = localizer.PageSizeLabel;

        // Assert
        result.Should().Be(expected);
    }
}
