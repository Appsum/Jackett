﻿using System.Threading;
using System.Threading.Tasks;

namespace JackettCore
{
    class WebAPIExceptionLogger : IExceptionLogger
    {
        public async Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            // OWIN seems to give lots of these exceptions but we are not interested in them.
            if (context.Exception.Message != "Error while copying content to a stream.")
            {
                Engine.Logger.Error("Unhandled exception: " + context.Exception.GetExceptionDetails());
                var request = await context.Request.Content.ReadAsStringAsync();
                Engine.Logger.Error("Unhandled exception url: " + context.Request.RequestUri);
            }
        }
    }
}
