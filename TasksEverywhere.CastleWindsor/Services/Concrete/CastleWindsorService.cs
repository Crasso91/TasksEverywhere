using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TasksEverywhere.CastleWindsor.Service.Abstract;
using TasksEverywhere.CastleWindsor.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.CastleWindsor.Service.Concrete
{
    public class CastleWindsorService
    {
        private static WindsorContainer _instance = null;

        public static WindsorContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WindsorContainer();
                    LoadServices();
                }
                return _instance;
            }
        }

        public static T Resolve<T>()
        {
            return Instance.Resolve<T>();
        }


        public static void Release(object obj)
        {
            Instance.Release(obj);
        }

        public static void Init()
        {
            if (_instance == null)
            {
                _instance = new WindsorContainer();
                
                LoadServices();
            }
        }

        private static void LoadServices()
        {
            //var _path = !String.IsNullOrEmpty(path) ? path : System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); //System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var _path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            using (var eventLog = new EventLog("Application"))
            {

                eventLog.Source = "Application";
                eventLog.WriteEntry("Bin path" + _path, EventLogEntryType.Information);
            }

            if (_path.ToLower().Contains("iis") || _path.ToLower().Contains("inetsrv")) _path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");

            using (var eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("Bin path" + _path, EventLogEntryType.Information);
            }

            _instance.Register(
                Classes.FromAssemblyInDirectory(new Castle.MicroKernel.Registration.AssemblyFilter(_path))
                    .BasedOn(typeof(ISingletonService))
                    .LifestyleSingleton()
            );
            _instance.Register(
                Classes.FromAssemblyInDirectory(new Castle.MicroKernel.Registration.AssemblyFilter(_path))
                    .BasedOn(typeof(ITransientService))
                    .LifestyleTransient()
                );
            _instance.Register(
                Classes.FromAssemblyInDirectory(new Castle.MicroKernel.Registration.AssemblyFilter(_path))
                    .BasedOn(typeof(IHttpComunicationService))
                    .LifestyleTransient()
                );
        }

        /// <summary>
        /// Recupera o instanzia l'oggetto del tipo passato in input
        /// </summary>
        /// <param name="typename">Tipo da risolvere</param>
        /// <returns></returns>
        public static ISingletonService Resolve(string typename, dynamic _params)
        {

            var _type = CastleWindsorService.GetTypeOf(typename);
            return (ISingletonService)CastleWindsorService.Instance.Resolve(_type, _params);
        }

        /// <summary>
        /// Recupera il nome completo del tipo in base al nome parziale
        /// </summary>
        /// <param name="typename">come parziale del tipo</param>
        /// <returns></returns>
        protected static string GetCompleteNameOf(string typename)
        {
            for (var i = 0; i < Instance.Kernel.GraphNodes.Length; i++)
            {
                Castle.Core.ComponentModel component = (Castle.Core.ComponentModel)Instance.Kernel.GraphNodes[i];
                if (component.ComponentName.Name.Contains(typename)) return component.ComponentName.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Recupera il Type in base al nome parziale
        /// </summary>
        /// <param name="typename">come parziale del tipo</param>
        /// <returns></returns>
        protected static Type GetTypeOf(string typename)
        {
            for (var i = 0; i < Instance.Kernel.GraphNodes.Length; i++)
            {
                Castle.Core.ComponentModel component = (Castle.Core.ComponentModel)Instance.Kernel.GraphNodes[i];
                if (component.ComponentName.Name.Contains(typename)) return component.Implementation;
            }
            return null;
        }
    }
}
