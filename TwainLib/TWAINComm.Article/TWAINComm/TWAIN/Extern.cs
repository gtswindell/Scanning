using System;
using System.Runtime.InteropServices;

namespace TWAINComm
{
    internal static class Extern
    {
        // ------ DSM DAT variants
        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DSM_Parent( [In, Out] TW_IDENTITY appIdentity, IntPtr pZero, DG dg, DAT dat, MSG msg, [In] ref IntPtr hWnd );

        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DSM_DataSource( [In, Out] TW_IDENTITY appIdentity, IntPtr pZero, DG dg, DAT dat, MSG msg, [In, Out] TW_IDENTITY dsIdentity );

        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DSM_Status( [In, Out] TW_IDENTITY appIdentity, IntPtr pZero, DG dg, DAT dat, MSG msg, [In, Out] TW_STATUS dsmStatus );


        // ------ DS DAT variants
        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DS_UserInterface( [In, Out] TW_IDENTITY appIdentity, [In] TW_IDENTITY dsIdentity, DG dg, DAT dat, MSG msg, [In] TW_USERINTERFACE ui );

        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DS_DeviceEvent( [In, Out] TW_IDENTITY appIdentity, [In] TW_IDENTITY dsIdentity, DG dg, DAT dat, MSG msg, [In] ref TW_EVENT evtmsg );

        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DS_Status( [In, Out] TW_IDENTITY appIdentity, [In] TW_IDENTITY dsIdentity, DG dg, DAT dat, MSG msg, [In, Out] TW_STATUS dsStatus );

        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DS_Capability( [In, Out] TW_IDENTITY appIdentity, [In] TW_IDENTITY dsIdentity, DG dg, DAT dat, MSG msg, [In, Out] TW_CAPABILITY cap );

        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DS_ImageInfo( [In, Out] TW_IDENTITY appIdentity, [In] TW_IDENTITY dsIdentity, DG dg, DAT dat, MSG msg, [In, Out] TW_IMAGEINFO imgInfo );

        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DS_ImageNativeXfer( [In, Out] TW_IDENTITY appIdentity, [In] TW_IDENTITY dsIdentity, DG dg, DAT dat, MSG msg, [In] ref IntPtr hDIB );

        [DllImport( "twain_32.dll", EntryPoint = "DSM_Entry" )]
        internal static extern TWRC DS_PendingXfers( [In, Out] TW_IDENTITY appIdentity, [In] TW_IDENTITY dsIdentity, DG dg, DAT dat, MSG msg, [In] ref TW_PENDINGXFERS pendXfr );


        // ------ Kernel32
        [DllImport( "kernel32.dll", SetLastError = true, ExactSpelling = true )]
        internal static extern IntPtr GlobalAlloc( GlobalAllocFlags flags, uint size );

        [DllImport( "kernel32.dll", SetLastError = true, ExactSpelling = true )]
        internal static extern IntPtr GlobalFree( IntPtr handle );

        [DllImport( "kernel32.dll", SetLastError = true, ExactSpelling = true )]
        internal static extern IntPtr GlobalLock( IntPtr handle );

        [DllImport( "kernel32.dll", SetLastError = true, ExactSpelling = true )]
        internal static extern bool GlobalUnlock( IntPtr handle );


        // ------ User32
        [DllImport( "user32.dll", ExactSpelling = true )]
        internal static extern int GetMessagePos();

        [DllImport( "user32.dll", ExactSpelling = true )]
        internal static extern int GetMessageTime();


        // ------ GDI+
        [DllImport( "gdiplus.dll", ExactSpelling = true )]
        internal static extern int GdiplusStartup( out IntPtr token, ref GdipStartupInput input, out GdipStartupOutput output );

        [DllImport( "gdiplus.dll", ExactSpelling = true )]
        internal static extern int GdiplusShutdown( IntPtr token );

        [DllImport( "gdiplus.dll", ExactSpelling = true )]
        internal static extern int GdipCreateBitmapFromGdiDib( IntPtr pDibBih, IntPtr pDibPixels, out IntPtr pBmp );

        [DllImport( "gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode )]
        internal static extern int GdipSaveImageToFile( IntPtr pBmp, string file, [In] ref Guid clsidEncoder, IntPtr pEncoderParams );

        [DllImport( "gdiplus.dll", ExactSpelling = true )]
        internal static extern int GdipDisposeImage( IntPtr pBmp );


        // ------ Supporting Structures
        [StructLayout( LayoutKind.Sequential )]
        internal struct GdipStartupInput
        {
            public int GdiplusVersion;
            public IntPtr DebugEventCallback;
            public bool SuppressBackgroundThread;
            public bool SuppressExternalCodecs;
        }

        [StructLayout( LayoutKind.Sequential )]
        internal struct GdipStartupOutput
        {
            public IntPtr hook;
            public IntPtr unhook;
        }

        [Flags]
        public enum GlobalAllocFlags : uint
        {
            GMEM_FIXED = 0x0000,
            GMEM_MOVEABLE = 0x0002,
            GMEM_ZEROINIT = 0x0040,
            GPTR = GMEM_FIXED | GMEM_ZEROINIT,
            GHND = GMEM_MOVEABLE | GMEM_ZEROINIT
        }

        // 40 byte bitmap header
        [StructLayout( LayoutKind.Sequential, Pack = 2 )]
        internal class BITMAPINFOHEADER
        {
            public uint HeaderSize { get; set; }            // biSize
            public int BitmapWidth { get; set; }            // biWidth - width of the image in pixels
            public int BitmapHeight { get; set; }           // biHeight - height of the image in pixels
            public ushort ColorPlanes { get; set; }         // biPlanes
            public ushort BitPerPixel { get; set; }         // biBitCount
            public uint CompressionMethod { get; set; }     // biCompression
            public uint BitmapSize { get; set; }            // biSizeImage - size of the image in bytes
            public int HorizontalResolution { get; set; }   // biXPelsPerMeter - X resolution in pixels/meter
            public int VerticalResolution { get; set; }     // biYPelsPerMeter - Y resolution in pixels/meter
            public uint ColorsUsed { get; set; }            // biClrUsed
            public uint ColorsImportant { get; set; }       // biClrImportant
        }

        internal enum BIHCompressionMethod : uint
        {
            BI_RGB = 0,
            BI_RLE8 = 1,
            BI_RLE4 = 2,
            BI_BITFIELDS = 3,
            BI_JPEG = 4,
            BI_PNG = 5,
            BI_ALPHABITFIELDS = 6,
            BI_CMYK = 11,
            BI_CMYKRLE8 = 12,
            BI_CMYKTLE4 = 13
        }

        [StructLayout( LayoutKind.Sequential, Pack = 4 )]
        internal struct WINMSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public int time;
            public int x;
            public int y;
        }
    }
}
