using System;

namespace JackettCore.Models
{
    public class CachedLog
    {
        public string Level { set; get; }
        public string Message { set; get; }
        public DateTime When { set; get; }
    }
}
