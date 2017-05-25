
namespace Schedulr.Infrastructure
{
    public class FlickrUserStatusInfo
    {
        public long BandwidthUsed { get; private set; }
        public long BandwidthMax { get; private set; }
        public double BandwidthUsedPercentage { get; private set; }
        public int? VideosUploaded { get; private set; }
        public int? VideosRemaining { get; private set; }

        public FlickrUserStatusInfo(long bandwidthUsed, long bandwidthMax, double bandwidthUsedPercentage, int? videosUploaded, int? videosRemaining)
        {
            this.BandwidthUsed = bandwidthUsed;
            this.BandwidthMax = bandwidthMax;
            this.BandwidthUsedPercentage = bandwidthUsedPercentage;
            this.VideosUploaded = videosUploaded;
            this.VideosRemaining = videosRemaining;
        }
    }
}