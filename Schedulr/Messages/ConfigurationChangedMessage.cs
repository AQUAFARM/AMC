using Schedulr.Models;

namespace Schedulr.Messages
{
    public class ConfigurationChangedMessage
    {
        public SchedulrConfiguration Configuration { get; private set; }

        public ConfigurationChangedMessage(SchedulrConfiguration configuration)
        {
            this.Configuration = configuration;
        }
    }
}