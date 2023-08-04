using Meadow;
using Meadow.Logging;

namespace MeadowTest.StrippedDownNetwork
{
    public abstract class MeadowBase
    {
        protected TestAppSettings AppSettings { get; } = (TestAppSettings)Resolver.Services.Get(typeof(TestAppSettings));
        protected Logger Log { get; } = Resolver.Log;
        protected IMeadowDevice Device { get; } = Resolver.Device;

        public MeadowBase() { }
    }
}
