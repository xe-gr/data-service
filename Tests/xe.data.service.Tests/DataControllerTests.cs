using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
            var mockService = new Mock<IDataService>(MockBehavior.Strict);
            mockService.Setup(x => x.ExecuteRequest("name", "param1", "value1"))
                .Throws(new ConfigurationNotFoundException("test")).Verifiable();

            var controller = new DataController(mockService.Object);

            var result = controller.Get("name", "param1", "value1");

            Assert.NotNull(result);

            var r = result as NotFoundObjectResult;

            Assert.NotNull(r);
            Assert.Equal("test", r.Value);

            mockService.Verify(x => x.ExecuteRequest("name", "param1", "value1"), Times.Once);
        }

        [Fact]
        public void BadRequest()
        {
            var mockService = new Mock<IDataService>(MockBehavior.Strict);
            mockService.Setup(x => x.ExecuteRequest("name", "param1", "value1"))
                .Throws(new BadRequestException("test")).Verifiable();

            var controller = new DataController(mockService.Object);

            var result = controller.Get("name", "param1", "value1");

            Assert.NotNull(result);

            var r = result as BadRequestObjectResult;

            Assert.NotNull(r);
            Assert.Equal("test", r.Value);

            mockService.Verify(x => x.ExecuteRequest("name", "param1", "value1"), Times.Once);
        }

        [Fact]
        public void ThrowsError()
        {
            var mockService = new Mock<IDataService>(MockBehavior.Strict);
            mockService.Setup(x => x.ExecuteRequest("name", "param1", "value1"))
                .Throws(new InvalidOperationException("test")).Verifiable();

            var controller = new DataController(mockService.Object);

            try
            {
                controller.Get("name", "param1", "value1");
                Assert.False(true, "Should get an exception");
            }
            catch (InvalidOperationException e)
            {
                Assert.Equal("test", e.Message);
            }
            
            mockService.Verify(x => x.ExecuteRequest("name", "param1", "value1"), Times.Once);
        }

        [Fact]
        public void Ok()
        {
            var mockService = new Mock<IDataService>(MockBehavior.Strict);
            mockService.Setup(x => x.ExecuteRequest("name", "param1", "value1")).Returns(new List<dynamic>()).Verifiable();

            var controller = new DataController(mockService.Object);

            var result = controller.Get("name", "param1", "value1");

            Assert.NotNull(result);

            var r = result as OkObjectResult;

            Assert.NotNull(r);

            mockService.Verify(x => x.ExecuteRequest("name", "param1", "value1"), Times.Once);
        }
    }
}