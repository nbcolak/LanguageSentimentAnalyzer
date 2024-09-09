using TranslateSentimentProcessorWTemplateFactoryPatterns.Template;

namespace TranslateSentimentProcessorWTemplateFactoryPatterns.Factory;

public class TextProcessingFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TextProcessingFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TextProcessing CreateProcessor(ServiceType translationService, ServiceType sentimentService)
    {
        var serviceKey = (translationService, sentimentService);

        var serviceMap = new Dictionary<(ServiceType, ServiceType), Func<TextProcessing>>()
        {
            { (ServiceType.Google, ServiceType.Azure), () => _serviceProvider.GetRequiredService<GoogleLanguageAzureSentiment>() },
            { (ServiceType.Azure, ServiceType.Google), () => _serviceProvider.GetRequiredService<AzureLanguageGoogleSentiment>() }
        };

        if (serviceMap.TryGetValue(serviceKey, out var service))
        {
            return service();
        }

        throw new ArgumentException($"Invalid service combination: Translation service {translationService} and sentiment service {sentimentService} are not supported together.");
    }
}