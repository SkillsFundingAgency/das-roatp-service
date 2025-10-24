using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Filters;
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Api.UnitTests.Filters;
public class RequiredHeaderAttributeTests
{
    [Test]
    public void OnActionExecuting_HeaderMissing_ShouldReturnBadRequest()
    {
        //Arrange
        var headerName = "X-Test-Header";
        RequiredHeaderAttribute sut = new(headerName);
        var actionCotext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        var context = new ActionExecutingContext(actionCotext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: null);
        var expectedResponse = new BadRequestObjectResult(new
        {
            Error = $"Missing required header: '{headerName}'."
        });

        // Act
        sut.OnActionExecuting(context);
        var result = context.Result.As<BadRequestObjectResult>();

        // Assert
        context.Result.Should().BeOfType<BadRequestObjectResult>();
        result.Should().BeEquivalentTo(expectedResponse);
    }

    [Test]
    public void OnActionExecuting_HeaderPresent_ShouldNotSetResult()
    {
        //Arrange
        var headerName = "X-Test-Header";
        RequiredHeaderAttribute sut = new(headerName);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[headerName] = "Test-Header";
        var actionCotext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var context = new ActionExecutingContext(actionCotext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: null);

        // Act
        sut.OnActionExecuting(context);

        // Assert
        context.Result.Should().BeNull();
    }
}