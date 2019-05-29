using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TasksEverywhere.Utilities.Services
{
    public class TaskManager
    {
        public static CancellationTokenSource RunTask(Action<CancellationTokenSource> _startWith, Action<Task, CancellationTokenSource> _continueWith = null)
        {
            var canceller =  new CancellationTokenSource();
            var task = Task.Factory.StartNew(() =>
            {
                Task.Run(() => CheckCancellationToken(canceller.Token));
                _startWith(canceller);
            }, canceller.Token);
            if(_continueWith != null)
                task.ContinueWith((x) => 
                {
                    Task.Run(() => CheckCancellationToken(canceller.Token));
                    _continueWith(x, canceller);
                });
            return canceller;
        }

        public static void CheckCancellationToken(CancellationToken token)
        {
            if (token != null && token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
