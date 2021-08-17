using System;
using AutoFixture;
using AutoFixture.Idioms;
using Elzik.Mecon.Framework.Domain;
using FluentAssertions;
using Xunit;

namespace Elzik.Mecon.Framework.Tests.Unit.Domain
{
    public class EntryKeyTests
    {
        private readonly IFixture _fixture;

        public EntryKeyTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void ConstructorNullChecks()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            assertion.Verify(typeof(EntryKey).GetConstructors());
        }

        [Fact]
        public void Equals_MismatchingFilenameAndSize_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 1);
            var testEntryKey2 = new EntryKey("fileName2", 2);

            // Act
            var areEqual = testEntryKey1.Equals(testEntryKey2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_SameFilenameMismatchingSize_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName", 1);
            var testEntryKey2 = new EntryKey("fileName", 2);

            // Act
            var areEqual = testEntryKey1.Equals(testEntryKey2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_SameSizeMismatchingFilename_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 0);
            var testEntryKey2 = new EntryKey("fileName2", 0);

            // Act
            var areEqual = testEntryKey1.Equals(testEntryKey2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_ComparedToNull_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            var areEqual = testEntryKey1.Equals(null);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_SameSizeSameFilename_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = new EntryKey(testEntryKey1.Filename, testEntryKey1.ByteCount);

            // Act
            var areEqual = testEntryKey1.Equals(testEntryKey2);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_SameInstance_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = testEntryKey1;

            // Act
            var areEqual = testEntryKey1.Equals(testEntryKey2);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_SameFilenameMismatchingSizeAsObject_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName", 1);
            var testEntryKey2 = new EntryKey("fileName", 2);

            // Act
            var areEqual = testEntryKey1.Equals((object)testEntryKey2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_SameSizeMismatchingFilenameAsObject_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 0);
            var testEntryKey2 = new EntryKey("fileName2", 0);

            // Act
            var areEqual = testEntryKey1.Equals((object)testEntryKey2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_ComparedToNullAsObject_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            var areEqual = testEntryKey1.Equals((object)null);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_DifferentTypes_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            // ReSharper disable once SuspiciousTypeConversion.Global - This is necessary for testing purposes.
            var areEqual = testEntryKey1.Equals(string.Empty);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_SameSizeSameFilenameAsObject_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = new EntryKey(testEntryKey1.Filename, testEntryKey1.ByteCount);

            // Act
            var areEqual = testEntryKey1.Equals((object)testEntryKey2);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_SameInstanceAsObject_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = testEntryKey1;

            // Act
            var areEqual = testEntryKey1.Equals((object)testEntryKey2);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void EqualsOperator_MismatchingFilenameAndSize_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 1);
            var testEntryKey2 = new EntryKey("fileName2", 2);

            // Act
            var areEqual = testEntryKey1 == testEntryKey2;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_SameFilenameMismatchingSize_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName", 1);
            var testEntryKey2 = new EntryKey("fileName", 2);

            // Act
            var areEqual = testEntryKey1 == testEntryKey2;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_SameSizeMismatchingFilename_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 0);
            var testEntryKey2 = new EntryKey("fileName2", 0);

            // Act
            var areEqual = testEntryKey1 == testEntryKey2;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_ComparedToNull_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            var areEqual = testEntryKey1 == null;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_SameSizeSameFilename_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = new EntryKey(testEntryKey1.Filename, testEntryKey1.ByteCount);

            // Act
            var areEqual = testEntryKey1 == testEntryKey2;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void EqualsOperator_SameInstance_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = testEntryKey1;

            // Act
            var areEqual = testEntryKey1 == testEntryKey2;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void EqualsOperator_SameFilenameMismatchingSizeAsObject_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName", 1);
            var testEntryKey2 = new EntryKey("fileName", 2);

            // Act
            var areEqual = testEntryKey1 == (object)testEntryKey2;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_SameSizeMismatchingFilenameAsObject_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 0);
            var testEntryKey2 = new EntryKey("fileName2", 0);

            // Act
            var areEqual = testEntryKey1 == (object)testEntryKey2;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_ComparedToNullAsObject_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            var areEqual = testEntryKey1 == (object)null;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_DifferentTypes_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - This is necessary for testing purposes.
            // ReSharper disable once SuspiciousTypeConversion.Global - This is necessary for testing purposes.
            var areEqual = testEntryKey1 == (object)string.Empty;

            // Assert
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - This is necessary for testing purposes.
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void EqualsOperator_SameInstanceAsObject_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = testEntryKey1;

            // Act
            var areEqual = testEntryKey1 == (object)testEntryKey2;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__MismatchingFilenameAndSize_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 1);
            var testEntryKey2 = new EntryKey("fileName2", 2);

            // Act
            var areEqual = testEntryKey1 != testEntryKey2;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__SameFilenameMismatchingSize_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName", 1);
            var testEntryKey2 = new EntryKey("fileName", 2);

            // Act
            var areEqual = testEntryKey1 != testEntryKey2;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__SameSizeMismatchingFilename_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 0);
            var testEntryKey2 = new EntryKey("fileName2", 0);

            // Act
            var areEqual = testEntryKey1 != testEntryKey2;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__ComparedToNull_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            var areEqual = testEntryKey1 != null;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__SameSizeSameFilename_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = new EntryKey(testEntryKey1.Filename, testEntryKey1.ByteCount);

            // Act
            var areEqual = testEntryKey1 != testEntryKey2;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void NotEqualsOperator__SameInstance_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = testEntryKey1;

            // Act
            var areEqual = testEntryKey1 != testEntryKey2;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void NotEqualsOperator__SameFilenameMismatchingSizeAsObject_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName", 1);
            var testEntryKey2 = new EntryKey("fileName", 2);

            // Act
            var areEqual = testEntryKey1 != (object)testEntryKey2;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__SameSizeMismatchingFilenameAsObject_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = new EntryKey("fileName1", 0);
            var testEntryKey2 = new EntryKey("fileName2", 0);

            // Act
            var areEqual = testEntryKey1 != (object)testEntryKey2;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__ComparedToNullAsObject_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            var areEqual = testEntryKey1 != (object)null;

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__DifferentTypes_ReturnsTrue()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();

            // Act
            // ReSharper disable once SuspiciousTypeConversion.Global - This is necessary for testing purposes.
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - This is necessary for testing purposes.
            var areEqual = testEntryKey1 != (object)string.Empty;

            // Assert
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - This is necessary for testing purposes.
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void NotEqualsOperator__SameInstanceAsObject_ReturnsFalse()
        {
            // Arrange
            var testEntryKey1 = _fixture.Create<EntryKey>();
            var testEntryKey2 = testEntryKey1;

            // Act
            var areEqual = testEntryKey1 != (object)testEntryKey2;

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithValidInput_ReturnsHashCodeCombineImplementation()
        {
            // Arrange
            var testEntryKey = new EntryKey(
                _fixture.Create<string>(), 
                _fixture.Create<long>());

            // Act
            var hashCode = testEntryKey.GetHashCode();

            // Assert
            hashCode.Should().Be(HashCode.Combine(testEntryKey.Filename, testEntryKey.ByteCount));
        }
    }
}
