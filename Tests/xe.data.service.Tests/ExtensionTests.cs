using System;
using xe.data.service.Extensions;
using xe.data.service.Models;
using Xunit;

namespace xe.data.service.Tests
{
    public class ExtensionTests
    {
        [Theory]
        [InlineData("SqLsErVeR", DatabaseType.SqlServer)]
        [InlineData("OrAcLe", DatabaseType.Oracle)]
        [InlineData("MySqL", DatabaseType.MySql)]
        public void ValidateDatabaseTypeMapping(string text, DatabaseType expectedType)
        {
            Assert.Equal(expectedType, text.ToDatabaseType());
        }

        [Fact]
        public void ValidateExceptionThrown()
        {
            Assert.Throws<InvalidOperationException>(() => "unknown".ToDatabaseType());
        }
    }
}
