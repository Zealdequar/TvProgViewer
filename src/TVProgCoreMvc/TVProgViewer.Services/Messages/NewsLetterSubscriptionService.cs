﻿using System;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Data;
using TVProgViewer.Core.Events;
using TVProgViewer.Services.Users;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Messages
{
    /// <summary>
    /// Newsletter subscription service
    /// </summary>
    public class NewsLetterSubscriptionService : INewsLetterSubscriptionService
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserUserRoleMapping> _userUserRoleMappingRepository;
        private readonly IRepository<NewsLetterSubscription> _subscriptionRepository;

        #endregion

        #region Ctor

        public NewsLetterSubscriptionService(IUserService userService,
            IEventPublisher eventPublisher,
            IRepository<User> userRepository,
            IRepository<UserUserRoleMapping> userUserRoleMappingRepository,
            IRepository<NewsLetterSubscription> subscriptionRepository)
        {
            _userService = userService;
            _eventPublisher = eventPublisher;
            _userRepository = userRepository;
            _userUserRoleMappingRepository = userUserRoleMappingRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Publishes the subscription event.
        /// </summary>
        /// <param name="subscription">The newsletter subscription.</param>
        /// <param name="isSubscribe">if set to <c>true</c> [is subscribe].</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        private async Task PublishSubscriptionEventAsync(NewsLetterSubscription subscription, bool isSubscribe, bool publishSubscriptionEvents)
        {
            if (!publishSubscriptionEvents)
                return;

            if (isSubscribe)
            {
                await _eventPublisher.PublishNewsletterSubscribeAsync(subscription);
            }
            else
            {
                await _eventPublisher.PublishNewsletterUnsubscribeAsync(subscription);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual async Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
        {
            if (newsLetterSubscription == null)
            {
                throw new ArgumentNullException(nameof(newsLetterSubscription));
            }

            //Handle e-mail
            newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

            //Persist
            await _subscriptionRepository.InsertAsync(newsLetterSubscription);

            //Publish event
            await _eventPublisher.EntityInsertedAsync(newsLetterSubscription);

            //Publish the subscription event 
            if (newsLetterSubscription.Active)
                await PublishSubscriptionEventAsync(newsLetterSubscription, true, publishSubscriptionEvents);
        }

        /// <summary>
        /// Updates a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual async Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
        {
            if (newsLetterSubscription == null)
            {
                throw new ArgumentNullException(nameof(newsLetterSubscription));
            }

            //Handle e-mail
            newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

            //Get original subscription record
            var originalSubscription = await _subscriptionRepository.LoadOriginalCopyAsync(newsLetterSubscription);

            //Persist
            await _subscriptionRepository.UpdateAsync(newsLetterSubscription);

            //Publish event
            await _eventPublisher.EntityUpdatedAsync(newsLetterSubscription);

            //Publish the subscription event 
            if ((originalSubscription.Active == false && newsLetterSubscription.Active) ||
                (newsLetterSubscription.Active && originalSubscription.Email != newsLetterSubscription.Email))
            {
                //If the previous entry was false, but this one is true, publish a subscribe.
                await PublishSubscriptionEventAsync(newsLetterSubscription, true, publishSubscriptionEvents);
            }

            if (originalSubscription.Active && newsLetterSubscription.Active &&
                originalSubscription.Email != newsLetterSubscription.Email)
            {
                //If the two emails are different publish an unsubscribe.
                await PublishSubscriptionEventAsync(originalSubscription, false, publishSubscriptionEvents);
            }

            if (originalSubscription.Active && !newsLetterSubscription.Active)
                //If the previous entry was true, but this one is false
                await PublishSubscriptionEventAsync(originalSubscription, false, publishSubscriptionEvents);
        }

        /// <summary>
        /// Deletes a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual async Task DeleteNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
        {
            if (newsLetterSubscription == null)
                throw new ArgumentNullException(nameof(newsLetterSubscription));

            await _subscriptionRepository.DeleteAsync(newsLetterSubscription);

            //event notification
            await _eventPublisher.EntityDeletedAsync(newsLetterSubscription);

            //Publish the unsubscribe event 
            await PublishSubscriptionEventAsync(newsLetterSubscription, false, publishSubscriptionEvents);
        }

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription identifier
        /// </summary>
        /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByIdAsync(int newsLetterSubscriptionId)
        {
            return await _subscriptionRepository.GetByIdAsync(newsLetterSubscriptionId, cache => default);
        }

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription GUID
        /// </summary>
        /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByGuidAsync(Guid newsLetterSubscriptionGuid)
        {
            if (newsLetterSubscriptionGuid == Guid.Empty) return null;

            var newsLetterSubscriptions = from nls in _subscriptionRepository.Table
                                          where nls.NewsLetterSubscriptionGuid == newsLetterSubscriptionGuid
                                          orderby nls.Id
                                          select nls;

            return await newsLetterSubscriptions.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a newsletter subscription by email and store ID
        /// </summary>
        /// <param name="email">The newsletter subscription email</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByEmailAndStoreIdAsync(string email, int storeId)
        {
            if (!CommonHelper.IsValidEmail(email))
                return null;

            email = email.Trim();

            var newsLetterSubscriptions = from nls in _subscriptionRepository.Table
                                          where nls.Email == email && nls.StoreId == storeId
                                          orderby nls.Id
                                          select nls;

            return await newsLetterSubscriptions.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the newsletter subscription list
        /// </summary>
        /// <param name="email">Email to search or string. Empty to load all records.</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="storeId">Store identifier. 0 to load all records.</param>
        /// <param name="userRoleId">User role identifier. Used to filter subscribers by user role. 0 to load all records.</param>
        /// <param name="isActive">Value indicating whether subscriber record should be active or not; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>NewsLetterSubscription entities</returns>
        public virtual async Task<IPagedList<NewsLetterSubscription>> GetAllNewsLetterSubscriptionsAsync(string email = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int storeId = 0, bool? isActive = null, int userRoleId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (userRoleId == 0)
            {
                //do not filter by user role
                var subscriptions = await _subscriptionRepository.GetAllPagedAsync(query =>
                {
                    if (!string.IsNullOrEmpty(email))
                        query = query.Where(nls => nls.Email.Contains(email));
                    if (createdFromUtc.HasValue)
                        query = query.Where(nls => nls.CreatedOnUtc >= createdFromUtc.Value);
                    if (createdToUtc.HasValue)
                        query = query.Where(nls => nls.CreatedOnUtc <= createdToUtc.Value);
                    if (storeId > 0)
                        query = query.Where(nls => nls.StoreId == storeId);
                    if (isActive.HasValue)
                        query = query.Where(nls => nls.Active == isActive.Value);
                    query = query.OrderBy(nls => nls.Email);

                    return query;
                }, pageIndex, pageSize);

                return subscriptions;
            }

            //filter by user role
            var guestRole = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.GuestsRoleName);
            if (guestRole == null)
                throw new TvProgException("'Guests' role could not be loaded");

            if (guestRole.Id == userRoleId)
            {
                //guests
                var subscriptions = await _subscriptionRepository.GetAllPagedAsync(query =>
                {
                    if (!string.IsNullOrEmpty(email))
                        query = query.Where(nls => nls.Email.Contains(email));
                    if (createdFromUtc.HasValue)
                        query = query.Where(nls => nls.CreatedOnUtc >= createdFromUtc.Value);
                    if (createdToUtc.HasValue)
                        query = query.Where(nls => nls.CreatedOnUtc <= createdToUtc.Value);
                    if (storeId > 0)
                        query = query.Where(nls => nls.StoreId == storeId);
                    if (isActive.HasValue)
                        query = query.Where(nls => nls.Active == isActive.Value);
                    query = query.Where(nls => !_userRepository.Table.Any(c => c.Email == nls.Email));
                    query = query.OrderBy(nls => nls.Email);

                    return query;
                }, pageIndex, pageSize);

                return subscriptions;
            }
            else
            {
                var subscriptions = await _subscriptionRepository.GetAllPagedAsync(query =>
                {
                    //other user roles (not guests)
                    var joindQuery = query.Join(_userRepository.Table,
                        nls => nls.Email,
                        c => c.Email,
                        (nls, c) => new { NewsletterSubscribers = nls, User = c });

                    joindQuery = joindQuery.Where(x => _userUserRoleMappingRepository.Table.Any(ccrm =>
                        ccrm.UserId == x.User.Id && ccrm.UserRoleId == userRoleId));

                    if (!string.IsNullOrEmpty(email))
                        joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.Email.Contains(email));
                    if (createdFromUtc.HasValue)
                        joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.CreatedOnUtc >= createdFromUtc.Value);
                    if (createdToUtc.HasValue)
                        joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.CreatedOnUtc <= createdToUtc.Value);
                    if (storeId > 0)
                        joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.StoreId == storeId);
                    if (isActive.HasValue)
                        joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.Active == isActive.Value);

                    joindQuery = joindQuery.OrderBy(x => x.NewsletterSubscribers.Email);

                    return joindQuery.Select(x => x.NewsletterSubscribers);
                }, pageIndex, pageSize);

                return subscriptions;
            }
        }

        #endregion
    }
}