﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TVProgViewer.Core.Events;
using TVProgViewer.Services.Events;
using TVProgViewer.Web.Framework.Events;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that publish ModelReceived event before the action executes, after model binding is complete
    /// and publish ModelPrepared event after the action executes, before the action result
    /// </summary>
    public sealed class PublishModelEventsAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public PublishModelEventsAttribute(bool ignore = false) : base(typeof(PublishModelEventsFilter))
        {
            IgnoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter { get; }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents filter that publish ModelReceived event before the action executes, after model binding is complete
        /// and publish ModelPrepared event after the action executes, before the action result
        /// </summary>
        private class PublishModelEventsFilter : IAsyncActionFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly IEventPublisher _eventPublisher;

            #endregion

            #region Ctor

            public PublishModelEventsFilter(bool ignoreFilter,
                IEventPublisher eventPublisher)
            {
                _ignoreFilter = ignoreFilter;
                _eventPublisher = eventPublisher;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that on completion indicates the necessary filter actions have been executed</returns>
            private async Task PublishModelReceivedEventAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter)
                    .OfType<PublishModelEventsAttribute>()
                    .FirstOrDefault();

                //whether to ignore this filter
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (context.HttpContext.Request == null)
                    return;

                //only in POST requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //model received event
                foreach (var model in context.ActionArguments.Values.OfType<BaseTvProgModel>())
                {
                    //we publish the ModelReceived event for all models as the BaseTvProgModel, 
                    //so you need to implement IConsumer<ModelReceived<BaseTvProgModel>> interface to handle this event
                    await _eventPublisher.ModelReceivedAsync(model, context.ModelState);
                }
            }

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that on completion indicates the necessary filter actions have been executed</returns>
            private async Task PublishModelPreparedEventAsync(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter)
                    .OfType<PublishModelEventsAttribute>()
                    .FirstOrDefault();

                //whether to ignore this filter
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (context.HttpContext.Request == null)
                    return;

                //model prepared event
                if (context.Controller is Controller controller)
                {
                    if (controller.ViewData.Model is BaseTvProgModel model)
                    {
                        //we publish the ModelPrepared event for all models as the BaseTvProgModel, 
                        //so you need to implement IConsumer<ModelPrepared<BaseTvProgModel>> interface to handle this event
                        await _eventPublisher.ModelPreparedAsync(model);
                    }

                    if (controller.ViewData.Model is IEnumerable<BaseTvProgModel> modelCollection)
                    {
                        //we publish the ModelPrepared event for collection as the IEnumerable<BaseTvProgModel>, 
                        //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseTvProgModel>>> interface to handle this event
                        await _eventPublisher.ModelPreparedAsync(modelCollection);
                    }
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
            /// <returns>A task that on completion indicates the filter has executed</returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                await PublishModelReceivedEventAsync(context);
                if (context.Result == null)
                    await next();
                await PublishModelPreparedEventAsync(context);
            }

            #endregion
        }

        #endregion
    }
}