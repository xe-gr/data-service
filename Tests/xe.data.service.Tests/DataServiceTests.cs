using System.Collections.Generic;
using System.Data;
using FakeItEasy;
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
            var mockReader = A.Fake<IConfigurationReader>(x => x.Strict());
            A.CallTo(() => mockReader.ReadConfiguration())
                .Returns([]);

            var service = new DataService(mockReader, null, null);

            Assert.Equal("Configuration not found",
                Assert.Throws<ConfigurationNotFoundException>(() => service.ExecuteRequest("name", null, null)).Message
            );

            A.CallTo(() => mockReader.ReadConfiguration())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void InvalidParametersPassed()
        {
            var mockReader = A.Fake<IConfigurationReader>(x => x.Strict());
            A.CallTo(() => mockReader.ReadConfiguration())
                .Returns([new() { Name = "name", Parameters = "param1,param2" }]);

            var service = new DataService(mockReader, null, null);

            Assert.Equal("Parameters passed are incorrect",
                Assert.Throws<BadRequestException>(() => service.ExecuteRequest("name", "param1", null)).Message
            );

            A.CallTo(() => mockReader.ReadConfiguration())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void InvalidValuesPassed()
        {
            var mockReader = A.Fake<IConfigurationReader>(x => x.Strict());
            A.CallTo(() => mockReader.ReadConfiguration())
                .Returns([new() { Name = "name", Parameters = "param1,param2" }]);

            var service = new DataService(mockReader, null, null);

            Assert.Equal("Passed values are incorrect",
                Assert.Throws<BadRequestException>(() => service.ExecuteRequest("name", "param1,param2", "value1")).Message
            );

            A.CallTo(() => mockReader.ReadConfiguration())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void UnknownParametersPassed()
        {
            var mockReader = A.Fake<IConfigurationReader>(x => x.Strict());
            A.CallTo(() => mockReader.ReadConfiguration())
                .Returns([new() { Name = "name", Parameters = "param1,param2", SqlCommand = "some command" }]);

            var service = new DataService(mockReader, null, null);

            Assert.Equal("Parameter param3 is unknown",
                Assert.Throws<BadRequestException>(() => service.ExecuteRequest("name", "param1,param3", "value1,value2")).Message
            );

            A.CallTo(() => mockReader.ReadConfiguration())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EmptyResultSet()
        {
            var mockReader = A.Fake<IConfigurationReader>(x => x.Strict());
            A.CallTo(() => mockReader.ReadConfiguration())
                .Returns([new() { Name = "name", Parameters = "param1,param2", SqlCommand = "some command" }]);

            var ds = new DataSet();
            ds.Tables.Add(new DataTable());

            var mockRetriever = A.Fake<IDataRetriever>(x => x.Strict());
            A.CallTo(() => mockRetriever.RetrieveData(A<IDataCreator>.Ignored, A<string>.Ignored, A<string>.Ignored,
                    A<string>.Ignored, A<int>.Ignored))
                .Returns(ds);

            var service = new DataService(mockReader, null, mockRetriever);

            var result = service.ExecuteRequest("name", "param1,param2", "value1,value2");

            Assert.NotNull(result);
            Assert.Empty(result);

            A.CallTo(() => mockReader.ReadConfiguration())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => mockRetriever.RetrieveData(A<IDataCreator>.Ignored, A<string>.Ignored, A<string>.Ignored,
                A<string>.Ignored, A<int>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ResultSetWithData()
        {
            var mockReader = A.Fake<IConfigurationReader>(x => x.Strict());
            A.CallTo(() => mockReader.ReadConfiguration())
                .Returns([new() { Name = "name", Parameters = "param1,param2", SqlCommand = "some command" }]);

            var ds = new DataSet();
            var dt = new DataTable();
            dt.Columns.Add("c1", typeof(string));
            dt.Columns.Add("c2", typeof(int));
            dt.Rows.Add("a", 1);
            dt.Rows.Add("b", 2);
            ds.Tables.Add(dt);

            var mockRetriever = A.Fake<IDataRetriever>(x => x.Strict());
            A.CallTo(() => mockRetriever.RetrieveData(A<IDataCreator>.Ignored, A<string>.Ignored, A<string>.Ignored,
                    A<string>.Ignored, A<int>.Ignored))
                .Returns(ds);

            var service = new DataService(mockReader, null, mockRetriever);

            var result = service.ExecuteRequest("name", "param1,param2", "value1,value2");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, ((List<object>) result).Count);

            A.CallTo(() => mockReader.ReadConfiguration())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => mockRetriever.RetrieveData(A<IDataCreator>.Ignored, A<string>.Ignored, A<string>.Ignored,
                    A<string>.Ignored, A<int>.Ignored))
                .MustHaveHappenedOnceExactly();
        }
    }
}