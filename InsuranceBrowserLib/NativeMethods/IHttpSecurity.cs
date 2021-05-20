using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace InsuranceBrowserLib.NativeMethods
{
    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface UCOMIServiceProvider
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryService(
            [In] ref Guid guidService,
            [In] ref Guid riid,
            [Out] out IntPtr ppvObject);
    }
    [ComImport()]
    [ComVisible(true)]
    [Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWindowForBindingUI
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetWindow(
            [In] ref Guid rguidReason,
            [In, Out] ref IntPtr phwnd);
    }
   


    [ComImport, ComVisible(true)]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryService(
            [In] ref Guid guidService,
            [In] ref Guid riid,
            [Out] out IntPtr ppvObject);
        //This does not work i.e.-> ppvObject = (INewWindowManager)this
        //[Out, MarshalAs(UnmanagedType.Interface)] out object ppvObject);
    }

    [ComImport(), ComVisible(true),
    Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IHttpSecurity
    {
        //IWindowForBindingUI
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetWindow(
            [In] ref Guid rguidReason,
            [In, Out] ref IntPtr phwnd);

        //IHttpSecurity
        //http://msdn.microsoft.com/library/default.asp?url=/workshop/networking/moniker/reference/ifaces/ihttpsecurity/onsecurityproblem.asp?frame=true
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnSecurityProblem(
            [In, MarshalAs(UnmanagedType.U4)] uint dwProblem);
        //dwProblem one of wininet error Messages
        //http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wininet/wininet/wininet_errors.asp?frame=true
    }

}
