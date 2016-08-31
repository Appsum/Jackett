namespace JackettCore.Models.IndexerConfig
{
    public class ConfigurationDataRecaptchaLogin : ConfigurationData
    {
        public StringItem Username { get; private set; }
        public StringItem Password { get; private set; }
        public RecaptchaItem Captcha { get; private set; }

        public ConfigurationDataRecaptchaLogin()
        {
            Username = new StringItem { Name = "Username" };
            Password = new StringItem { Name = "Password" };
            Captcha = new RecaptchaItem() { Name = "Recaptcha" };
        }


    }
}
