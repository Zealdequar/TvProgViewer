﻿using Microsoft.AspNetCore.Routing;

namespace TVProgViewer.TVProgUpdaterV2.Mvc.Routing
{
    /// <summary>
    /// Represents route publisher
    /// </summary>
    public interface IRoutePublisher
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        void RegisterRoutes(IEndpointRouteBuilder routeBuilder);
    }
}