using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace huaanClient.Worker
{
    using Business;
    using DBUtility.SQLite;
    using Polly;
    using System.Net.Http;
    using System.Threading;

    public class DeviceAccessRuleDeployer
    {
        public string DeviceIp { get; }
        public AccessControlDeployRule[] Rules { get; }
        public AccessControlDeployItem[] Items { get; }
        public Exception LastError { get; private set; }
        public int LastErrorCode { get; set; }

        public event EventHandler<DeployEventArgs> ItemDeployedEvent;
        public event EventHandler<DeployEventArgs> RuleDeployEvent;

        private HttpClient _http;
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
                    var errCode = await DeployRulesAsync(tk);
                    var arg = new DeployEventArgs
                    {
                        ErrorCode = errCode
                    };
                    RuleDeployEvent?.Invoke(this, arg);
                }, token);
            }
            catch (Exception ex)
            {
                LastError = ex;
                Logger.Error(ex, $"Deploy Rules to {DeviceIp} exception");
                var arg = new DeployEventArgs { Exception = ex };
                RuleDeployEvent?.Invoke(this, arg);
                return;
            }

            await DeployItemsAsync(policy, token);
        }

        private async Task<int> DeployRulesAsync(CancellationToken token)
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
            return res.code;
        }

        private async Task DeployItemsAsync(Polly.Retry.AsyncRetryPolicy policy, CancellationToken token)
        {
            foreach (var item in Items)
            {
                if (item.State != DeployResult.Waiting) continue;
                if (token.IsCancellationRequested) return;

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
                        ItemDeployedEvent?.Invoke(this, new DeployEventArgs { ErrorCode = res.code });
                        if (AccessRuleDeployManager.Instance.DefaultAccess.Equals(Access.NoAccess) && req.kind == 1)//默认规则不准通行并且调度类别为1的加入到下发列表，进行删除
                        {
                            using (var conn = SQLiteHelper.GetConnection())
                            {
                                GetData.DeleteStaffFromDevice(item.id?.ToString(), (int)item.DeviceId, conn);
                            }
                        }
                    }, token);
                }
                catch (Exception ex)
                {
                    item.State = DeployResult.Failed;
                    Logger.Error(ex, $"Deploy access control item Id:{item.id} to {DeviceIp} exception");
                    LastError = ex;
                    var arg = new DeployEventArgs { Exception = ex };
                    ItemDeployedEvent?.Invoke(this, arg);
                }
            }


            _http.Dispose();
        }


    }
}
