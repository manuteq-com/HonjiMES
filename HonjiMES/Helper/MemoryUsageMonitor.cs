using HonjiMES.Hubs;
using HonjiMES.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace HonjiMES.Helper
{
    public class MemoryUsageMonitor : IHostedService, IDisposable
    {
        static Timer _timer;
        private bool _disposed = false;
        private IHubContext<ChartHub> _hubContext;
        public MemoryUsageMonitor(IHubContext<ChartHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1800));// 1800秒執行一次
            return Task.CompletedTask;
        }

        private int execCount = 0;

        public void DoWork(object state)
        {
            //利用 Interlocked 計數防止重複執行
            Interlocked.Increment(ref execCount);
            if (execCount == 1)
            {
                try
                {
                    var message = new HubMessage();
                    message.clientuniqueid = "00123";
                    message.type = "AlertMessage";
                    message.message = DateTime.Now.ToString();
                    message.date = DateTime.Now;
                    _hubContext.Clients.All.SendAsync("MessageReceived", message);
                }
                catch (Exception ex)
                {

                }
            }
            Interlocked.Decrement(ref execCount);
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            //調整Timer為永不觸發，停用定期排程
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer?.Dispose();
            }

            _disposed = true;
        }
    }
}
