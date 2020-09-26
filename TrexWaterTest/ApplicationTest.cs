using System;
using TrexWater;
using Xunit;

namespace TrexWaterTest
{
	public class ApplicationTest : IClassFixture<ProgramFixture>
	{
		private ProgramFixture Fixture { get; }
		public ApplicationTest(ProgramFixture fixture)
		{
			Fixture = fixture;
		}

		[Fact]
		public void RunWithoutException()
		{
			Fixture.RunMain();
		}
	}
}
