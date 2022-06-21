using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace huaanClient.Api
{
    public class Client : IDisposable
    {
        public string IP { get; private set; }
        public int Port { get; private set; } = 8000;

        public event EventHandler<ResponseCaptureRecord> OnRecordReceived;

        private HttpClient _client;
        private bool disposedValue;
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Client(string ip, int port = 8000)
        {
            IP = ip;
            Port = port;
            BuildClient();
        }

        private void BuildClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri($"http://{IP}:{Port}/");
        }


        public void QueryCaptureRecord(int pageSize, int maxRecordCount, DateTime from, DateTime to, bool includeRegimage, bool includeFaceImage, CancellationToken token)
        {
            if (from >= to) throw new ArgumentException("from must be smaller than to");

            var count = 0;
            var req = new RequestCaptureRecord()
            {
                page_no = 0,
                page_size = pageSize,
                time_start = from.Subtract(TimeSpan.FromHours(1)).ToUniversalTime().ToUnixTimestamp(), //把时间推前1小时，防止记录不全
                time_end = to.ToUniversalTime().ToUnixTimestamp(),
                reg_image_flag = includeRegimage ? 1 : 0,
                face_image_flag = includeFaceImage ? 1 : 0
            };
            while (!token.IsCancellationRequested)
            {
                req.page_no++;

                var policy = HttpPolicyExtensions
                                .HandleTransientHttpError()
                                    .WaitAndRetryAsync(
                                        4,
                                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                        (ex, t) => Logger.Trace(ex.Exception, "request capture data exception")
                                      );
                
                var res = policy.ExecuteAndCaptureAsync(async ct => {
                    Logger.Trace($"send request: {req.ToString()}");
                    return await _client.PostAsJsonAsync("", req, ct);
                }, token).Result;
                
                if (res.Outcome == OutcomeType.Successful)
                {
                    try
                    {
                        res.Result.EnsureSuccessStatusCode();
                        var result = res.Result.Content.ReadAsAsync<ResponseCaptureRecord>().Result;
                        OnRecordReceived?.Invoke(this, result);
                        count += result.count;
                        if (result.records == null && result.records?.Length == 0) break;
                        if (result.count < req.page_size) break;
                        if (count >= maxRecordCount) break;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "handle capture data exception", req.ToString());
                        break;
                    }
                }
                else
                {
                    if (res.FinalException is TaskCanceledException)
                    {
                        Logger.Info("request capture data cancelled");
                    }
                    else
                    {
                        Logger.Error(res.FinalException, "Request capture data failed");
                    }
                    break;
                }
            }
           
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _client?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _client = null;
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
