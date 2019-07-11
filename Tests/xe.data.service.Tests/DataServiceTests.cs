using System.Collections.Generic;
using System.Data;
using Moq;
using xe.data.service.Exceptions;
using xe.data.service.Models;
using xe.data.service.Services;
using xe.data.service.Services.Interfaces;
using Xunit;

namespace xe.data.service.Tests
{
    public class DataServiceTests
    {
        [Fact]
        public void NoName()
        {
            var service = new DataService(null, null, null);

            Assert.Equal("No config requested",
                Assert.Throws<BadRequestException>(() => service.ExecuteRequest(null, null, null)).Message
            );
        }

        [Fact]
        public void NameNotFoundWithoutConfig()
        {
            var mockReader = new Mock<IConfigurationReader>(MockBehavior.Strict);
            mockReader.Setup(x => x.ReadConfiguration()).Returns(new List<ConfigurationEntry>()).Verifiable();

            var service = new DataService(mockReader.Object, null, null);

            Assert.Equal("Configuration not found",
                Assert.Throws<ConfigurationNotFoundException>(() => service.ExecuteRequest("name", null, null)).Message
            );

            mockReader.Verify(x => x.ReadConfiguration(), Times.Once);
        }

        [Fact]
        public void InvalidParametersPassed()
        {
            var mockReader = new Mock<IConfigurationReader>(MockBehavior.Strict);
            mockReader.Setup(x => x.ReadConfiguration()).Returns(new List<ConfigurationEntry>
                {new ConfigurationEntry {Name = "name", Parameters = "param1,param2"}}).Verifiable();

            var service = new DataService(mockReader.Object, null, null);

            Assert.Equal("Parameters passed are incorrect",
                Assert.Throws<BadRequestException>(() => service.ExecuteRequest("name", "param1", null)).Message
            );

            mockReader.Verify(x => x.ReadConfiguration(), Times.Once);
        }

        [Fact]
        public void InvalidValuesPassed()
        {
            var mockReader = new Mock<IConfigurationReader>(MockBehavior.Strict);
            mockReader.Setup(x => x.ReadConfiguration()).Returns(new List<ConfigurationEntry>
                {new ConfigurationEntry {Name = "name", Parameters = "param1,param2"}}).Verifiable();

            var service = new DataService(mockReader.Object, null, null);

            Assert.Equal("Passed values are incorrect",
                Assert.Throws<BadRequestException>(() => service.ExecuteRequest("name", "param1,param2", "value1")).Message
            );

            mockReader.Verify(x => x.ReadConfiguration(), Times.Once);
        }

        [Fact]
        public void UnknownParametersPassed()
        {
            var mockReader = new Mock<IConfigurationReader>(MockBehavior.Strict);
            mockReader.Setup(x => x.ReadConfiguration()).Returns(new List<ConfigurationEntry>
                {new ConfigurationEntry {Name = "name", Parameters = "param1,param2", SqlCommand = "some command"}}).Verifiable();

            var service = new DataService(mockReader.Object, null, null);

            Assert.Equal("Parameter param3 is unknown",
                Assert.Throws<BadRequestException>(() => service.ExecuteRequest("name", "param1,param3", "value1,value2")).Message
            );

            mockReader.Verify(x => x.ReadConfiguration(), Times.Once);
        }

        [Fact]
        public void EmptyResultSet()
        {
            var mockReader = new Mock<IConfigurationReader>(MockBehavior.Strict);
            mockReader.Setup(x => x.ReadConfiguration()).Returns(new List<ConfigurationEntry>
                {new ConfigurationEntry {Name = "name", Parameters = "param1,param2", SqlCommand = "some command"}}).Verifiable();

            var mockRetriever = new Mock<IDataRetriever>(MockBehavior.Strict);
            mockRetriever.Setup(x => x.RetrieveData(It.IsAny<IDataCreator>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() =>
                {
                    var ds = new DataSet();
                    ds.Tables.Add(new DataTable());
                    return ds;

                }).Verifiable();

            var service = new DataService(mockReader.Object, null, mockRetriever.Object);

            var result = service.ExecuteRequest("name", "param1,param2", "value1,value2");

            Assert.NotNull(result);
            Assert.Empty(result);

            mockReader.Verify(x => x.ReadConfiguration(), Times.Once);
            mockRetriever.Verify(x => x.RetrieveData(It.IsAny<IDataCreator>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ResultSetWithData()
        {
            var mockReader = new Mock<IConfigurationReader>(MockBehavior.Strict);
            mockReader.Setup(x => x.ReadConfiguration()).Returns(new List<ConfigurationEntry>
                {new ConfigurationEntry {Name = "name", Parameters = "param1,param2", SqlCommand = "some command"}}).Verifiable();

            var mockRetriever = new Mock<IDataRetriever>(MockBehavior.Strict);
            mockRetriever.Setup(x => x.RetrieveData(It.IsAny<IDataCreator>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() =>
                {
                    var ds = new DataSet();
                    var dt = new DataTable();
                    dt.Columns.Add("c1", typeof(string));
                    dt.Columns.Add("c2", typeof(int));
                    dt.Rows.Add("a", 1);
                    dt.Rows.Add("b", 2);
                    ds.Tables.Add(dt);
                    return ds;

                }).Verifiable();

            var service = new DataService(mockReader.Object, null, mockRetriever.Object);

            var result = service.ExecuteRequest("name", "param1,param2", "value1,value2");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, ((List<object>) result).Count);

            mockReader.Verify(x => x.ReadConfiguration(), Times.Once);
            mockRetriever.Verify(x => x.RetrieveData(It.IsAny<IDataCreator>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }
}
