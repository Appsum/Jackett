using System;
using Jackett.Models.IndexerConfig;

namespace Jackett
{

    public class ExceptionWithConfigData : Exception
    {
        public ConfigurationData ConfigData { get; private set; }
        public ExceptionWithConfigData(string message, ConfigurationData data)
            : base(message)
        {
            ConfigData = data;
        }

    }
}
