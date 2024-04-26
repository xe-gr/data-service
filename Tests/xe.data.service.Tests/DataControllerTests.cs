using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using xe.data.service.Controllers;
using xe.data.service.Exceptions;
using xe.data.service.Services.Interfaces;
using Xunit;

namespace xe.data.service.Tests
{
    public class DataControllerTests
    {
        [Fact]
        public void ConfigurationNotFound()
        {
            var mockService = A.Fake<IDataService>(x => x.Strict());
            A.CallTo(() => mockService.ExecuteRequest("name", "param1", "value1"))
                .Throws(new ConfigurationNotFoundException("test"));

            var controller = new DataController(mockService);

            var result = controller.Get("name", "param1", "value1");

            Assert.NotNull(result);

            var r = result as NotFoundObjectResult;

            Assert.NotNull(r);
            Assert.Equal("test", r.Value);

            A.CallTo(() => mockService.ExecuteRequest("name", "param1", "value1"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void BadRequest()
        {
            var mockService = A.Fake<IDataService>(x => x.Strict());
            A.CallTo(() => mockService.ExecuteRequest("name", "param1", "value1"))
                .Throws(new BadRequestException("test"));

            var controller = new DataController(mockService);

            var result = controller.Get("name", "param1", "value1");

            Assert.NotNull(result);

            var r = result as BadRequestObjectResult;

            Assert.NotNull(r);
            Assert.Equal("test", r.Value);

            A.CallTo(() => mockService.ExecuteRequest("name", "param1", "value1"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ThrowsError()
        {
            var mockService = A.Fake<IDataService>(x => x.Strict());
            A.CallTo(() => mockService.ExecuteRequest("name", "param1", "value1"))
                .Throws(new InvalidOperationException("test"));

            var controller = new DataController(mockService);

            try
            {
                controller.Get("name", "param1", "value1");
                Assert.Fail("Should get an exception");
            }
            catch (InvalidOperationException e)
            {
                Assert.Equal("test", e.Message);
            }

            A.CallTo(() => mockService.ExecuteRequest("name", "param1", "value1"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Ok()
        {
            var mockService = A.Fake<IDataService>(x => x.Strict());
            A.CallTo(() => mockService.ExecuteRequest("name", "param1", "value1"))
                .Returns(new List<dynamic>());

            var controller = new DataController(mockService);

            var result = controller.Get("name", "param1", "value1");

            Assert.NotNull(result);

            var r = result as OkObjectResult;

            Assert.NotNull(r);

            A.CallTo(() => mockService.ExecuteRequest("name", "param1", "value1"))
                .MustHaveHappenedOnceExactly();
        }
    }
}