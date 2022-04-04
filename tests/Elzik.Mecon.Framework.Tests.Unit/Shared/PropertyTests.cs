using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Shared
{
    public abstract class PropertyTests<T>
    {
        protected readonly IFixture Fixture;

        protected PropertyTests()
        {
            Fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void WritablePropertyAssertions()
        {
            var assertion = new WritablePropertyAssertion(Fixture);

            assertion.Verify(typeof(T).GetProperties());
        }
    }
}
