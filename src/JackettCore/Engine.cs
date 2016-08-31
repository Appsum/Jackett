using System;
using System.Text;
using Autofac;
using JackettCore.Services;
using Microsoft.Extensions.Logging;

namespace JackettCore
{
    public class Engine
    {
        private static IContainer _container;

        static Engine()
        {
            BuildContainer();

        }

        public static void BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<JackettModule>();
            _container = builder.Build();

            // Register the container in itself to allow for late resolves
            var secondaryBuilder = new ContainerBuilder();
            secondaryBuilder.RegisterInstance(_container).SingleInstance();
            secondaryBuilder.Update(_container);

        }

        public static IContainer GetContainer()
        {
            return _container;
        }

        public static IConfigurationService ConfigService => _container.Resolve<IConfigurationService>();

        public static IProcessService ProcessService => _container.Resolve<IProcessService>();

        public static IServiceConfigService ServiceConfig => _container.Resolve<IServiceConfigService>();

        public static ITrayLockService LockService => _container.Resolve<ITrayLockService>();

        public static IServerService Server => _container.Resolve<IServerService>();

        public static IRunTimeService RunTime => _container.Resolve<IRunTimeService>();

        public static Logger<Engine> Logger => _container.Resolve<Logger<Engine>>();

        public static ISecuityService SecurityService => _container.Resolve<ISecuityService>();


        [LayoutRenderer("simpledatetime")]
    public class SimpleDateTimeRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(DateTime.Now.ToString("MM-dd HH:mm:ss"));
        }
    }
}
