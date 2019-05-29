using TasksEverywhere.Api.Dashboard.Models;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.HttpUtilities.HttpModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace TasksEverywhere.Api.Dashboard.Services
{
    public class GitWebhookManager
    {
        public enum GitRequestType
        {
            Undefined,
            Branch,
            Push
        }

        public static GitRequestType GetGitRequestType(HttpRequestMessage request)
        {
            IEnumerable<string> keys = null;
            if (!request.Headers.TryGetValues("x-gogs-event", out keys))
                return GitRequestType.Undefined;

            switch (keys.First().Trim())
            {
                case "create": return GitRequestType.Branch;
                case "push": return GitRequestType.Push;
                default: return GitRequestType.Undefined;
            }
        }

        public static SocketMessage GetMessageFor(Branch branch)
        {
            return new SocketMessage {
                Title = "Git - [BRANCH] repository : " + branch.Repository.FullName,
                DataType = branch.GetType(),
                Data = branch,
                IsImportant = false,
            };
        }

        public static SocketMessage GetMessageFor(Push push)
        {
            return new SocketMessage
            {
                Title = "Git - [PUSH] Repository : " + push.Repository.FullName,
                DataType = push.GetType(),
                Data = push,
                IsImportant = false,
            };
        }
    }
}