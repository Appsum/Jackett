using System;

namespace JackettCore.Models
{
    public class CachedResult
    {
        public ReleaseInfo Result
        {
            set; get;
        }

        public DateTime Created
        {
            set; get;
        }
    }
}
