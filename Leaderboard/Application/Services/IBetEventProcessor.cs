namespace Application.Services;

public interface IBetEventProcessor
{
    Task ProcessBetEventAsync(string eventJson);
}
