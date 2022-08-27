namespace TweetPicker.Service.Interfaces;

public interface ISyncRepliesService
{
  Task SyncRepliesAsync(CancellationToken cancellationToken);
}
