using Microsoft.Extensions.Logging;
using Quartz;
using TweetPicker.Service.Interfaces;

namespace TweetPicker.Service.Jobs;

public class SyncRetweetsJob : IJob
{
  private readonly ILogger<SyncRetweetsJob> _logger;
  private readonly ISyncRetweetsService _syncRetweetsService;

  public SyncRetweetsJob(ILogger<SyncRetweetsJob> logger, ISyncRetweetsService syncRetweetsService)
  {
    _logger = logger;
    _syncRetweetsService = syncRetweetsService;
  }

  public async Task Execute(IJobExecutionContext context)
  {
    _logger.LogDebug($"Job started: {nameof(SyncRetweetsJob)}");
    await _syncRetweetsService.SyncRetweetsAsync(context.CancellationToken);
    _logger.LogDebug($"Job finished: {nameof(SyncRetweetsJob)}");
  }
}
