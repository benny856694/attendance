using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsuranceBrowserLib
{
    public interface IWebOperation
    {
        void GoBack();
        void GoForward();
        void Stop();
        void Refresh();
        void Navigate(string url);
        void Submit();
        void SubmitForGongbao();
        void ClearCache();
        void RunScript();
        string GetString();
        string GetUrl();
        void RunScript(string path);

        bool IsChrome { get;  }
        bool IsChromePopup { get;  }
    }
}
