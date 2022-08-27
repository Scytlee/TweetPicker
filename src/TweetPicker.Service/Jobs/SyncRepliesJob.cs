using Microsoft.Extensions.Logging;
using Quartz;
using TweetPicker.Service.Interfaces;

namespace TweetPicker.Service.Jobs;

public class SyncRepliesJob : IJob
{
  private readonly ILogger<SyncRepliesJob> _logger;
  private readonly ISyncRepliesService _syncRepliesService;

  public SyncRepliesJob(ILogger<SyncRepliesJob> logger, ISyncRepliesService syncRepliesService)
  {
    _logger = logger;
    _syncRepliesService = syncRepliesService;
  }

  public async Task Execute(IJobExecutionContext context)
  {
    _logger.LogDebug($"Job started: {nameof(SyncRepliesJob)}");
    await _syncRepliesService.SyncRepliesAsync(context.CancellationToken);
    _logger.LogDebug($"Job finished: {nameof(SyncRepliesJob)}");
  }
}
