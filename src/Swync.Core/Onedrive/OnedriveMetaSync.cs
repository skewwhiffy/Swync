using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hangfire;

namespace Swync.Core.Onedrive
{
    public class OnedriveMetaSync
    {
        private static readonly Lazy<OnedriveMetaSync> SingletonLazy = new Lazy<OnedriveMetaSync>(() => new OnedriveMetaSync());
        
        private OnedriveMetaSync()
        {
        }

        public static readonly OnedriveMetaSync Singleton = SingletonLazy.Value;

        public void Start()
        {
            if (Running) return;
            BackgroundJob.Enqueue(() => RunAsync());
        }

        public bool Running { get; private set; }
        
        public ConcurrentQueue<Exception> Exceptions { get; } = new ConcurrentQueue<Exception>();

        public async Task RunAsync()
        {
            lock (this)
            {
                if (Running) return;
                Running = true;
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                throw new NotImplementedException("Woo hoo");
            }
            catch (Exception ex)
            {
                Exceptions.Enqueue(ex);
            }

            lock (this)
            {
                Running = false;
            }
        }
    }
}