using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;
using DataGrabber.Parser.Models;

namespace DataGrabber.Parser.Core.ParsingStrategies;

public class SpellA5EStrategy : IParsingStrategy<SpellModel>
{
    public Task<SpellModel> ParseAsync(IHtmlDocument htmlDocument)
    {
        return Task.FromResult(Parse(htmlDocument));
    }

    public SpellModel Parse(IHtmlDocument htmlDocument)
    {

        var spell = new SpellModel()
        {
            Name = htmlDocument.QuerySelector("h1.page-header")?.TextContent.Trim() ?? "",
            Level = GetLevel(htmlDocument),
            School = GetSchool(htmlDocument),
            Tags = GetTags(htmlDocument),
            Classes = GetClasses(htmlDocument),
            CastingTime = htmlDocument.QuerySelector(".field--name-field-spell-casting-time .field--item")?.TextContent?.Trim() ?? "",
            Duration = htmlDocument.QuerySelector("#duration .duration-value a")?.TextContent.Trim() ?? "",
            Range =  GetRange(htmlDocument),
            Components = GetComponents(htmlDocument),
            Target = htmlDocument.QuerySelector(".field--name-field-spell-target .field--item")?.TextContent.Trim() ?? "",
            Ritual = IsRitual(htmlDocument),
            Source = GetSource(htmlDocument),
            SavingThrow = htmlDocument.QuerySelector(".field.field--name-field-spell-saving-throw-desc .field--item")?.TextContent.Trim() ?? "",
            Texts = GetTexts(htmlDocument)
        };
        return spell;
    }

    private int GetLevel(IHtmlDocument htmlDocument)
    {
        
        var levelElement = htmlDocument.QuerySelector(".field--name-field-spell-level a");
        if (levelElement != null)
        {
            var levelText = levelElement.TextContent.Trim();
            if (levelText.Equals("cantrip", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }
            if (int.TryParse(levelText.Substring(0, levelText.Length - 2), out int level))
            {
                return level;
            }
        }
        return -1;
    }
    
    private string GetSchool(IHtmlDocument htmlDocument)
    {
        var schoolElement = htmlDocument.QuerySelector(".field--name-field-classical-spell-school a");
        return schoolElement != null ? schoolElement.TextContent.Trim() : string.Empty;
    }
    
    private List<string> GetTags(IHtmlDocument htmlDocument)
    {
        var tags = new List<string>();
        var tagElements = htmlDocument.QuerySelectorAll(".field--name-field-spell-schools .field--item a");
        foreach (var tagElement in tagElements)
        {
            tags.Add(tagElement.TextContent.Trim());
        }
        return tags;
    }

    private List<string> GetClasses(IHtmlDocument htmlDocument)
    {
        var classes = new List<string>();
        var classElements = htmlDocument.QuerySelectorAll(".field--name-field-spell-classes .field--item a");
        foreach (var classElement in classElements)
        {
            classes.Add(classElement.TextContent.Trim());
        }
        return classes;
    }
    
    private List<string> GetComponents(IHtmlDocument htmlDocument)
    {
        var components = new List<string>();
        var componentElements = htmlDocument.QuerySelectorAll("#spell-components-display .component-value a");
        var materialComponentElement = htmlDocument.QuerySelector("div.field.field--name-field-spellcomponent-description.field--type-string.field--label-hidden.field--item");


        foreach (var componentElement in componentElements)
        {
            components.Add(componentElement.TextContent.Trim());
        }
        if(materialComponentElement != null)
        {
            components.Add(materialComponentElement.TextContent.Trim());
        }
        return components;
    }
    
    private string GetRange(IHtmlDocument htmlDocument)
    {
        var rangeElement = htmlDocument.QuerySelector(".field--name-field-spell-range .field--item a");
        return rangeElement != null ? rangeElement.TextContent.Trim() : string.Empty;
    }
    
    private bool IsRitual(IHtmlDocument htmlDocument)
    {
        var ritualElement = htmlDocument.QuerySelector(".ritual-note .ritual-indicator");
        return ritualElement != null;
    }
    
    private List<string> GetTexts(IHtmlDocument htmlDocument)
    {
        var selectors = new string[]
        {
            "#spell-body .field.field--name-body.field--type-text-with-summary.field--label-hidden.field--item p",
            ".field.field--name-field-spellcast-at-higher-levels .field--label",
            ".field--name-field-spellcast-at-higher-levels .field--item p",
            ".field--name-field-spell-rare-versions .field--label",
            ".field--name-field-spell-rare-versions .field--item p"
        };

        var texts = new List<string>();
        foreach (var selector in selectors)
        {
            var elements = htmlDocument.QuerySelectorAll(selector);
            foreach (var element in elements)
            {
                var tmp = Regex.Replace(element.TextContent.Trim().Replace("\n", "").Replace("\r", ""), @"\s+", " ");
                texts.Add(Regex.Replace(tmp, @"\t+", " "));
            }
        }
        return texts;
    }
    
    private string GetSource(IHtmlDocument htmlDocument)
    {
        var sourceElement = htmlDocument.QuerySelector(".field--name-field-spell-source .field--item a");
        if (sourceElement != null)
        {
            var sourceText = sourceElement.TextContent.Trim();
            sourceText = RemoveHiddenCharacters(sourceText);
            return sourceText;
        }
        return string.Empty;
    }

    private string RemoveHiddenCharacters(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var asciiOnly = new string(normalized.Where(c => c < 128).ToArray());
        return asciiOnly;
    }
}