

namespace TranslateSentimentProcessorWTemplateFactoryPatterns.Factory;



public enum ServiceType
{
    [System.ComponentModel.Description("Google Translation Service")]
    Google = 0,

    [System.ComponentModel.Description("Azure Translation Service")]
    Azure = 1
}