using System.Text;
using AngleSharp;
using AngleSharp.Html.Parser;
using DataGrabber.Parser.Core.ParsingStrategies;
using Xunit.Abstractions;

namespace DataGrabber.Parser.UnitTests;

public class SpellA5ETests
{
    private readonly IHtmlParser _parser;
    private readonly ITestOutputHelper _output;
    private const string SpellFileName1 = "HtmlPageSources/Spells/AlteredStrikeA5E.html";
    private const string SpellFileName2 = "HtmlPageSources/Spells/AlarmA5E.html";
    private const string SpellFileName3 = "HtmlPageSources/Spells/AntipathySympathyA5E.html";
    public SpellA5ETests(ITestOutputHelper output)
    {
        this._output = output;
        // Read the HTML file content
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        _parser = context.GetService<IHtmlParser>() ?? new HtmlParser();
    }

    public static IEnumerable<object[]> SpellNameData =>
        new List<object[]>
        {
            new object[] { SpellFileName1, "Altered Strike" },
            new object[] { SpellFileName2, "Alarm" },
            new object[] { SpellFileName3, "ANTIPATHY/SYMPATHY" }
        };

    [Theory]
    [MemberData(nameof(SpellNameData))]
    public async Task SpellA5EStrategy_ShouldExtractSpellName(string fileName, string expectedName)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedName, spell.Name, ignoreCase: true);
    }
    
    public static IEnumerable<object[]> SpellLevelData =>
        new List<object[]>
        {
            new object[] { SpellFileName1, 0 },
            new object[] { SpellFileName2, 1  },
            new object[] { SpellFileName3, 8 }
        };

    [Theory]
    [MemberData(nameof(SpellLevelData))]
    public async Task SpellA5EStrategy_ShouldExtractSpellLevel(string fileName, int expectedLevel)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedLevel, spell.Level);
    }

    public static IEnumerable<object[]> SpellSchoolData =>
        new List<object[]>
        {
            new object[] { SpellFileName1, "Transmutation" },
            new object[] { SpellFileName2, "Abjuration" },
            new object[] { SpellFileName3, "Enchantment" }
        };

    [Theory]
    [MemberData(nameof(SpellSchoolData))]
    public async Task SpellA5EStrategy_ShouldExtractSpellSchool(string fileName, string expectedSchool)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedSchool, spell.School, ignoreCase: true);
    }

    public static IEnumerable<object[]> SpellSpellTags =>
        new List<object[]>
        {
            new object[] { SpellFileName1, new[] { "enhancement", "transformation", "unarmed", "weaponry" } },
            new object[] { SpellFileName2, new[] { "arcane", "protection", "scrying", "utility" } },
            new object[] { SpellFileName3, new[] { "compulsion" } }
        };

    [Theory]
    [MemberData(nameof(SpellSpellTags))]
    public async Task SpellA5EStrategy_ShouldExtractSpellTags(string fileName, string[] expectedTags)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedTags.Length, spell.Tags.Count);
        for(var i = 0; i < expectedTags.Length; i++)
        {
            Assert.Equal(expectedTags[i], spell.Tags[i], ignoreCase: true);
        }
    }

    public static IEnumerable<object[]> SpellCastingTime =>
        new List<object[]>
        {
            new object[] { SpellFileName1, "1 Action" },
            new object[] { SpellFileName2, "1 Minute" },
            new object[] { SpellFileName3, "1 Hour" }
        };

    [Theory]
    [MemberData(nameof(SpellCastingTime))]
    public async Task SpellA5EStrategy_ShouldExtractSpellTime(string fileName, string expectedTime)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedTime, spell.CastingTime, ignoreCase: true);
    }

    public static IEnumerable<object[]> SpellRange =>
        new List<object[]>
        {
            new object[] { SpellFileName1, "Self" },
            new object[] { SpellFileName2, "Medium (60 feet)" },
            new object[] { SpellFileName3, "Medium (60 feet)" }
        };

    [Theory]
    [MemberData(nameof(SpellRange))]
    public async Task SpellA5EStrategy_ShouldExtractSpellRange(string fileName, string expectedRange)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedRange, spell.Range, ignoreCase: true);
    }

    public static IEnumerable<object[]> SpellComponents =>
        new List<object[]>
        {
            new object[] { SpellFileName1, new[] { "vocalized", "seen", "material", "piece of the desired material" } },
            new object[] { SpellFileName2, new[] { "vocalized", "seen", "material", "miniature trip wire" } },
            new object[] { SpellFileName3, new[] { "vocalized", "seen", "material", "flask of honey and vinegar" } }
        };

    [Theory]
    [MemberData(nameof(SpellComponents))]
    public async Task SpellA5EStrategy_ShouldExtractSpellComponents(string fileName, string[] expectedComponents)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedComponents.Length, spell.Components.Count);
        for(var i = 0; i < expectedComponents.Length; i++)
        {
            Assert.Equal(expectedComponents[i], spell.Components[i], ignoreCase: true);
        }
    }

    public static IEnumerable<object[]> SpellClasses =>
        new List<object[]>
        {
            new object[] { SpellFileName1, new[] { "Artificer", "bard", "herald", "sorcerer", "wizard" } },
            new object[] { SpellFileName2, new[] { "Artificer", "Wizard" } },
            new object[] { SpellFileName3, new[] { "Druid", "Wizard" } }
        };

    [Theory]
    [MemberData(nameof(SpellClasses))]
    public async Task SpellA5EStrategy_ShouldExtractSpellClasses(string fileName, string[] expectedClasses)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedClasses.Length, spell.Classes.Count);
        for(var i = 0; i < expectedClasses.Length; i++)
        {
            Assert.Equal(expectedClasses[i], spell.Classes[i], ignoreCase: true);
        }
    }

    public static IEnumerable<object[]> SpellDuration =>
        new List<object[]>
        {
            new object[] { SpellFileName1, "1 Round" },
            new object[] { SpellFileName2, "8 hours" },
            new object[] { SpellFileName3, "10 Days" }
        };

    [Theory]
    [MemberData(nameof(SpellDuration))]
    public async Task SpellA5EStrategy_ShouldExtractDuration(string fileName, string expectedDuration)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedDuration, spell.Duration, ignoreCase: true);
    }

    public static IEnumerable<object[]> SpellTarget =>
        new List<object[]>
        {
            new object[] { SpellFileName1, "" },
            new object[] { SpellFileName2, "Object or area up to 20-foot cube" },
            new object[] { SpellFileName3, "Huge or smaller object, creature, or 200-foot cube" }
        };

    [Theory]
    [MemberData(nameof(SpellTarget))]
    public async Task SpellA5EStrategy_ShouldExtractTarget(string fileName, string expectedTarget)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedTarget, spell.Target, ignoreCase: true);
    }

    public static IEnumerable<object[]> SpellRitualFlag =>
        new List<object[]>
        {
            new object[] { SpellFileName1, false },
            new object[] { SpellFileName2, true },
            new object[] { SpellFileName3, false }
        };

    [Theory]
    [MemberData(nameof(SpellRitualFlag))]
    public async Task SpellA5EStrategy_ShouldExtractRitual(string fileName, bool expectedFlag)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedFlag, spell.Ritual);
    }

    public static IEnumerable<object[]> SpellSource =>
        new List<object[]>
        {
            new object[] { SpellFileName1, "Adventurer's Guide" },
            new object[] { SpellFileName2, "Adventurer's Guide" },
            new object[] { SpellFileName3, "Adventurer's Guide" }
        };

    [Theory]
    [MemberData(nameof(SpellSource))]
    public async Task SpellA5EStrategy_ShouldExtractSource(string fileName, string expectedSource)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedSource, spell.Source, ignoreCase: true);
    }

    public static IEnumerable<object[]> SpellSavingThrow =>
        new List<object[]>
        {
            new object[] { SpellFileName1, "" },
            new object[] { SpellFileName2, "" },
            new object[] { SpellFileName3, "Wisdom (special)" }
        };

    [Theory]
    [MemberData(nameof(SpellSavingThrow))]
    public async Task SpellA5EStrategy_ShouldExtractSavingThrow(string fileName, string expectedSavingThrow)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        Assert.Equal(expectedSavingThrow, spell.SavingThrow, ignoreCase: true);
    }

    [Theory]
    [MemberData(nameof(SpellText))]
    public async Task SpellA5EStrategy_ShouldExtractSpellText(string fileName, string expectedText)
    {
        // Arrange
        var htmlContent = await File.ReadAllTextAsync(fileName);
        var document = await _parser.ParseDocumentAsync(htmlContent);
        var strategy = new SpellA5EStrategy();

        // Act
        var spell = await strategy.ParseAsync(document);

        // Assert
        var sb = new StringBuilder();
        foreach(var text in spell.Texts)
        {
            sb.AppendLine(text);
        }

        var result = sb.ToString();
        Assert.Equal(expectedText, result, ignoreCase: true);
    }

    public static IEnumerable<object[]> SpellText =>
        new List<object[]>
        {
            new object[] { SpellFileName1, @"You briefly transform your weapon or fist into another material and strike with it, making a melee weapon attack against a target within your reach. You use your spellcasting ability for your attack and damage rolls, and your melee weapon attack counts as if it were made with a different material for the purpose of overcoming resistance and immunity to nonmagical attacks and damage: either bone, bronze, cold iron, steel, stone, or wood.
When you reach 5th level, you can choose silver or mithral as the material.
When you reach 11th level, if you have the Extra Attack feature you make two melee weapon attacks as part of the casting of this spell instead of 1. In addition, you can choose adamantine as the material.
When you reach 17th level, your attacks with this spell deal an extra 1d6 damage.
" },
            new object[] { SpellFileName2, @"You set an alarm against unwanted intrusion that alerts you whenever a creature of size Tiny or larger touches or enters the warded area. When you cast the spell, choose any number of creatures. These creatures don’t set off the alarm.
Choose whether the alarm is silent or audible. The silent alarm is heard in your mind if you are within 1 mile of the warded area and it awakens you if you are sleeping. An audible alarm produces a loud noise of your choosing for 10 seconds within 60 feet.
Area: Object or area up to 20-foot cube
Cast at Higher Levels
You may create an additional alarm for each slot level above 1st. The spell’s range increases to 600 feet, but you must be familiar with the locations you ward, and all alarms must be set within the same physical structure. Setting off one alarm does not activate the other alarms.
You may choose one of the following effects in place of creating an additional alarm. The effects apply to all alarms created during the spell’s casting.
Increased Duration. The spell’s duration increases to 24 hours.
Improved Audible Alarm. The audible alarm produces any sound you choose and can be heard up to 300 feet away.
Improved Mental Alarm. The mental alarm alerts you regardless of your location, even if you and the alarm are on different planes of existence.
" },
            new object[] { SpellFileName3, @"You mystically impart great love or hatred for a place, thing, or creature. Designate a kind of intelligent creature, such as dragons, goblins, or vampires. The target now causes either antipathy or sympathy for the specified creatures for the duration of the spell. When a designated creature successfully saves against the effects of this spell, it immediately understands it was under a magical effect and is immune to this spell’s effects for 1 minute.
Antipathy: When a designated creature can see the target or comes within 60 feet of it, the creature makes a Wisdom saving throw or becomes frightened . While frightened the creature must use its movement to move away from the target to the nearest safe spot from which it can no longer see the target. If the creature moves more than 60 feet from the target and can no longer see it, the creature is no longer frightened, but the creature becomes frightened again if it regains sight of the target or moves within 60 feet of it.
Sympathy: When a designated creature can see the target or comes within 60 feet of it, the creature must succeed on a Wisdom saving throw . On a failure, the creature uses its movement on each of its turns to enter the area or move within reach of the target, and is unwilling to move away from the target.
If the target damages or otherwise harms an affected creature, the affected creature can make a Wisdom saving throw to end the effect. An affected creature can also make a saving throw once every 24 hours while within the area of the spell, and whenever it ends its turn more than 60 feet from the target and is unable to see the target.
Rare Versions
Roav’s Fiendish Antipathy/Sympathy. A fiend has disadvantage on its first saving throw against an effect caused by this spell.
" }
        };
}