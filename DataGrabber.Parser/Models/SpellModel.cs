namespace DataGrabber.Parser.Models;

public record SpellModel
{
    public string Name { get; init; }

    public int Level { get; init; }

    public string School { get; init; }
    
    public List<string> Tags { get; init; }

    public string CastingTime { get; init; }

    public string Range { get; init; }
    
    public string Target { get; init; }
    public string SavingThrow { get; init; }

    public List<string> Components { get; init; }

    public string Duration { get; init; }

    public string Roll { get; init; }
    
    public bool Ritual { get; set; }

    public List<string> Classes { get; init; }

    public List<string> Texts { get; init; }
    public string Source { get; init; }
}