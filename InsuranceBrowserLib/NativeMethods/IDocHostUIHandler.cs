using System;
using System.Windows.Forms;
using mshtml;
using SHDocVw;
using System.Runtime.InteropServices;

namespace NativeMethods
{
    //public enum DOCHOSTUITYPE
    //{
    //    DOCHOSTUITYPE_BROWSE = 0,
    //    DOCHOSTUITYPE_AUTHOR = 1
    //}

    //public enum DOCHOSTUIDBLCLK
    //{
    //    DOCHOSTUIDBLCLK_DEFAULT = 0,
    //    DOCHOSTUIDBLCLK_SHOWPROPERTIES = 1,
    //    DOCHOSTUIDBLCLK_SHOWCODE = 2
    //}

    //public enum DOCHOSTUIFLAG
    //{
    //    DOCHOSTUIFLAG_DIALOG = 0x00000001,
    //    DOCHOSTUIFLAG_DISABLE_HELP_MENU = 0x00000002,
    //    DOCHOSTUIFLAG_NO3DBORDER = 0x00000004,
    //    DOCHOSTUIFLAG_SCROLL_NO = 0x00000008,
    //    DOCHOSTUIFLAG_DISABLE_SCRIPT_INACTIVE = 0x00000010,
    //    DOCHOSTUIFLAG_OPENNEWWIN = 0x00000020,
    //    DOCHOSTUIFLAG_DISABLE_OFFSCREEN = 0x00000040,
    //    DOCHOSTUIFLAG_FLAT_SCROLLBAR = 0x00000080,
    //    DOCHOSTUIFLAG_DIV_BLOCKDEFAULT = 0x00000100,
    //    DOCHOSTUIFLAG_ACTIVATE_CLIENTHIT_ONLY = 0x00000200,
    //    DOCHOSTUIFLAG_OVERRIDEBEHAVIORFACTORY = 0x00000400,
    //    DOCHOSTUIFLAG_CODEPAGELINKEDFONTS = 0x00000800,
    //    DOCHOSTUIFLAG_URL_ENCODING_DISABLE_UTF8 = 0x00001000,
    //    DOCHOSTUIFLAG_URL_ENCODING_ENABLE_UTF8 = 0x00002000,
    //    DOCHOSTUIFLAG_ENABLE_FORMS_AUTOCOMPLETE = 0x00004000,
    //    DOCHOSTUIFLAG_ENABLE_INPLACE_NAVIGATION = 0x00010000,
    //    DOCHOSTUIFLAG_IME_ENABLE_RECONVERSION = 0x00020000,
    //    DOCHOSTUIFLAG_THEME = 0x00040000,
    //    DOCHOSTUIFLAG_NOTHEME = 0x00080000,
    //    DOCHOSTUIFLAG_NOPICS = 0x00100000,
    //    DOCHOSTUIFLAG_NO3DOUTERBORDER = 0x00200000,
    //    DOCHOSTUIFLAG_DELEGATESIDOFDISPATCH = 0x00400000
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct DOCHOSTUIINFO
    //{
    //    [MarshalAs(UnmanagedType.U4)]
    //    public uint cbSize;
    //    [MarshalAs(UnmanagedType.U4)]
    //    public uint dwFlags;
    //    [MarshalAs(UnmanagedType.U4)]
    //    public uint dwDoubleClick;
    //    [MarshalAs(UnmanagedType.BStr)]
    //    public string pchHostCss;
    //    [MarshalAs(UnmanagedType.BStr)]
    //    public string pchHostNS;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct tagMSG
    //{
    //    public IntPtr hwnd;
    //    public uint message;
    //    public uint wParam;
    //    public int lParam;
    //    public uint time;
    //    public tagPOINT pt;
    //}

    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    public struct DOCHOSTUIINFO
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwFlags;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwDoubleClick;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pchHostCss;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pchHostNS;
    }

    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    public struct tagMSG
    {
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.I4)]
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        [MarshalAs(UnmanagedType.I4)]
        public int time;
        // pt was a by-value POINT structure
        [MarshalAs(UnmanagedType.I4)]
        public int pt_x;
        [MarshalAs(UnmanagedType.I4)]
        public int pt_y;
        //public tagPOINT pt;
    }

    [ComVisible(true), ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("bd3f23c0-d43e-11cf-893b-00aa00bdce1a")]
    public interface IDocHostUIHandler
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowContextMenu(
            [In, MarshalAs(UnmanagedType.U4)] uint dwID,
            [In, MarshalAs(UnmanagedType.Struct)] ref tagPOINT pt,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pcmdtReserved,
            [In, MarshalAs(UnmanagedType.IDispatch)] object pdispReserved);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetHostInfo([In, Out, MarshalAs(UnmanagedType.Struct)] ref DOCHOSTUIINFO info);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowUI(
            [In, MarshalAs(UnmanagedType.I4)] int dwID,
            [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceActiveObject activeObject,
            [In, MarshalAs(UnmanagedType.Interface)] IOleCommandTarget commandTarget,
            [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceFrame frame,
            [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceUIWindow doc);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int HideUI();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int UpdateUI();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnableModeless(
            [In, MarshalAs(UnmanagedType.Bool)] bool fEnable);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnDocWindowActivate(
            [In, MarshalAs(UnmanagedType.Bool)] bool fActivate);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnFrameWindowActivate(
            [In, MarshalAs(UnmanagedType.Bool)] bool fActivate);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ResizeBorder(
            [In, MarshalAs(UnmanagedType.Struct)] ref tagRECT rect,
            [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceUIWindow doc,
            [In, MarshalAs(UnmanagedType.Bool)] bool fFrameWindow);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int TranslateAccelerator(
            [In, MarshalAs(UnmanagedType.Struct)] ref tagMSG msg,
            [In] ref Guid group,
            [In, MarshalAs(UnmanagedType.U4)] uint nCmdID);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetOptionKeyPath(
            //out IntPtr pbstrKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out String pbstrKey,
            //[Out, MarshalAs(UnmanagedType.LPArray)] String[] pbstrKey,
            [In, MarshalAs(UnmanagedType.U4)] uint dw);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetDropTarget(
            [In, MarshalAs(UnmanagedType.Interface)] IDropTarget pDropTarget,
            [Out, MarshalAs(UnmanagedType.Interface)] out IDropTarget ppDropTarget);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetExternal(
            [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppDispatch);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int TranslateUrl(
            [In, MarshalAs(UnmanagedType.U4)] uint dwTranslate,
            [In, MarshalAs(UnmanagedType.LPWStr)] string strURLIn,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pstrURLOut);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int FilterDataObject(
            [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IDataObject pDO,
            [Out, MarshalAs(UnmanagedType.Interface)] out System.Runtime.InteropServices.ComTypes.IDataObject ppDORet);
    }

    [ComImport(),
    GuidAttribute("3050f6d0-98b5-11cf-bb82-00aa00bdce0b")]
    public interface IDocHostUIHandler2 : IDocHostUIHandler
    {
        void GetOverrideKeyPath([MarshalAs(UnmanagedType.BStr)] ref string pchKey, uint dw);
    }


    [ComVisible(true), ComImport(), Guid("00000115-0000-0000-C000-000000000046"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleInPlaceUIWindow
    {
        //IOleWindow
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetWindow([In, Out] ref IntPtr phwnd);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ContextSensitiveHelp([In, MarshalAs(UnmanagedType.Bool)] bool fEnterMode);

        //IOleInPlaceUIWindow
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetBorder([In, Out, MarshalAs(UnmanagedType.Struct)] ref tagRECT lprectBorder);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int RequestBorderSpace([In, MarshalAs(UnmanagedType.Struct)] ref tagRECT pborderwidths);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetBorderSpace([In, MarshalAs(UnmanagedType.Struct)] ref tagRECT pborderwidths);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetActiveObject(
            [In, MarshalAs(UnmanagedType.Interface)]
                ref IOleInPlaceActiveObject pActiveObject,
            [In, MarshalAs(UnmanagedType.LPWStr)]
                string pszObjName);
    }

    //    virtual HRESULT STDMETHODCALLTYPE EnableModeless( 
    //        /* [in] */ BOOL fEnable) = 0;
    //};
    [ComVisible(true), ComImport(), Guid("00000117-0000-0000-C000-000000000046"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleInPlaceActiveObject
    {
        //IOleWindow
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetWindow([In, Out] ref IntPtr phwnd);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ContextSensitiveHelp([In, MarshalAs(UnmanagedType.Bool)] bool
            fEnterMode);

        //IOleInPlaceActiveObject
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int TranslateAccelerator(
            [In, MarshalAs(UnmanagedType.Struct)] ref tagMSG lpmsg);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnFrameWindowActivate(
            [In, MarshalAs(UnmanagedType.Bool)] bool fActivate);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnDocWindowActivate(
            [In, MarshalAs(UnmanagedType.Bool)] bool fActivate);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ResizeBorder(
            [In, MarshalAs(UnmanagedType.Struct)] ref tagRECT prcBorder,
            [In, MarshalAs(UnmanagedType.Interface)] ref IOleInPlaceUIWindow pUIWindow,
            [In, MarshalAs(UnmanagedType.Bool)] bool fFrameWindow);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnableModeless(
            [In, MarshalAs(UnmanagedType.Bool)] bool fEnable);
    }
    [ComImport(), ComVisible(true),
    Guid("B722BCCB-4E68-101B-A2BC-00AA00404770"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleCommandTarget
    {

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryStatus(
            [In] IntPtr pguidCmdGroup,
            [In, MarshalAs(UnmanagedType.U4)] uint cCmds,
            [In, Out, MarshalAs(UnmanagedType.Struct)] ref tagOLECMD prgCmds,
            //This parameter must be IntPtr, as it can be null
            [In, Out] IntPtr pCmdText);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Exec(
            //[In] ref Guid pguidCmdGroup,
            //have to be IntPtr, since null values are unacceptable
            //and null is used as default group!
            [In] IntPtr pguidCmdGroup,
            [In, MarshalAs(UnmanagedType.U4)] uint nCmdID,
            [In, MarshalAs(UnmanagedType.U4)] uint nCmdexecopt,
            [In] IntPtr pvaIn,
            [In, Out] IntPtr pvaOut);
    }
    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    public struct tagOLECMD
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint cmdID;
        [MarshalAs(UnmanagedType.U4)]
        public uint cmdf;
    }

    [ComVisible(true), ComImport(), Guid("00000116-0000-0000-C000-000000000046"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleInPlaceFrame
    {
        //IOleWindow
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetWindow([In, Out] ref IntPtr phwnd);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ContextSensitiveHelp([In, MarshalAs(UnmanagedType.Bool)] bool fEnterMode);

        //IOleInPlaceUIWindow
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetBorder(
            [Out, MarshalAs(UnmanagedType.LPStruct)] tagRECT lprectBorder);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int RequestBorderSpace([In, MarshalAs(UnmanagedType.Struct)] ref tagRECT pborderwidths);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetBorderSpace([In, MarshalAs(UnmanagedType.Struct)] ref tagRECT pborderwidths);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetActiveObject(
            [In, MarshalAs(UnmanagedType.Interface)] ref IOleInPlaceActiveObject pActiveObject,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszObjName);

        //IOleInPlaceFrame 
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int InsertMenus([In] IntPtr hmenuShared,
           [In, Out, MarshalAs(UnmanagedType.Struct)] ref object lpMenuWidths);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetMenu(
            [In] IntPtr hmenuShared,
            [In] IntPtr holemenu,
            [In] IntPtr hwndActiveObject);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int RemoveMenus([In] IntPtr hmenuShared);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetStatusText([In, MarshalAs(UnmanagedType.LPWStr)] string pszStatusText);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnableModeless([In, MarshalAs(UnmanagedType.Bool)] bool fEnable);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int TranslateAccelerator(
            [In, MarshalAs(UnmanagedType.Struct)] ref tagMSG lpmsg,
            [In, MarshalAs(UnmanagedType.U2)] short wID);
    }
}
