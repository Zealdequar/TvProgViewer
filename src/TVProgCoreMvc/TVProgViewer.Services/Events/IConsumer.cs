﻿using System.Threading.Tasks;

namespace TVProgViewer.Services.Events
{
    /// <summary>
    /// Consumer interface
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public interface IConsumer<T>
    {
        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="eventMessage">Event</param>
        Task HandleEventAsync(T eventMessage);
    }
}
