# Localization

The Paging TagHelper includes comprehensive localization support with 8 languages built-in, plus the ability to customize any text or create your own localizations.

## Table of Contents

- [Built-in Languages](#built-in-languages)
- [Quick Start](#quick-start)
- [Customizing Text](#customizing-text)
- [Custom Summary Templates](#custom-summary-templates)
- [Adding New Languages](#adding-new-languages)
- [Localization Architecture](#localization-architecture)
- [All Localizable Strings](#all-localizable-strings)

## Built-in Languages

The library includes complete translations for:

| Language Code | Language | Native Name |
|--------------|----------|-------------|
| `en` | English | English |
| `de` | German | Deutsch |
| `es` | Spanish | Español |
| `fr` | French | Français |
| `it` | Italian | Italiano |
| `pt` | Portuguese | Português |
| `ja` | Japanese | 日本語 |
| `zh-Hans` | Chinese (Simplified) | 简体中文 |

## Quick Start

### Use Current Culture

By default, the pager uses the current UI culture from `CultureInfo.CurrentUICulture`:

```csharp
// In your Startup.cs or Program.cs
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("fr"),
    SupportedCultures = new[] { new CultureInfo("en"), new CultureInfo("fr"), new CultureInfo("de") },
    SupportedUICultures = new[] { new CultureInfo("en"), new CultureInfo("fr"), new CultureInfo("de") }
});
```

```html
<!-- Pager automatically uses current culture -->
<paging model="Model" />
```

### Specify Language Explicitly

Override the current culture for a specific pager:

```html
<!-- French -->
<paging model="Model" language="fr" />

<!-- German -->
<paging model="Model" language="de" />

<!-- Japanese -->
<paging model="Model" language="ja" />
```

## Customizing Text

You can override any text while keeping other localized strings:

### Override Individual Text Properties

```html
<paging model="Model"
        language="fr"
        first-page-text="Début"
        last-page-text="Fin" />
```

This example uses French for all text EXCEPT the first/last page buttons, which use your custom text.

### Override All Button Text

```html
<paging model="Model"
        first-page-text="⏮"
        previous-page-text="◀ Prev"
        next-page-text="Next ▶"
        last-page-text="⏭"
        skip-back-text="..."
        skip-forward-text="..." />
```

## Custom Summary Templates

The summary text supports placeholders for dynamic content:

### Default Summary

Without customization, you get localized summaries like:

**English:** "Page 2 of 10 (Total items: 156)"
**French:** "Page 2 sur 10 (Articles totaux : 156)"
**German:** "Seite 2 von 10 (Gesamtanzahl: 156)"
**Spanish:** "Página 2 de 10 (Elementos totales: 156)"

### Custom Template

```html
<paging model="Model"
        summary-template="Showing {startItem}-{endItem} of {totalItems} items" />
```

### Available Placeholders

| Placeholder | Description | Example |
|------------|-------------|---------|
| `{currentPage}` | Current page number | `2` |
| `{totalPages}` | Total number of pages | `10` |
| `{totalItems}` | Total number of items | `156` |
| `{pageSize}` | Items per page | `15` |
| `{startItem}` | First item on current page | `16` |
| `{endItem}` | Last item on current page | `30` |

### Template Examples

**Range display:**
```html
<paging model="Model"
        summary-template="{startItem}-{endItem} / {totalItems}" />
<!-- Output: "16-30 / 156" -->
```

**Percentage style:**
```html
<paging model="Model"
        summary-template="Page {currentPage}/{totalPages} ({pageSize} per page)" />
<!-- Output: "Page 2/10 (15 per page)" -->
```

**Verbose format:**
```html
<paging model="Model"
        summary-template="Displaying items {startItem} through {endItem} from a total of {totalItems} results" />
<!-- Output: "Displaying items 16 through 30 from a total of 156 results" -->
```

**Localized with custom template:**
```html
<paging model="Model"
        language="fr"
        summary-template="Affichage de {startItem} à {endItem} sur {totalItems} éléments" />
<!-- Output: "Affichage de 16 à 30 sur 156 éléments" -->
```

## Adding New Languages

### Method 1: Resource Files (Recommended)

Create a resource file in your application:

**Resources/PagingResources.nl.resx** (Dutch):

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="FirstPageText" xml:space="preserve">
    <value>«</value>
  </data>
  <data name="PreviousPageText" xml:space="preserve">
    <value>‹ Vorige</value>
  </data>
  <data name="NextPageText" xml:space="preserve">
    <value>Volgende ›</value>
  </data>
  <data name="LastPageText" xml:space="preserve">
    <value>»</value>
  </data>
  <data name="PageSummaryFormat" xml:space="preserve">
    <value>Pagina {0} van {1} (Totaal items: {2})</value>
  </data>
  <data name="PageSizeLabel" xml:space="preserve">
    <value>Items per pagina:</value>
  </data>
  <data name="SkipForwardText" xml:space="preserve">
    <value>..</value>
  </data>
  <data name="SkipBackText" xml:space="preserve">
    <value>..</value>
  </data>
  <data name="NextPageAriaLabel" xml:space="preserve">
    <value>ga naar de volgende pagina</value>
  </data>
</root>
```

### Method 2: Inline Override

For quick customization without resource files:

```html
<paging model="Model"
        language="custom"
        first-page-text="Primera"
        previous-page-text="Anterior"
        next-page-text="Siguiente"
        last-page-text="Última"
        summary-template="Página {currentPage} de {totalPages}" />
```

## Localization Architecture

### How It Works

1. **TagHelper** requests localized text from `PagingLocalizer`
2. **PagingLocalizer** checks for:
   - Explicit override (e.g., `first-page-text` attribute)
   - Resource file for specified culture
   - Fallback to default English
3. **ViewComponent** receives pre-localized text
4. **View** renders the localized strings

### Localization Priority

Text is resolved in this order (highest priority first):

1. **Explicit attribute** - `first-page-text="Custom"`
2. **Specified language** - `language="fr"` → PagingResources.fr.resx
3. **Current UI culture** - `CultureInfo.CurrentUICulture`
4. **Default English** - Built-in fallback

### Example

```html
<paging model="Model"
        language="fr"
        first-page-text="Start" />
```

Result:
- First page button: "Start" (explicit override)
- Previous button: "‹ Précédent" (from French resources)
- Next button: "Suivant ›" (from French resources)
- Summary: "Page 1 sur 5 (Articles totaux : 42)" (from French resources)

## All Localizable Strings

### Navigation Buttons

| Property | Attribute | Default (EN) | Purpose |
|----------|-----------|--------------|---------|
| `FirstPageText` | `first-page-text` | `«` | First page button |
| `PreviousPageText` | `previous-page-text` | `‹ Previous` | Previous page button |
| `NextPageText` | `next-page-text` | `Next ›` | Next page button |
| `LastPageText` | `last-page-text` | `»` | Last page button |
| `SkipBackText` | `skip-back-text` | `..` | Skip backward ellipsis |
| `SkipForwardText` | `skip-forward-text` | `..` | Skip forward ellipsis |

### Accessibility

| Property | Attribute | Default (EN) | Purpose |
|----------|-----------|--------------|---------|
| `NextPageAriaLabel` | `next-page-aria-label` | `go to next page` | Screen reader label |
| `PreviousPageAriaLabel` | `previous-page-aria-label` | `go to previous page` | Screen reader label |

### Summary and Labels

| Property | Attribute | Default (EN) | Purpose |
|----------|-----------|--------------|---------|
| `PageSummaryFormat` | `summary-template` | `Page {0} of {1} (Total items: {2})` | Pagination summary |
| `PageSizeLabel` | - | `Items per page:` | Page size dropdown label |

## Complete Translation Examples

### French (fr)

```html
<paging model="Model" language="fr" />
```

**Output:**
- First: `«`
- Previous: `‹ Précédent`
- Next: `Suivant ›`
- Last: `»`
- Summary: `Page 2 sur 10 (Articles totaux : 156)`
- Page size label: `Éléments par page :`

### German (de)

```html
<paging model="Model" language="de" />
```

**Output:**
- First: `«`
- Previous: `‹ Zurück`
- Next: `Weiter ›`
- Last: `»`
- Summary: `Seite 2 von 10 (Gesamtanzahl: 156)`
- Page size label: `Elemente pro Seite:`

### Japanese (ja)

```html
<paging model="Model" language="ja" />
```

**Output:**
- First: `«`
- Previous: `‹ 前へ`
- Next: `次へ ›`
- Last: `»`
- Summary: `ページ 2 / 10 (総アイテム数: 156)`
- Page size label: `ページあたりのアイテム数:`

### Chinese Simplified (zh-Hans)

```html
<paging model="Model" language="zh-Hans" />
```

**Output:**
- First: `«`
- Previous: `‹ 上一页`
- Next: `下一页 ›`
- Last: `»`
- Summary: `第 2 页，共 10 页（总计 156 项）`
- Page size label: `每页项目数：`

## Real-World Examples

### Multi-Language Site

**Startup.cs / Program.cs:**
```csharp
var supportedCultures = new[] { "en", "fr", "de", "es", "it", "pt", "ja", "zh-Hans" };
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures));
```

**View:**
```html
<!-- Automatically uses user's preferred language from Accept-Language header -->
<paging model="Model" />
```

### User-Selectable Language

**Controller:**
```csharp
public IActionResult Index(string lang = "en", int page = 1)
{
    CultureInfo.CurrentUICulture = new CultureInfo(lang);
    // ... load data
}
```

**View:**
```html
<div class="language-selector">
    <a href="?lang=en">English</a>
    <a href="?lang=fr">Français</a>
    <a href="?lang=de">Deutsch</a>
    <a href="?lang=es">Español</a>
</div>

<paging model="Model" />
```

### Custom Branding with Localization

```html
<paging model="Model"
        language="en"
        first-page-text="⏮ Start"
        last-page-text="End ⏭"
        summary-template="📄 Page {currentPage} of {totalPages} | 📊 {totalItems} results" />
```

### Accessibility-Focused

```html
<paging model="Model"
        language="en"
        next-page-aria-label="Navigate to next page of results"
        previous-page-aria-label="Navigate to previous page of results"
        summary-template="Currently viewing page {currentPage} of {totalPages}, showing items {startItem} through {endItem} out of {totalItems} total results" />
```

## Testing Localization

### Test All Cultures

```csharp
[Theory]
[InlineData("en")]
[InlineData("fr")]
[InlineData("de")]
[InlineData("es")]
[InlineData("it")]
[InlineData("pt")]
[InlineData("ja")]
[InlineData("zh-Hans")]
public void Pager_Should_Render_In_All_Cultures(string culture)
{
    CultureInfo.CurrentUICulture = new CultureInfo(culture);
    // Test pager rendering
}
```

### Test Custom Templates

```csharp
[Fact]
public void Custom_Summary_Template_Should_Replace_Placeholders()
{
    var template = "Items {startItem}-{endItem} of {totalItems}";
    var model = new PagerViewModel
    {
        Page = 2,
        PageSize = 10,
        TotalItems = 100,
        SummaryTemplate = template
    };

    var summary = model.GetPageSummary();

    Assert.Equal("Items 11-20 of 100", summary);
}
```

## Troubleshooting

### Issue: Text Not Localizing

**Problem:** Pager shows English text even though culture is set.

**Solutions:**
1. Check culture is set BEFORE rendering:
```csharp
CultureInfo.CurrentUICulture = new CultureInfo("fr");
```

2. Use explicit `language` attribute:
```html
<paging model="Model" language="fr" />
```

3. Verify supported culture in Startup:
```csharp
.AddSupportedUICultures("en", "fr", "de", "es", "it", "pt", "ja", "zh-Hans")
```

### Issue: Custom Template Not Working

**Problem:** Placeholders not being replaced.

**Solution:** Ensure exact placeholder names with braces:
```html
<!-- ✅ Correct -->
<paging summary-template="{currentPage} of {totalPages}" />

<!-- ❌ Wrong -->
<paging summary-template="currentPage of totalPages" />
```

### Issue: Mixed Languages

**Problem:** Some text in one language, some in another.

**Explanation:** This is by design! Explicit attributes override localization:
```html
<paging model="Model"
        language="fr"
        first-page-text="Start" />
<!-- "Start" appears in English, rest in French -->
```

## See Also

- [Getting Started](getting-started.md)
- [Pager TagHelper](pager-taghelper.md)
- [Custom Views](custom-views.md)
- [Advanced Usage](advanced-usage.md)
