﻿namespace JackettCore.Models.IndexerConfig
{
    public class ConfigurationDataBasicLoginWithAlternateLink : ConfigurationData
    {
        public StringItem Username { get; private set; }
        public StringItem Password { get; private set; }
        public DisplayItem Instructions { get; private set; }
        public StringItem AlternateLink { get; set; }

        public ConfigurationDataBasicLoginWithAlternateLink(string instructionMessageOptional = null)
        {
            Username = new StringItem { Name = "Username" };
            Password = new StringItem { Name = "Password" };
            Instructions = new DisplayItem(instructionMessageOptional) { Name = "" };
            AlternateLink = new StringItem { Name = "Alternate Link" };
        }


    }
}