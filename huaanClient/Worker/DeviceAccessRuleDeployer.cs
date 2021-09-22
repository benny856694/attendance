using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace huaanClient.Worker
{
    using Business;
    using Polly;
    using System.Net.Http;
    using System.Threading;

    public class DeviceAccessRuleDeployer
    {
        public string DeviceIp { get; }
        public AccessControlDeployRule[] Rules { get; }
        public AccessControlDeployItem[] Items { get; }
        public Exception LastError {  get; private set; }
        public int LastErrorCode { get;set;  }

        public event EventHandler<ItemDeployedEventArgs> ItemDeployedEvent;

        private HttpClient _http;
        private static NLog.Logger Logger= NLog.LogManager.GetCurrentClassLogger();

        public DeviceAccessRuleDeployer(string deviceIp, AccessControlDeployRule[] rules, AccessControlDeployItem[] items)
        {
            DeviceIp = deviceIp;
            Rules = rules;
            Items = items;

            _http = new HttpClient();
            _http.BaseAddress = new Uri($"http://{deviceIp}:8000");
        }

        public async Task DeployAsync(CancellationToken token)
        {
            var policy = Policy.Handle<Exception>().WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(200)
                );
            LastError = null;
            LastErrorCode = 0;
            try
            {
                await policy.ExecuteAsync(async tk =>
                {
                   await DeployRulesAsync(tk);
                }, token);
            }
            catch (Exception ex)
            {
                LastError = ex;
                Logger.Error(ex, "Deploy Access rules failed, items deployment cancelled");
                return;
            }
            
            await DeployItemsAsync(policy, token);
        }

        private async Task DeployRulesAsync(CancellationToken token)
        {
            var req = new
            {
                cmd = "update schedule params",
                kinds = Rules,
            };

            var resp = await _http.PostAsJsonAsync("", req);
            var res = await resp.Content.ReadAsAsync<Api.Response>();
            if (res.code != 0)
            {
                LastErrorCode = res.code;
            }
        }

        private async Task DeployItemsAsync(Polly.Retry.AsyncRetryPolicy policy, CancellationToken token)
        {
            foreach (var item in Items)
            {
                if (item.State != DeployResult.Waiting) continue;

                if (!token.IsCancellationRequested)
                {
                    var req = new
                    {
                        cmd = "upload person",
                        id = item.id,
                        kind = item.kind
                    };

                    try
                    {
                        await policy.ExecuteAsync(async tk => 
                        {
                            var resp = await _http.PostAsJsonAsync("", req);
                            var res = await resp.Content.ReadAsAsync<Api.Response>();
                            item.ErrorCode = res.code;
                            item.State = res.code == 0 ? DeployResult.Succeed : DeployResult.Failed;
                            Debug.WriteLine($"deploy id: {item.id} kind:{item.kind} to {item.DeviceId}, result: {res.code}");
                            ItemDeployedEvent?.Invoke(this, new ItemDeployedEventArgs { ErrorCode = res.code });
                        }, token);
                    }
                    catch (Exception ex)
                    {
                        LastError = ex;
                    }
                }
               
            }

            Debug.WriteLine("Item deploy finished");

            _http.Dispose();
        }

        
    }
}
