using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Identity.Protocol.Api;
using Microsoft.AspNetCore.Mvc;

namespace Identity;

public class ControllerInspector
{
    public static List<RouteInfo> Inspect(params Assembly[] assemblies)
    {
        var routes = new List<RouteInfo>();
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(ControllerBase).IsAssignableFrom(type))
                {
                    string routeTemplate = "";
                    var route = type.GetCustomAttribute<RouteAttribute>();
                    if (route != null) routeTemplate = route.Template;
                    foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        // var getAttr = methodInfo.GetCustomAttribute<HttpGetAttribute>();
                        // var putAttr = methodInfo.GetCustomAttribute<HttpPutAttribute>();
                        // var deleteAttr = methodInfo.GetCustomAttribute<HttpDeleteAttribute>();
                        var postAttr = methodInfo.GetCustomAttribute<HttpPostAttribute>();
                        var parameters = methodInfo.GetParameters();
                        if (postAttr != null && parameters.Length == 1)
                        {
                            var parameter = parameters[0];
                            if (typeof(IIdentityRequest).IsAssignableFrom(parameter.ParameterType))
                            {
                                foreach (var @interface in parameter.ParameterType.GetInterfaces())
                                {
                                    if (@interface.GetGenericTypeDefinition() == typeof(IIdentityRequest<>))
                                    {
                                        string methodTemplate = "";
                                        var methodRoute = methodInfo.GetCustomAttribute<RouteAttribute>();
                                        if (methodRoute != null)
                                        {
                                            methodTemplate = methodRoute.Template;
                                        }
                                        else
                                        {
                                            methodTemplate = methodInfo.Name;
                                        }
                                        routes.Add(new RouteInfo
                                        {
                                            Path = string.Join("/", new[] {routeTemplate, methodTemplate}.Select(x => x.Trim(' ', '/', '\\')).ToArray()),
                                            RequestType = parameter.ParameterType,
                                            ResponseType = @interface.GetGenericArguments()[0]
                                        });
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return routes;
    }

    public class RouteInfo
    {
        public string Path { get; set; }
        public Type RequestType { get; set; }
        public Type ResponseType { get; set; }
    }
}