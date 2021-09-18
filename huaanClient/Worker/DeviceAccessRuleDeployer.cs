using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace huaanClient.Worker
{
    using Business;
    using System.Net.Http;

    public class DeviceAccessRuleDeployer
    {
        public string DeviceIp { get; }
        public AccessControlDeployRule[] Rules { get; }
        public AccessControlDeployItem[] Items { get; }

        private HttpClient _http;

        public DeviceAccessRuleDeployer(string deviceIp, AccessControlDeployRule[] rules, AccessControlDeployItem[] items)
        {
            DeviceIp = deviceIp;
            Rules = rules;
            Items = items;

            _http = new HttpClient();
            _http.BaseAddress = new Uri($"http://{deviceIp}:8000");
        }

        public async Task DeployAsync()
        {
            await DeployRulesAsync();
            await DeployItemsAsync();
        }

        private async Task DeployRulesAsync()
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
                Debug.WriteLine($"failed to deploy access rules to {DeviceIp}, reply:{res.reply}");
            }
        }

        private async Task DeployItemsAsync()
        {
            foreach (var item in Items)
            {
                var req = new
                {
                    cmd = "upload person",
                    id = item.id,
                    kind = item.kind
                };
                var resp = await _http.PostAsJsonAsync("", req);
                var res = await resp.Content.ReadAsAsync<Api.Response>();
                if (res.code != 0)
                {
                    Debug.WriteLine($"failed to deploy access rule item: {item.id} to {DeviceIp}, reply:{res.reply}");
                }
                await Task.Delay(500);
            }

            _http.Dispose();
        }

        
    }
}
