namespace JackettCore.Utils
{
    public static class BrowserUtil
    {
        public static string ChromeUserAgent
        {
            get {
                if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    return "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Ubuntu Chrome/47.0.2526.73 Safari/537.36";
                }
                else
                {
                    return "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36";
                }
            }
        }
    }
}
