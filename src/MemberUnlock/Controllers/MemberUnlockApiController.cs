using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace MemberUnlock.Controllers
{
    /// <summary>
    /// Implement the MemberUnlockApiController
    /// </summary>
    [PluginController("MemberUnlock")]
    public class MemberUnlockApiController : UmbracoApiController
    {
        /// <summary>
        /// Get the lockedOut time in minutes from the AppSettings with key `memberLockedOutInMinutes`
        /// Default is 10 minutes
        /// </summary>
        private int LockedOutInMinutes
        {
            get
            {
                try
                {
                    var timeSpan = int.Parse(ConfigurationManager.AppSettings["memberLockedOutInMinutes"]);
                    return timeSpan < 0 ? 0 : timeSpan;
                }
                catch
                {
                    return 10;
                }
            }
        }

        /// <summary>
        /// Get the MemberUnlock APP Key from within the webConfig
        /// </summary>
        private Guid MemberUnlockAppKey
        {
            get
            {
                try
                {
                    return Guid.Parse(ConfigurationManager.AppSettings["memberUnlockAppKey"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        /// <summary>
        /// Get the IMemberService
        /// </summary>
        private IMemberService MemberService
        {
            get
            {
                return Services.MemberService;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public MemberUnlockApiController() { }

        /// <summary>
        /// Manual setting Member Test to locked out
        /// </summary>
        [HttpGet]
        public void SetLockedOut()
        {
            var memberService = Services.MemberService;
            var member = memberService.GetByKey(new Guid("fe41d912-39be-4514-89bb-5429d8137c26"));
            member.IsLockedOut = true;
            member.LastLockoutDate = DateTime.Now;

            memberService.Save(member);
        }

        /// <summary>
        /// Get all locked members and check if there locked out period is expired
        /// If so, then unlock the member by setting `IsLockedOut` to false and reset the failedLoginAttempts
        /// </summary>
        [HttpGet]
        public void DoUnlock(Guid appKey)
        {
            // Do security check on AppKey
            if(Guid.Equals(appKey, MemberUnlockAppKey))
            {
                // Get locked members
                var lockedMembers = MemberService.GetMembersByPropertyValue("umbracoMemberLockedOut", true);

                foreach (var member in lockedMembers)
                {
                    // Check if the locked out period is expired
                    if (LockedOutExpired(member))
                    {
                        // Update member properties
                        member.IsLockedOut = false;
                        member.FailedPasswordAttempts = 0;

                        // Save Member
                        MemberService.Save(member);

                        // Set log entry of the member that has been unlocked
                        Logger.Info<MemberUnlockApiController>(
                            String.Format("Member '{0}' has been unlocked automatically.", member.Username));
                    }
                }
            }
        }

        /// <summary>
        /// Check and determine if the locked out period of the member is expired or not
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private bool LockedOutExpired(IMember member)
        {
            var lockedOutPeriod = (DateTime.Now - member.LastLockoutDate).TotalMinutes;
            return lockedOutPeriod >= LockedOutInMinutes;
        }
    }
}