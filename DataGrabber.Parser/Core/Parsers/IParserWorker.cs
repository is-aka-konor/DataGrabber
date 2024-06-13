namespace DataGrabber.Parser.Core.Parsers;

public interface IParserWorker
{
    Task Start();

    void Abort();

    Task Work();
}