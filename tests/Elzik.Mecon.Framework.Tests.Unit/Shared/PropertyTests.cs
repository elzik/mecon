using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Shared
{
    public abstract class PropertyTests<T>
    {
        protected readonly IFixture _fixture;

        protected PropertyTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void WritablePropertyAssertions()
        {
            var assertion = new WritablePropertyAssertion(_fixture);

            assertion.Verify(typeof(T).GetProperties());
        }
    }
}
