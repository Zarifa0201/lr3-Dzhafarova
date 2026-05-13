using System;
using Xunit;
using LearnForkModule;

namespace LearnForkModule;

public class ApiKeyTests
{
    [Fact]
    public void Validate_ShouldReturnTrue_ForValidApiKey()
    {
        // Arrange
        // Техніка: EP + позитивний тест
        var apiKey = new ApiKey("sk-example-12345678901234567890", "OpenAI");

        // Act
        var result = apiKey.Validate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Validate_ShouldThrow_WhenApiKeyIsEmpty()
    {
        // Arrange
        // Техніка: BVA + негативний тест
        var apiKey = new ApiKey("", "OpenAI");

        // Act + Assert
        Assert.Throws<ArgumentException>(() => apiKey.Validate());
    }

    [Fact]
    public void Validate_ShouldThrow_WhenApiKeyHas19Characters()
    {
        // Arrange
        // Техніка: BVA + негативний тест
        var apiKey = new ApiKey("1234567890123456789", "OpenAI");

        // Act + Assert
        Assert.Throws<ArgumentException>(() => apiKey.Validate());
    }
}

public class MessageTests
{
    [Fact]
    public void Constructor_ShouldCreateMessage_WithValidText()
    {
        // Arrange
        // Техніка: EP + позитивний тест
        var text = "Поясни, що таке AI";

        // Act
        var message = new Message(text, "user");

        // Assert
        Assert.Equal(text, message.Text);
        Assert.Equal("user", message.Sender);
        Assert.NotEqual(Guid.Empty, message.MessageId);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenTextIsEmpty()
    {
        // Arrange
        // Техніка: BVA + негативний тест
        var text = "";

        // Act + Assert
        Assert.Throws<ArgumentException>(() => new Message(text, "user"));
    }
}

public class ChatTests
{
    [Fact]
    public void Constructor_ShouldCreateChat_WithValidTopic()
    {
        // Arrange
        // Техніка: EP + позитивний тест
        var topic = "Вивчення C#";

        // Act
        var chat = new Chat(topic);

        // Assert
        Assert.Equal(topic, chat.Topic);
        Assert.Empty(chat.Messages);
        Assert.Empty(chat.Branches);
    }

    [Fact]
    public void AddMessage_ShouldAddMessageToChat()
    {
        // Arrange
        // Техніка: EP + позитивний тест
        var chat = new Chat("AI навчання");

        // Act
        var message = chat.AddMessage("Що таке гілка?", "user");

        // Assert
        Assert.Single(chat.Messages);
        Assert.Equal(message, chat.Messages[0]);
    }

    [Fact]
    public void SearchMessages_ShouldFindMessage_ByKeyword()
    {
        // Arrange
        // Техніка: EP + позитивний тест
        var chat = new Chat("AI навчання");
        chat.AddMessage("C# — це мова програмування", "AI");
        chat.AddMessage("Python також популярний", "AI");

        // Act
        var result = chat.SearchMessages("C#");

        // Assert
        Assert.Single(result);
        Assert.Contains("C#", result[0].Text);
    }

    [Fact]
    public void SearchMessages_ShouldThrow_WhenKeywordIsEmpty()
    {
        // Arrange
        // Техніка: BVA + негативний тест
        var chat = new Chat("AI навчання");

        // Act + Assert
        Assert.Throws<ArgumentException>(() => chat.SearchMessages(""));
    }

}

public class AIProviderTests
{
    [Fact]
    public void SendRequest_ShouldReturnAiMessage_WithValidPromptAndKey()
    {
        // Arrange
        // Техніка: EP + позитивний тест
        var provider = new AIProvider("OpenAI");
        var apiKey = new ApiKey("sk-example-12345678901234567890", "OpenAI");

        // Act
        var result = provider.SendRequest("Поясни C#", apiKey);

        // Assert
        Assert.Equal("AI", result.Sender);
        Assert.Contains("AI-відповідь від OpenAI", result.Text);
    }

    [Fact]
    public void SendRequest_ShouldReturnSystemMessage_WhenPromptIsEmpty()
    {
        // Arrange
        // Техніка: BVA + негативний тест
        var provider = new AIProvider("OpenAI");
        var apiKey = new ApiKey("sk-example-12345678901234567890", "OpenAI");

        // Act
        var result = provider.SendRequest("", apiKey);

        // Assert
        Assert.Equal("system", result.Sender);
        Assert.Contains("Помилка", result.Text);
    }
}