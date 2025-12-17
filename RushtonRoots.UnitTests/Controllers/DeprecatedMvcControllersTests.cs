using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Web.Controllers;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers;

/// <summary>
/// Tests to verify that deprecated MVC POST actions are properly marked with [Obsolete] attribute
/// as part of Phase 4.3: Deprecate Old MVC POST Patterns
/// </summary>
public class DeprecatedMvcControllersTests
{
    [Theory]
    [InlineData(typeof(PersonController), "Create", typeof(Domain.UI.Requests.CreatePersonRequest))]
    [InlineData(typeof(PersonController), "Edit", typeof(int), typeof(Domain.UI.Requests.UpdatePersonRequest))]
    [InlineData(typeof(PersonController), "DeleteConfirmed", typeof(int))]
    public void PersonController_PostActions_ShouldBeMarkedObsolete(Type controllerType, string methodName, params Type[] parameterTypes)
    {
        // Arrange & Act
        var method = controllerType.GetMethod(methodName, parameterTypes);

        // Assert
        Assert.NotNull(method);
        var obsoleteAttribute = method.GetCustomAttribute<ObsoleteAttribute>();
        Assert.NotNull(obsoleteAttribute);
        Assert.Contains("deprecated", obsoleteAttribute.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("/api/person", obsoleteAttribute.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(typeof(PartnershipController), "Create", typeof(Domain.UI.Requests.CreatePartnershipRequest))]
    [InlineData(typeof(PartnershipController), "Edit", typeof(int), typeof(Domain.UI.Requests.UpdatePartnershipRequest))]
    [InlineData(typeof(PartnershipController), "DeleteConfirmed", typeof(int))]
    public void PartnershipController_PostActions_ShouldBeMarkedObsolete(Type controllerType, string methodName, params Type[] parameterTypes)
    {
        // Arrange & Act
        var method = controllerType.GetMethod(methodName, parameterTypes);

        // Assert
        Assert.NotNull(method);
        var obsoleteAttribute = method.GetCustomAttribute<ObsoleteAttribute>();
        Assert.NotNull(obsoleteAttribute);
        Assert.Contains("deprecated", obsoleteAttribute.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("/api/partnership", obsoleteAttribute.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(typeof(ParentChildController), "Create", typeof(Domain.UI.Requests.CreateParentChildRequest))]
    [InlineData(typeof(ParentChildController), "Edit", typeof(int), typeof(Domain.UI.Requests.UpdateParentChildRequest))]
    [InlineData(typeof(ParentChildController), "DeleteConfirmed", typeof(int))]
    public void ParentChildController_PostActions_ShouldBeMarkedObsolete(Type controllerType, string methodName, params Type[] parameterTypes)
    {
        // Arrange & Act
        var method = controllerType.GetMethod(methodName, parameterTypes);

        // Assert
        Assert.NotNull(method);
        var obsoleteAttribute = method.GetCustomAttribute<ObsoleteAttribute>();
        Assert.NotNull(obsoleteAttribute);
        Assert.Contains("deprecated", obsoleteAttribute.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("/api/parentchild", obsoleteAttribute.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(typeof(PersonController), "Create")]
    [InlineData(typeof(PersonController), "Edit")]
    [InlineData(typeof(PersonController), "DeleteConfirmed")]
    [InlineData(typeof(PartnershipController), "Create")]
    [InlineData(typeof(PartnershipController), "Edit")]
    [InlineData(typeof(PartnershipController), "DeleteConfirmed")]
    [InlineData(typeof(ParentChildController), "Create")]
    [InlineData(typeof(ParentChildController), "Edit")]
    [InlineData(typeof(ParentChildController), "DeleteConfirmed")]
    public void DeprecatedPostActions_ShouldHaveHttpPostAttribute(Type controllerType, string methodName)
    {
        // Arrange & Act
        var methods = controllerType.GetMethods()
            .Where(m => m.Name == methodName && m.GetCustomAttribute<HttpPostAttribute>() != null)
            .ToList();

        // Assert
        Assert.NotEmpty(methods);
        Assert.All(methods, method =>
        {
            var httpPostAttribute = method.GetCustomAttribute<HttpPostAttribute>();
            Assert.NotNull(httpPostAttribute);
        });
    }

    [Fact]
    public void AllDeprecatedActions_ShouldHaveMigrationGuideLink()
    {
        // Arrange
        var controllers = new[] { typeof(PersonController), typeof(PartnershipController), typeof(ParentChildController) };
        var deprecatedMethods = controllers
            .SelectMany(c => c.GetMethods())
            .Where(m => m.GetCustomAttribute<ObsoleteAttribute>() != null)
            .ToList();

        // Assert
        Assert.NotEmpty(deprecatedMethods);
        Assert.All(deprecatedMethods, method =>
        {
            var obsoleteAttribute = method.GetCustomAttribute<ObsoleteAttribute>();
            Assert.Contains("ApiEndpointsImplementationPlan.md", obsoleteAttribute!.Message);
            Assert.Contains("phase-43", obsoleteAttribute.Message, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public void DeprecatedActions_Count_ShouldBeNine()
    {
        // Arrange
        var controllers = new[] { typeof(PersonController), typeof(PartnershipController), typeof(ParentChildController) };
        
        // Act
        var deprecatedMethodsCount = controllers
            .SelectMany(c => c.GetMethods())
            .Count(m => m.GetCustomAttribute<ObsoleteAttribute>() != null);

        // Assert - 3 controllers * 3 methods each (Create, Edit, Delete)
        Assert.Equal(9, deprecatedMethodsCount);
    }
}
