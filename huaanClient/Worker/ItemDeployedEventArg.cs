using System;

namespace huaanClient.Worker
{
    public class ItemDeployedEventArgs : EventArgs
    {
        public bool Success => this.ErrorCode == 0;
        public int ErrorCode;
    }
}