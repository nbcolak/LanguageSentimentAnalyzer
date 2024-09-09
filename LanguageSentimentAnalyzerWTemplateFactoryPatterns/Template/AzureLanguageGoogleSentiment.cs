using System.Text;
using Azure;
using Azure.AI.TextAnalytics;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Language.V1;
using Microsoft.Extensions.Options;

namespace TranslateSentimentProcessorWTemplateFactoryPatterns.Template;

public class AzureLanguageGoogleSentiment : TextProcessing
{
    private readonly LanguageServiceClient _googleClientlient;
    private readonly TextAnalyticsClient _azureClient;


    public AzureLanguageGoogleSentiment(IOptions<GoogleApiSettings> googlesettings,IOptions<AzureTextAnalyticsSettings> azuresettings)
    {
        var endpoint = azuresettings.Value.Endpoint;
        var apiKey = azuresettings.Value.ApiKey;
        _azureClient = AuthenticateClient(endpoint, apiKey);
        
        string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), googlesettings.Value.CredentialsFilePath);

        var credential = GoogleCredential.FromFile(jsonPath);

        _googleClientlient = new LanguageServiceClientBuilder
        {
            Credential = credential
        }.Build();
    }



    protected override string DetectLanguage(string text)
    {
        var result=_azureClient.DetectLanguage(text);
        return result.Value.Name;
    }

    protected override string AnalyzeSentiment(string text)
    {
        var document = Document.FromPlainText(text);
        var response =  _googleClientlient.AnalyzeSentimentAsync(document);

        var result = new StringBuilder();
    
        var sentimentScore = response.Result.DocumentSentiment.Score;
        result.AppendLine("Google Analyze");

        if (sentimentScore > 0)
        {
            result.AppendLine("Sentiment: Positive");
        }
        else if (sentimentScore < 0)
        {
            result.AppendLine("Sentiment: Negative");
        }
        else
        {
            result.AppendLine("Sentiment: Neutral");
        }

        //result.AppendLine($"Sentiment Score: {response.DocumentSentiment.Score}");
        //result.AppendLine($"Sentiment Magnitude: {response.DocumentSentiment.Magnitude}");

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