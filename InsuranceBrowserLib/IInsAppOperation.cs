using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsuranceBrowserLib
{
    public interface IInsAppOperation
    {
        void GotoInsCenter();
        void GotoTastCenter(string msg);
        void GotoPolicyPage(string url);
    }
}
