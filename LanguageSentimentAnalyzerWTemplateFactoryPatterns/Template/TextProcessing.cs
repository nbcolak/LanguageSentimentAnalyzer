namespace TranslateSentimentProcessorWTemplateFactoryPatterns.Template;

public abstract class TextProcessing
{
    public void ProcessText(string text)
    {
        string language = DetectLanguage(text); 
        string sentiment = AnalyzeSentiment(text);
        GenerateResponse(language, sentiment);
    }
    protected abstract string DetectLanguage(string text);
    protected abstract string AnalyzeSentiment(string text);
    protected abstract void GenerateResponse(string translatedText, string sentiment);
}