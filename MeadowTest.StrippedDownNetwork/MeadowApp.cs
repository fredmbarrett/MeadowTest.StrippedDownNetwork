using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace MeadowTest.StrippedDownNetwork
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            // get apps settings and put into app resolver storage
            var appSettings = new TestAppSettings(Resolver.App.Settings);
            Resolver.Services.Add(appSettings, typeof(TestAppSettings));

            return base.Initialize();
        }

        public async override Task Run()
        {
            Resolver.Log.Info("Run...");

            await NetworkController.Instance.InitializeWifiNetwork();
        }

    }
}