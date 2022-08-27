namespace TweetPicker.Service.Interfaces;

public interface ISyncRetweetsService
{
  Task SyncRetweetsAsync(CancellationToken cancellationToken);
}
