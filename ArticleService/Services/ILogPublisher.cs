using ArticleService.Models;

namespace ArticleService.Services;

public interface ILogPublisher
{
    void SendMessage(LogMessage message);
}