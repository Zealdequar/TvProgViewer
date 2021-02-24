﻿using System.Threading.Tasks;
using TVProgViewer.WebUI.Models.Topics;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the topic model factory
    /// </summary>
    public partial interface ITopicModelFactory
    {
        /// <summary>
        /// Get the topic model by topic identifier
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Topic model</returns>
        Task<TopicModel> PrepareTopicModelByIdAsync(int topicId, bool showHidden = false);

        /// <summary>
        /// Get the topic model by topic system name
        /// </summary>
        /// <param name="systemName">Topic system name</param>
        /// <returns>Topic model</returns>
        Task<TopicModel> PrepareTopicModelBySystemNameAsync(string systemName);

        /// <summary>
        /// Get the topic template view path
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>View path</returns>
        Task<string> PrepareTemplateViewPathAsync(int topicTemplateId);
    }
}
