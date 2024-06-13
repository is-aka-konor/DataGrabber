namespace DataGrabber.Parser.Core.Parsers;

public interface IParserNotification<in T> where T : new()
{
    void Parser_OnNewData(object arg1, T arg2);

    void Parser_OnCompleted(object arg1);
}