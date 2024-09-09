using System.Text;
using Azure;
using Azure.AI.TextAnalytics;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Language.V1;
using Microsoft.Extensions.Options;

namespace TranslateSentimentProcessorWTemplateFactoryPatterns.Template;
public class GoogleLanguageAzureSentiment : TextProcessing
{   
    private readonly TextAnalyticsClient _azureClient;
    private readonly LanguageServiceClient _googleClient;

    public GoogleLanguageAzureSentiment(IOptions<GoogleApiSettings> googlesettings,IOptions<AzureTextAnalyticsSettings> settings)
    {
        var endpoint = settings.Value.Endpoint;
        var apiKey = settings.Value.ApiKey;
        _azureClient = AuthenticateClient(endpoint, apiKey);
        string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), googlesettings.Value.CredentialsFilePath);

        var credential = GoogleCredential.FromFile(jsonPath);

        _googleClient = new LanguageServiceClientBuilder
        {
            Credential = credential
        }.Build();
    }


    protected override string DetectLanguage(string text)
    {
        var languageResponse = _googleClient.AnalyzeEntities(new AnalyzeEntitiesRequest
        {
            Document = new Document
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            },
            EncodingType = EncodingType.Utf8
        });

        string detectedLanguage = languageResponse.Language;
    
        if (detectedLanguage == "tr")
        {
            Console.WriteLine("Unsupported Turkish");
            return detectedLanguage;
        }

        var syntaxResponse = _googleClient.AnalyzeSyntax(new AnalyzeSyntaxRequest
        {
            Document = new Document
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            },
            EncodingType = EncodingType.Utf8
        });

        return syntaxResponse.Language;
    }

    protected override string AnalyzeSentiment(string text)
    {
        var sentimentResult =  _azureClient.AnalyzeSentimentAsync(text);
        var result = new StringBuilder();

        foreach (var sentence in sentimentResult.Result.Value.Sentences)
        {
            result.AppendLine("Azure Analyze");

            if (sentence.Sentiment == TextSentiment.Positive)
            {
                result.AppendLine("Sentiment: Positive");
            }
            else if (sentence.Sentiment == TextSentiment.Negative)
            {
                result.AppendLine("Sentiment: Negative");
            }
            else
            {
                result.AppendLine("Sentiment: Neutral");
            }
        }

        return result.ToString();        
    }

    protected override void GenerateResponse(string translatedText, string sentiment)
    {
        Console.WriteLine($"Translated Text: {translatedText}, Sentiment: {sentiment}");
    }
    private TextAnalyticsClient AuthenticateClient(string endpoint, string apiKey)
    {
        var credentials = new AzureKeyCredential(apiKey);
        var client = new TextAnalyticsClient(new Uri(endpoint), credentials);
        return client;
    }
}