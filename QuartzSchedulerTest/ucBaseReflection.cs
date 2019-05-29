using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class ReflectionExtensione
    {
        /// <summary>
        /// Dato il nome della proprietà restituisce l'oggetto descrittivo della stessa
        /// </summary>
        /// <param name="propertyName">Nome della proprietà</param>
        /// <returns>Informazioni della proprietà</returns>
        public static PropertyInfo GetProperty(this object _in, string propertyName)
        {
            return _in.GetType().GetProperties().First(x => x.Name == propertyName);
        }

        /// <summary>
        /// Dato il nome della proprietà restituisce il valore della stessa
        /// </summary>
        /// <typeparam name="T">Tipo della proprietà</typeparam>
        /// <param name="propertyName">Nome della proprietà</param>
        /// <returns>Valore della proprietà del tipo designato in input</returns>
        public static T GetPropertyValue<T>(this object _in, string propertyName)
        {
            return (T)_in.GetProperty(propertyName).GetValue(_in);
        }

        /// <summary>
        /// Dato il nome della proprietà restituisce il valore della stessa
        /// </summary>
        /// <param name="propertyName">Nome della proprietà</param>
        /// <returns>Valore della proprietà</returns>
        public static object GetPropertyValue(this object _in, string propertyName)
        {
            return _in.GetProperty(propertyName).GetValue(_in);
        }

        /// <summary>
        /// Dato il nome della proprietà restituisce il tipo della stessa
        /// </summary>
        /// <param name="propertyName">Nome della proprietà</param>
        /// <returns>Tipo della proprietà</returns>
        public static Type GetPropertyType(this object _in, string propertyName)
        {
            return _in.GetProperty(propertyName).DeclaringType;
        }

        /// <summary>
        /// Dato il nome della proprietà setta il valore della stessa con il valore in input
        /// </summary>
        /// <param name="propertyName">Nome della proprietà</param>
        /// <param name="propertyValue">Valore della proprietà</param>
        public static void SetPropertyValue(this object _in, string propertyName, object propertyValue)
        {
            _in.GetProperty(propertyName).SetValue(_in, propertyValue);
        }

        /// <summary>
        /// Dato il nome del metodo in input viene eseguito passando i parametri in input e ritornato il suo risultato
        /// </summary>
        /// <param name="methodName">Nome del metodo</param>
        /// <param name="args">Argomenti del metodo (possono essere null)</param>
        /// <returns>L'oggetto ritornato dal metodo</returns>
        public static object RunMethodWithReturn(this object _in, string methodName, object[] args = null)
        {
            return _in.GetType()
                .GetMethod(methodName)
                .Invoke(_in, args);
        }

        /// <summary>
        /// Dato il nome del methodo in input viene eseguito passando i paramentri in input, non viene ritornato il risultato
        /// </summary>
        /// <param name="methodName">Nome del metodo</param>
        /// <param name="args">Argomenti del metodo (possono essere null)</param>
        public static void RunMethod(this object _in, string methodName, object[] args = null)
        {
            _in.GetType()
                .GetMethod(methodName)
                .Invoke(_in, args);
        }

        /// <summary>
        /// Definita la struttura del metodo tramite l'oggetto in input viene eseguito il metodo richiesto.
        /// Nel caso in cui quest'ultimo restituisca un risultato le proprietà dell'oggeto in input MethodCall.ReturnType e MethodCall.ReturnObject vengono valorizzati
        /// Nel caso in cui il metodo non restituisca alcun risultato le proprietà MethodCall.Returntype e MethodCall.ReturnObject saranno valorizzate nul
        /// Nel caso in cui l'esecuzione lanci un'eccezzione le proprietà dell'oggetto in input MethodCall.ReturnType e MethodCall.ReturnObject saranno null ma sarà valorizzata la proprietà MethodCall.Exception
        /// </summary>
        /// <param name="methodCall">Oggetto che definisce il metodo</param>
        /// <returns>Oggetto che definisce il metodo con dei campi valorizzati</returns>
        public static MethodCall RunMethod(this object _in, MethodCall methodCall)
        {
            try
            {
                var invocationResult = _in.GetType()
                    .GetMethod(methodCall.Name)
                    .Invoke(_in, methodCall.Args);

                methodCall.ReturnType = invocationResult.GetType();
                methodCall.ReturnObject = invocationResult;

            }
            catch (Exception ex)
            {
                methodCall.ReturnObject = null;
                methodCall.ReturnType = null;
                methodCall.Exception = ex;
            }
            return methodCall;
        }

        /// <summary>
        /// Data la lista di oggetti per la definizione di più metodi vengono eseguiti tutti i metodi.
        /// Ritora la stessa lista in input con le proprità valorizzate secondo queste logiche:
        /// Nel caso in cui quest'ultimo restituisca un risultato le proprietà dell'oggeto in input MethodCall.ReturnType e MethodCall.ReturnObject vengono valorizzati
        /// Nel caso in cui il metodo non restituisca alcun risultato le proprietà MethodCall.Returntype e MethodCall.ReturnObject saranno valorizzate nul
        /// Nel caso in cui l'esecuzione lanci un'eccezzione le proprietà dell'oggetto in input MethodCall.ReturnType e MethodCall.ReturnObject saranno null ma sarà valorizzata la proprietà MethodCall.Exception
        /// </summary>
        /// <param name="methodsCall">Lista di oggetti che definiscono i metodi</param>
        /// <returns>Lista di oggetti che definiscono i metodi con dei campi valorizzati</returns>
        public static List<MethodCall> RunMethods(this object _in, List<MethodCall> methodsCall)
        {
            methodsCall.ForEach(x =>
            {
                try
                {
                    var invocationResult = _in.GetType()
                        .GetMethod(x.Name)
                        .Invoke(_in, x.Args);

                    x.ReturnType = invocationResult.GetType();
                    x.ReturnObject = invocationResult;

                }
                catch (Exception ex)
                {
                    x.ReturnObject = null;
                    x.ReturnType = null;
                    x.Exception = ex;
                }
            });

            return methodsCall;
        }
        /// <summary>
        /// Dato il nome del metodo in input restituisce un dictionary avente come chiave il nome dell'argomento del metodo e come valore il suo tipo
        /// </summary>
        /// <param name="methodName">Nome del metodo</param>
        /// <returns>Dictionary avente come chiave il nomde dell'argomento e come valore il tipo</returns>
        public static Dictionary<string, Type> GetMethodArgs(this object _in, string methodName)
        {
            var result = new Dictionary<string, Type>();

            var paramsInfo = _in.GetType()
                .GetMethod(methodName)
                .GetParameters();

            foreach(var _param in paramsInfo)
            {
                result.Add(_param.Name, _param.ParameterType);
            }

            return result;
        }
    }

    public class MethodCall
    {
        public string Name { get; set; }
        public object[] Args { get; set; }
        public Type ReturnType { get; set; }
        public object ReturnObject { get; set; }
        public Exception Exception { get; set; }
    }
}
