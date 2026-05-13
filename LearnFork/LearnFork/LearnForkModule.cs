using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnForkModule;

public class Message
{
    public Guid MessageId { get; }
    public string Text { get; private set; }
    public string Sender { get; }
    public DateTime CreatedAt { get; }

    public Message(string text, string sender)
    {
        // Перевірка, що повідомлення не є порожнім
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Повідомлення не може бути порожнім.");

        MessageId = Guid.NewGuid();
        Text = text;
        Sender = sender;
        CreatedAt = DateTime.Now;
    }

    // Повертає короткий попередній перегляд повідомлення
    public string GetShortPreview()
    {
        return Text.Length > 50 ? Text.Substring(0, 50) + "..." : Text;
    }
}

public class ApiKey
{
    public Guid KeyId { get; }
    public string Value { get; }
    public string ProviderName { get; }
    public DateTime CreatedAt { get; }

    public ApiKey(string value, string providerName)
    {
        KeyId = Guid.NewGuid();
        Value = value;
        ProviderName = providerName;
        CreatedAt = DateTime.Now;
    }

    // Перевіряє коректність API-ключа
    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
            throw new ArgumentException("API-ключ відсутній.");

        if (Value.Length < 20)
            throw new ArgumentException("API-ключ занадто короткий.");

        return true;
    }
}

public class AIProvider
{
    public Guid ProviderId { get; }
    public string Name { get; }

    public AIProvider(string name)
    {
        ProviderId = Guid.NewGuid();
        Name = name;
    }

    // Імітує надсилання запиту до AI-провайдера
    public Message SendRequest(string prompt, ApiKey apiKey)
    {
        try
        {
            apiKey.Validate();

            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Питання не може бути порожнім.");

            return new Message($"AI-відповідь від {Name}: пояснення для запиту \"{prompt}\"", "AI");
        }
        catch (Exception ex)
        {
            return new Message($"Помилка: {ex.Message}", "system");
        }
    }
}

public class Branch
{
    public Guid BranchId { get; }
    public string Title { get; private set; }
    public Message ParentMessage { get; }
    public List<Message> Messages { get; }
    public DateTime CreatedAt { get; }

    public Branch(string title, Message parentMessage)
    {
        if (parentMessage == null)
            throw new ArgumentException("Гілка повинна бути створена від існуючого повідомлення.");

        BranchId = Guid.NewGuid();
        Title = string.IsNullOrWhiteSpace(title) ? "Нова гілка" : title;
        ParentMessage = parentMessage;
        Messages = new List<Message>();
        CreatedAt = DateTime.Now;
    }

    public Message AddMessage(string text, string sender = "user")
    {
        var message = new Message(text, sender);
        Messages.Add(message);
        return message;
    }

    public void RenameBranch(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Назва гілки не може бути порожньою.");

        Title = newTitle;
    }
}

public class Chat
{
    public Guid ChatId { get; }
    public string Topic { get; }
    public List<Message> Messages { get; }
    public List<Branch> Branches { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public Chat(string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Тема чату не може бути порожньою.");

        ChatId = Guid.NewGuid();
        Topic = topic;
        Messages = new List<Message>();
        Branches = new List<Branch>();
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public Message AddMessage(string text, string sender = "user")
    {
        var message = new Message(text, sender);
        Messages.Add(message);
        UpdatedAt = DateTime.Now;
        return message;
    }

    public Branch CreateBranch(Guid messageId, string title)
    {
        var parentMessage = Messages.FirstOrDefault(m => m.MessageId == messageId);

        if (parentMessage == null)
            throw new ArgumentException("Неможливо створити гілку: повідомлення не знайдено.");

        var branch = new Branch(title, parentMessage);
        Branches.Add(branch);
        UpdatedAt = DateTime.Now;
        return branch;
    }

    public List<Message> SearchMessages(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Пошукове слово не може бути порожнім.");

        var result = new List<Message>();

        foreach (var message in Messages)
        {
            if (message.Text.ToLower().Contains(keyword.ToLower()))
                result.Add(message);
        }

        return result;
    }

    public (Message userMessage, Message aiMessage) AskAI(string prompt, AIProvider provider, ApiKey apiKey)
    {
        var userMessage = AddMessage(prompt, "user");
        var aiMessage = provider.SendRequest(prompt, apiKey);
        Messages.Add(aiMessage);
        UpdatedAt = DateTime.Now;
        return (userMessage, aiMessage);
    }
}