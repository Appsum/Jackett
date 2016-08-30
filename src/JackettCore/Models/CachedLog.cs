using System;

namespace Jackett.Models
{
    public class CachedLog
    {
        public string Level { set; get; }
        public string Message { set; get; }
        public DateTime When { set; get; }
    }
}
