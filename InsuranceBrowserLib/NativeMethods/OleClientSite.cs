using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NativeMethods
{
  //[ComImport,
  //Guid("00000118-0000-0000-C000-000000000046"),
  //InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
  //]

  //public interface IOleClientSite
  //{
  //  void SaveObject();
  //  void GetMoniker(uint dwAssign, uint dwWhichMoniker, ref object ppmk);
  //  void GetContainer(ref object ppContainer);
  //  void ShowObject();
  //  void OnShowWindow(bool fShow);
  //  void RequestNewObjectLayout();
  //}


    [ComImport, ComVisible(true)]
    [Guid("00000118-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleClientSite
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SaveObject();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMoniker(
            [In, MarshalAs(UnmanagedType.U4)]         uint dwAssign,
            [In, MarshalAs(UnmanagedType.U4)]         uint dwWhichMoniker,
            [Out, MarshalAs(UnmanagedType.Interface)] out IMoniker ppmk);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetContainer(
            [Out, MarshalAs(UnmanagedType.Interface)] out IOleContainer ppContainer);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowObject();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnShowWindow([In, MarshalAs(UnmanagedType.Bool)] bool fShow);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int RequestNewObjectLayout();
    }

    [ComImport(), ComVisible(true),
    Guid("0000011B-0000-0000-C000-000000000046"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleContainer
    {
        //IParseDisplayName
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ParseDisplayName(
            [In, MarshalAs(UnmanagedType.Interface)] object pbc,
            [In, MarshalAs(UnmanagedType.BStr)]      string pszDisplayName,
            [Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten,
            [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppmkOut);

        //IOleContainer
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumObjects(
            [In, MarshalAs(UnmanagedType.U4)] tagOLECONTF grfFlags,
            out IEnumUnknown ppenum);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int LockContainer(
            [In, MarshalAs(UnmanagedType.Bool)] Boolean fLock);
    }

    [ComImport, ComVisible(true)]
    [Guid("00000100-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumUnknown
    {
        [PreserveSig]
        int Next(
            [In, MarshalAs(UnmanagedType.U4)] int celt,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object rgelt,
            [Out, MarshalAs(UnmanagedType.U4)] out int pceltFetched);
        [PreserveSig]
        int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);
        void Reset();
        void Clone(out IEnumUnknown ppenum);
    }

    public enum tagOLECONTF
    {
        OLECONTF_EMBEDDINGS = 1,
        OLECONTF_LINKS = 2,
        OLECONTF_OTHERS = 4,
        OLECONTF_ONLYUSER = 8,
        OLECONTF_ONLYIFRUNNING = 16
    }
}
