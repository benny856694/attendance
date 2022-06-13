﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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


        public async Task QueryCaptureRecordAsync(int pageSize, DateTime from, DateTime to, bool includeRegimage, bool includeFaceImage)
        {
            if (from >= to) throw new ArgumentException("from must be smaller than to");

            
            var req = new RequestCaptureRecord()
            {
                page_no = 0,
                page_size = pageSize,
                time_start = from.ToUnixTimestamp(),
                time_end = to.ToUnixTimestamp(),
                reg_image_flag = includeRegimage ? 1 : 0,
                face_image_flag = includeFaceImage ? 1 : 0
            };
            while (true)
            {
                req.page_no++;
                var response = await _client.PostAsJsonAsync("", req);
                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsAsync<ResponseCaptureRecord>();
                OnRecordReceived?.Invoke(this, res);

                if (res.records?.Length == 0) break;
                if (res.count < req.page_size) break;
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
