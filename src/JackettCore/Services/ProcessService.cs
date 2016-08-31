﻿using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace JackettCore.Services
{
    public interface IProcessService
    {
        void StartProcessAndLog(string exe, string args, bool asAdmin = false);
        string StartProcessAndGetOutput(string exe, string args, bool keepnewlines = false, bool asAdmin = false);
    }

    public class ProcessService : IProcessService
    {
        private readonly ILogger _logger;

        public ProcessService(ILogger logger)
        {
            _logger = logger;
        }

        private void Run(string exe, string args, bool asAdmin, DataReceivedEventHandler d, DataReceivedEventHandler r)
        {
            var startInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = exe,
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            if (asAdmin)
            {
                startInfo.Verb = "runas";
                startInfo.UseShellExecute = true;
                startInfo.RedirectStandardError = false;
                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardInput = false;
            }
            _logger.LogDebug("Running " + startInfo.FileName + " " + startInfo.Arguments);
            var proc = Process.Start(startInfo);

            if (!asAdmin)
            {
                proc.OutputDataReceived += d;
                proc.ErrorDataReceived += r;
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
            }
            proc.WaitForExit();
            if (!asAdmin)
            {
                proc.OutputDataReceived -= d;
                proc.ErrorDataReceived -= r;
            }
        }

        public string StartProcessAndGetOutput(string exe, string args, bool keepnewlines = false, bool asAdmin = false)
        {
            var sb = new StringBuilder();
            DataReceivedEventHandler rxData = (a, e) => {
                if (keepnewlines || !string.IsNullOrWhiteSpace(e.Data))
                {
                    sb.AppendLine(e.Data);
                }
            };
            DataReceivedEventHandler rxError = (s, e) => {
                if (keepnewlines || !string.IsNullOrWhiteSpace(e.Data))
                {
                    sb.AppendLine(e.Data);
                }
            };

            Run(exe, args, asAdmin, rxData, rxError);
            return sb.ToString();
        }

        public void StartProcessAndLog(string exe, string args, bool asAdmin = false)
        {
            var sb = new StringBuilder();
            DataReceivedEventHandler rxData = (a, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    _logger.LogDebug(e.Data);
                }
            };
            DataReceivedEventHandler rxError = (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    _logger.LogError(e.Data);
                }
            };

            Run(exe, args, asAdmin, rxData, rxError);
        }
    }
}
