using TasksEverywhere.WebApi.Controllers;
using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.DataLayer.Context.Concrete;
using TasksEverywhere.DataLayer.Enumerators;
using TasksEverywhere.DataLayer.Models;
using TasksEverywhere.Quartz.Context.Jobs.Concrete;
using TasksEverywhere.Quartz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace TasksEverywhere.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            

        }
    }
}
