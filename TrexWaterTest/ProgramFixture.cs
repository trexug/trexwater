using Microsoft.Extensions.DependencyInjection;
using System;
using TrexWater;
using TrexWaterTest.Mock;

namespace TrexWaterTest
{
	public class ProgramFixture : Program
	{
		protected override IServiceProvider ConfigureServices(IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddLogging()
				.AddSingleton<ITimeProvider, TestTimeProvider>()
				.AddSingleton<IGpioPinFactory, TestGpioPinFactory>()
				.AddSingleton<IWaterControllerFactory, WaterControllerFactory>()
				.AddSingleton<IApplication, Application>()
				.BuildServiceProvider();
		}
	}
}
