﻿using System;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;

namespace JackettCore.Services
{
    public interface ISecuityService
    {
        bool CheckAuthorised(HttpRequestMessage request);
        string HashPassword(string input);
        void Login(HttpResponseMessage request);
        void Logout(HttpResponseMessage request);
    }

    class SecuityService : ISecuityService
    {
        private const string COOKIENAME = "JACKETT";
        private IServerService serverService;

        public SecuityService(IServerService ss)
        {
            serverService = ss;
        }

        public string HashPassword(string input)
        {
            // Append key as salt
            input += serverService.Config.APIKey;

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = UE.GetBytes(input);
            SHA512 hashString = SHA512.Create();
            string hex = "";

            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += string.Format("{0:x2}", x);
            }
            return hex;
        }

        public void Login(HttpResponseMessage response)
        {
            // Login
            response.Headers.Add("Set-Cookie", COOKIENAME + "=" + serverService.Config.AdminPassword + "; path=/");
        }

        public void Logout(HttpResponseMessage response)
        {
            // Logout
            response.Headers.Add("Set-Cookie", COOKIENAME + "=; path=/");
        }

        public bool CheckAuthorised(HttpRequestMessage request)
        {
            if (string.IsNullOrEmpty(Engine.Server.Config.AdminPassword))
                return true;

            try
            {
                var cookie = request.Headers.GetCookies(COOKIENAME).FirstOrDefault();
                if (cookie != null)
                {
                    return cookie[COOKIENAME].Value == serverService.Config.AdminPassword;
                }
            }
            catch { }

            return false;
        }
    }
}
