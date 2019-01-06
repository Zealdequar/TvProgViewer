﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.TVProgApp.Controllers
{
    internal class TvProgController : BaseController
    {
        internal static Task<ProviderType[]> GetProviderTypeAsyncList()
        {
            return Task<ProviderType[]>.Factory.StartNew(() => { return TvProgService.GetProviderTypeList(); });
        }

        internal static Task<SystemChannel[]> GetSystemChannelAsyncList(int tvProgProviderID)
        {
            return TvProgService.GetSystemChannelListAsync(tvProgProviderID);
        
        }

        internal static Task<SystemProgramme[]> GetSystemProgrammesAtNowAsycList(int typeProgID, DateTimeOffset dateTimeOffset)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.GetSystemProgrammeList(typeProgID, dateTimeOffset, 1); });
        }

        internal static Task<SystemProgramme[]> GetSystemProgrammesAtNextAsycList(int typeProgID, DateTimeOffset dateTimeOffset)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.GetSystemProgrammeList(typeProgID, dateTimeOffset, 2); });
        }

        internal static ProgPeriod GetSystemProgrammePeriod(int typeProgID)
        {
            return TvProgService.GetSystemProgrammePeriod(typeProgID);
        }

        internal static Task<SystemProgramme[]> GetSystemProgrammesOfDayAsyncList(int typeProgID, int cid, 
            DateTime tsStart, DateTime tsStop)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.GetSystemProgrammeDayList(typeProgID, cid, tsStart, tsStop); });
        }

        internal static UserChannel[] GetUserChannelList(long uid, int typeProgID)
        {
            return TvProgService.GetUserChannelList(uid, typeProgID);
        }

        internal static void InsertUserChannel(int userChannelID, long uid, int typeProgID, int cid, 
            string displayName, int orderCol)
        {
            TvProgService.InsertUserChannel(userChannelID, uid, typeProgID, cid, displayName, orderCol);
        }

        internal static void DeleteUserChannel(long uid, int cid)
        {
            TvProgService.DeleteUserChannelAsync(uid, cid);
        }

        internal static Task<SystemProgramme[]> GetUserProgrammesAtNowAsyncList(
            long uid, int typeProgID, DateTimeOffset dateTimeOffset)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.GetUserProgrammeList(uid, typeProgID, dateTimeOffset, 1); });
        }

        internal static Task<SystemProgramme[]> GetUserProgrammesAtNextAsyncList(
            long uid, int typeProgID, DateTimeOffset dateTimeOffset)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.GetUserProgrammeList(uid, typeProgID, dateTimeOffset, 2); });
        }

        internal static Task<SystemProgramme[]> GetUserProgrammesOfDayAsyncList(long uid, int typeProgID, int cid,
            DateTime tsStart, DateTime tsStop)
        {
            return TvProgService.GetUserProgrammeDayListAsync(uid, typeProgID, cid, tsStart, tsStop);
        }
    }
}
