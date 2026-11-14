using System;
using System.Collections.Generic;

/*
using System;
using System.Collections.Generic;

public class StatutePayload
{
    public string StatuteId { get; set; } = string.Empty;
    public string ClauseBody { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = false;
}

public class LocalRabbitMqBroker
{
    public void PublishEvent(string queueName, string messagePayload)
    {
        Console.WriteLine($"[RABBITMQ] Event pushed to message broker queue: {queueName}");
    }
}

public class StatutePublishingEngine
{
    private readonly LocalRabbitMqBroker _messageBroker;

    public StatutePublishingEngine()
    {
        _messageBroker = new LocalRabbitMqBroker();
    }

    public void DispatchStatute(StatutePayload payload, string statuteCategory)
    {
        string formattedPayload = string.Empty;

        if (statuteCategory == "NationalAct")
        {
            formattedPayload = $"[NATIONAL LEGISLATION RECORD]\n{payload.ClauseBody}";
            payload.IsPublished = true;
        }
        else if (statuteCategory == "ProvincialByLaw")
        {
            formattedPayload = $"[PROVINCIAL BY-LAW GAZETTE]\n{payload.ClauseBody}";
            payload.IsPublished = true;
        }
        else if (statuteCategory == "DraftReview")
        {
            formattedPayload = $"[INTERNAL DRAFT - DO NOT DISTRIBUTE]\n{payload.ClauseBody}";
            return;
        }

        string targetQueue = $"statutes.{payload.StatuteId}.published";
        _messageBroker.PublishEvent(targetQueue, formattedPayload);

        if (!payload.IsPublished)
        {
            throw new InvalidOperationException("Post-condition failed: Statute state was not marked published!");
        }
    }
}
*/

public class StatutePayload
{
    public string StatuteId { get; set; } = string.Empty;
    public string ClauseBody { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = false;
}

public class LocalRabbitMqBroker : IMessageBroker
{
    public void PublishEvent(string queueName, string messagePayload)
    {
        Console.WriteLine($"[RABBITMQ] Event pushed to message broker queue: {queueName}");
    }
}
public interface IStatuteHandler
{
    string FormatStatuteForPublication(StatutePayload payload);
}
public interface IStatuteReview
{
    string FormatStatuteForReview(StatutePayload payload);
}
public interface IMessageBroker
{
    void PublishEvent(string queueName, string messagePayload);
}

public class NationalAct : IStatuteHandler
{
    public string FormatStatuteForPublication(StatutePayload payload)
    {
        return $"[NATIONAL LEGISLATION RECORD]\n{payload.ClauseBody}";
    }
}

public class ProvincialByLaw : IStatuteHandler
{
    public string FormatStatuteForPublication(StatutePayload payload)
    {
        return $"[PROVINCIAL BY-LAW GAZETTE]\n{payload.ClauseBody}";
    }
}

public class DraftReview : IStatuteReview
{
    public string FormatStatuteForReview(StatutePayload payload)
    {
        return $"[INTERNAL DRAFT - DO NOT DISTRIBUTE]\n{payload.ClauseBody}";
    }
}

public class StatutePublishingEngine
{
    private readonly IMessageBroker _messageBroker;

    public StatutePublishingEngine(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public string ReviewStatute(StatutePayload payload, IStatuteReview statuteReview)
    {
        return statuteReview.FormatStatuteForReview(payload);
    }

    public void DispatchStatute(StatutePayload payload, IStatuteHandler statuteHandler)
    {
        string formattedPayload = statuteHandler.FormatStatuteForPublication(payload);
        payload.IsPublished = true;

        string targetQueue = $"statutes.{payload.StatuteId}.published";
        _messageBroker.PublishEvent(targetQueue, formattedPayload);
    }
}
