using TasksEverywhere.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasksEverywhere.Extensions.Extensions;
using System.Web;
using TasksEverywhere.Api.Dashboard.Exceptions;

namespace TasksEverywhere.Api.Dashboard.Extensions
{
    public static class Extensions
    {
        public static string GetSessionKey(this HttpContext context, Account account)
        {
            var sessionKey = string.Empty;
            if (context.Cache["SessionKey_" + account.AccountID] != null)
            {
                if (context.IsSessionKeyValid(account))
                    sessionKey = context.Cache["SessionKey_" + account.AccountID].ToString();
                else throw new InvalidSessionKeyException("Invalid session key for account : " + account.Username);
            }
            else
            {
                sessionKey = account.Username + "-" + Guid.NewGuid() + "_" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                context.Cache["SessionKey_" + account.AccountID] = sessionKey;
                context.AddAccount(sessionKey, account);
            }
            return sessionKey;
        }

        public static void RemoveSessionKey(this HttpContext context, Account account)
        {
            if (context.Cache["SessionKey_" + account.AccountID] != null)
            {
                context.RemoveAccount(account);
                context.Cache.Remove("LoginInfo_" + account.AccountID);
            }
        }

        public static void RemoveSessionKey(this HttpContext context, string accountID)
        {
            if (context.Cache["SessionKey_" + accountID] != null)
            {
                context.Cache.Remove("SessionKey_" + accountID);
            }
        }

        public static bool IsSessionKeyValid(this HttpContext context, Account account)
        {
            if (context.Cache["SessionKey_" + account.AccountID] != null)
            {
                var sessionKey = context.Cache["SessionKey_" + account.AccountID].ToString();
                var datetime = DateTime.Parse(sessionKey.Split('_')[1]);
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

        public static bool IsSessionKeyValid(this HttpContext context, string sessionKey, string accountID)
        {
            if (context.Cache["SessionKey_" + accountID] != null)
            {
                var datetime = DateTime.Parse(sessionKey.Split('_')[1]);
                if (DateTime.Now > datetime.AddHours(1))
                {
                    context.RemoveAccount(sessionKey);
                    context.RemoveSessionKey(accountID);
                    return false;
                }
                return true;
            }
            else return false;
        }

        public static bool ExistsSessionKey(this HttpContext context, string sessionKey, string accountID)
        {
            return context.Cache["SessionKey_" + accountID] != null;
        }

        public static void AddAccount(this HttpContext context, string sessionKey, Account account)
        {
            context.Cache["Account_" + sessionKey] = Newtonsoft.Json.JsonConvert.SerializeObject(account);
        }

        public static void RemoveAccount(this HttpContext context, string sessionKey)
        {
            context.Cache.Remove("Account_" + sessionKey);
        }

        public static void RemoveAccount(this HttpContext context, Account account)
        {
            context.Cache.Remove("Account_" + context.GetSessionKey(account));
        }

        public static string GetAccount(this HttpContext context, string sessionKey)
        {
            return context.Cache["Account_" + sessionKey].ToString();
        }
    }
}
