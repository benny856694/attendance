using System;

namespace huaanClient.Worker
{
    public class DeployEventArgs : EventArgs
    {
        public Exception Exception {  get; set; }
        public bool Success => this.ErrorCode == 0;
        public int ErrorCode { get; set; } = -1;
        public string Message {  get; set; }
    }
}