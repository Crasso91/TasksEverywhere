using ICeScheduler.DataLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICeScheduler.Extensions.Extensions;
using ICeScheduler.Dashboard.Exceptions;

namespace ICeScheduler.Dashboard.Extensions
{
    public static class Extensions
    {
        public static string GetSessionKey(this HttpContext context, Account account)
        {
            var sessionKey = string.Empty;
            if (context.Session.Keys.FirstOrDefault(x => x == "SessionKey_" + account.AccountID) != null)
            {
                if (context.IsSessionKeyValid(account))
                    sessionKey = context.Session.GetString("SessionKey_" + account.AccountID);
                else throw new InvalidSessionKeyException("Invalid session key for account : " + account.Username);
            }
            else
            {
                sessionKey = account.Username + "-" + Guid.NewGuid();
                context.Session.SetString("SessionKey_" + account.AccountID, sessionKey + "#" + DateTime.Now.ToString());
                context.AddAccount(sessionKey, account);
            }
            return sessionKey;
        }

        public static void RemoveSessionKey(this HttpContext context, Account account)
        {
            if (context.Session.Keys.FirstOrDefault(x => x == "SessionKey_" + account.AccountID) != null)
            {
                context.RemoveAccount(account);
                context.Session.Remove("LoginInfo_" + account.AccountID);
            }
        }

        public static void RemoveSessionKey(this HttpContext context, string sessionKey)
        {
            if (context.Session.Keys.FirstOrDefault(x => x == sessionKey) != null)
            {
                context.Session.Remove(sessionKey);
            }
        }

        public static bool IsSessionKeyValid(this HttpContext context, Account account)
        {
            if (context.Session.Keys.FirstOrDefault(x => x == "SessionKey_" + account.AccountID) != null)
            {
                var sessionKey = context.Session.GetString("SessionKey_" + account.AccountID);
                var datetime = DateTime.Parse(sessionKey.Split('#')[1]);
                if (DateTime.Now > datetime.AddHours(2))
                {
                    context.RemoveAccount(account);
                    context.RemoveSessionKey(account);
                    return false;
                }
                return true;
            }
            else return false;
        }

        public static bool IsSessionKeyValid(this HttpContext context, string sessionKey)
        {
            if (context.Session.Keys.FirstOrDefault(x => x == sessionKey) != null)
            {
                var datetime = DateTime.Parse(sessionKey.Split('#')[1]);
                if (DateTime.Now > datetime.AddHours(2))
                {
                    context.RemoveAccount(sessionKey);
                    context.RemoveSessionKey(sessionKey);
                    return false;
                }
                return true;
            }
            else return false;
        }

        public static bool ExistsSessionKey(this HttpContext context, string sessionKey)
        {
            return context.Session.Keys.FirstOrDefault(x => x == sessionKey) != null;
        }

        public static void AddAccount(this HttpContext context, string sessionKey, Account account)
        {
            context.Session.SetString("Account_" + sessionKey, Newtonsoft.Json.JsonConvert.SerializeObject(account));
        }

        public static void RemoveAccount(this HttpContext context, string sessionKey)
        {
            context.Session.Remove("Account_" + sessionKey);
        }

        public static void RemoveAccount(this HttpContext context, Account account)
        {
            context.Session.Remove("Account_" + context.GetSessionKey(account));
        }

        public static string GetAccount(this HttpContext context, string sessionKey)
        {
            return context.Session.GetString("Account_" + sessionKey);
        }
    }
}
