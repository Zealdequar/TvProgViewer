﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Services.Polls;
using TVProgViewer.WebUI.Infrastructure.Cache;
using TVProgViewer.WebUI.Models.Polls;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the poll model factory
    /// </summary>
    public partial class PollModelFactory : IPollModelFactory
    {
        #region Fields

        private readonly IPollService _pollService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PollModelFactory(IPollService pollService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _pollService = pollService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the poll model
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that user already voted for this poll</param>
        /// <returns>Poll model</returns>
        public virtual async Task<PollModel> PreparePollModelAsync(Poll poll, bool setAlreadyVotedProperty)
        {
            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            var model = new PollModel
            {
                Id = poll.Id,
                AlreadyVoted = setAlreadyVotedProperty && await _pollService.AlreadyVotedAsync(poll.Id, (await _workContext.GetCurrentUserAsync()).Id),
                Name = poll.Name
            };
            var answers = await _pollService.GetPollAnswerByPollAsync(poll.Id);

            foreach (var answer in answers)
                model.TotalVotes += answer.NumberOfVotes;
            foreach (var pa in answers)
            {
                model.Answers.Add(new PollAnswerModel
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    NumberOfVotes = pa.NumberOfVotes,
                    PercentOfTotalVotes = model.TotalVotes > 0 ? ((Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes)) * Convert.ToDouble(100)) : 0,
                });
            }

            return model;
        }

        /// <summary>
        /// Get the poll model by poll system keyword
        /// </summary>
        /// <param name="systemKeyword">Poll system keyword</param>
        /// <returns>Poll model</returns>
        public virtual async Task<PollModel> PreparePollModelBySystemNameAsync(string systemKeyword)
        {
            if (string.IsNullOrWhiteSpace(systemKeyword))
                return null;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.PollBySystemNameModelKey,
                systemKeyword, await _workContext.GetWorkingLanguageAsync(), await _storeContext.GetCurrentStoreAsync());

            var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var poll = (await _pollService
                    .GetPollsAsync((await _storeContext.GetCurrentStoreAsync()).Id, (await _workContext.GetWorkingLanguageAsync()).Id, systemKeyword: systemKeyword))
                    .FirstOrDefault();

                //we do not cache nulls. that's why let's return an empty record (ID = 0)
                if (poll == null)
                    return new PollModel { Id = 0 };

                return await PreparePollModelAsync(poll, false);
            });

            if ((cachedModel?.Id ?? 0) == 0)
                return null;

            //"AlreadyVoted" property of "PollModel" object depends on the current user. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = cachedModel with { };
            model.AlreadyVoted = await _pollService.AlreadyVotedAsync(model.Id, (await _workContext.GetCurrentUserAsync()).Id);

            return model;
        }

        /// <summary>
        /// Prepare the home page poll models
        /// </summary>
        /// <returns>List of the poll model</returns>
        public virtual async Task<List<PollModel>> PrepareHomepagePollModelsAsync()
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.HomepagePollsModelKey,
                await _workContext.GetWorkingLanguageAsync(), await _storeContext.GetCurrentStoreAsync());

            var cachedPolls = await (await _staticCacheManager.GetAsync(cacheKey, async () =>
                (await _pollService.GetPollsAsync((await _storeContext.GetCurrentStoreAsync()).Id, (await _workContext.GetWorkingLanguageAsync()).Id, loadShownOnHomepageOnly: true))
                   .SelectAwait(async poll => await PreparePollModelAsync(poll, false)).ToListAsync()));

            //"AlreadyVoted" property of "PollModel" object depends on the current user. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = new List<PollModel>();
            foreach (var poll in cachedPolls)
            {
                var pollModel = poll with { };
                pollModel.AlreadyVoted = await _pollService.AlreadyVotedAsync(pollModel.Id, (await _workContext.GetCurrentUserAsync()).Id);
                model.Add(pollModel);
            }

            return model;
        }

        #endregion
    }
}
