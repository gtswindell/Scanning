/*
 * BMP / DIB references:
 * http://en.wikipedia.org/wiki/BMP_file_format
 * http://msdn.microsoft.com/en-us/library/windows/desktop/dd183376(v=vs.85).aspx
 * http://msdn.microsoft.com/en-us/library/windows/desktop/dd318229(v=vs.85).aspx
 */

using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


namespace TWAINComm.Imaging
{
    internal static class DIB
    {
        /// <summary>
        /// Converts an in-memory DIB to a PNG and saves it to a temp file
        /// </summary>
        /// <param name="hMemDib">A pointer to the memory address of the DIB</param>
        /// <returns>The file location of the temporary file where the PNG file was stored</returns>
        internal static string ConvertToPng( IntPtr hMemDib )
        {
            string tempFile = Path.GetTempFileName();
            IntPtr pBIH = IntPtr.Zero;
            IntPtr pBmp = IntPtr.Zero;
            IntPtr gdipToken = IntPtr.Zero;
            Exception error = null;

            // BASIC STEPS
            // determin the position of the DIB bitmap info header and lock the memory
            // determin the position of the DIB pixels
            // convert from an in-memory DIB to an in-memory BMP
            // convert from an in-memory BMP to a PNG file
            // free all memory associated with the DIB and subsiquent conversions

            try
            {
                // check to make sure that we have a valid pointer to a DIB
                if ( hMemDib == IntPtr.Zero )
                {
                    throw new Exception( "The handle to the scanned in-memory DIB to be converted is null" );
                }

                // lock the memory to the DIB and get a pointer to the bitmap info header
                pBIH = Extern.GlobalLock( hMemDib );
                if ( pBIH == IntPtr.Zero )
                {
                    throw new Exception( "An error was encountered while attempting to obtain a pointer to the in-memory DIB bitmap info header" );
                }

                // calculate the position to the pixels of the DIB and set a pointer to their first position
                IntPtr pPixels = IntPtr.Zero;
                pPixels = GetDibPixelReference( pBIH );
                if ( pPixels == IntPtr.Zero )
                {
                    throw new Exception( "An error was encountered while attempting to obtain a pointer to the pixels of the in-memory DIB" );
                }

                // load the GDI Plus library
                Extern.GdipStartupInput startupInput = new Extern.GdipStartupInput()
                {
                    GdiplusVersion = 1
                };
                Extern.GdipStartupOutput startupOutput = new Extern.GdipStartupOutput();
                if ( Extern.GdiplusStartup( out gdipToken, ref startupInput, out startupOutput ) != 0 )
                {
                    throw new Exception( "GDI+ API failed to load" );
                }

                // convert the in-memory DIB to an in-memory BMP using the pointers to the BIH and pixels
                if ( ( Extern.GdipCreateBitmapFromGdiDib( pBIH, pPixels, out pBmp ) != 0 ) || ( pBmp == IntPtr.Zero ) )
                {
                    throw new Exception( "The scanned in-memory DIB could not be converted to a bitmap" );
                }

                // retrieve a PNG image codec info object
                ImageCodecInfo imgCodecInfo;
                if ( ( imgCodecInfo = GetImageCodecInfo( "image/png" ) ) == null )
                {
                    throw new Exception( "The png image codec could not be acquired" );
                }

                // convert the in-memory BMP to a PNG image using the previously obtained image codec and save it to a file
                Guid clsidEncoder = imgCodecInfo.Clsid;
                if ( Extern.GdipSaveImageToFile( pBmp, tempFile, ref clsidEncoder, IntPtr.Zero ) != 0 )
                {
                    throw new Exception( "The scanned in-memory bitmap failed to be saved to a temp file" );
                }
            }
            catch ( Exception ex )
            {
                error = ex;

                if ( !string.IsNullOrEmpty( tempFile ) && File.Exists( tempFile ) )
                {
                    File.Delete( tempFile );
                    tempFile = string.Empty;
                }
            }
            finally
            {
                if ( pBIH != IntPtr.Zero )
                {
                    Extern.GlobalUnlock( hMemDib );
                    pBIH = IntPtr.Zero;
                }
                
                if ( hMemDib != IntPtr.Zero )
                {
                    Extern.GlobalFree( hMemDib );
                    hMemDib = IntPtr.Zero;
                }

                if ( pBmp != IntPtr.Zero )
                {
                    Extern.GdipDisposeImage( pBmp );
                    pBmp = IntPtr.Zero;
                }

                if ( gdipToken != IntPtr.Zero )
                {
                    Extern.GdiplusShutdown( gdipToken );
                    gdipToken = IntPtr.Zero;
                }
            }

            if ( error != null )
            {
                throw error;
            }

            return tempFile;
        }

        /// <summary>
        /// Calculates the position of the pixel array for the device independent bitmap (DIB)
        /// </summary>
        /// <param name="hMemDib">A pointer to the memory address of the DIB</param>
        /// <returns>A pointer to the memory address of the header for the DIB</returns>
        private static IntPtr GetDibPixelReference( IntPtr hMemDib )
        {
            // we assume that the device is using either a 40, 52, 56, 108, or 124 bit bitmap header
            // the 40 bit bitmap header we use will work with any of the above, since the larger (newer) are compatible with the smaller (older)
            // if it's using one of the older 12, or 64 bit headers then the 40 bit bitmap header we use will fail
            // but I don't want to spend time supporting those since they were built for Win2 and OS/2 anyway
            Extern.BITMAPINFOHEADER bmpInfoHeader = new Extern.BITMAPINFOHEADER();
            Marshal.PtrToStructure( hMemDib, bmpInfoHeader );

            // bitmap size can be set to 0 for RGB compressed DIBs (compression method 0)
            // this is the most common compression method so check to see if it is 0 and calculate it out if it is
            if ( bmpInfoHeader.BitmapSize == 0 )
            {
                // we need the absolute value of height since it can be a negative value; indicating that it's a top down DIB
                bmpInfoHeader.BitmapSize = (uint)( ( ( ( ( bmpInfoHeader.BitmapWidth * bmpInfoHeader.BitPerPixel ) + 31 ) & ~31 ) >> 3 ) * Math.Abs( bmpInfoHeader.BitmapHeight ) );
            }

            // if a color palette is used, determine how many colors are in it
            ulong pPixelArray = bmpInfoHeader.ColorsUsed;

            // if a value of 0 was stored and it's a b&w or gray scale image, then calculate it out manually based on the bit count
            if ( ( pPixelArray == 0 ) && ( bmpInfoHeader.BitPerPixel <= 8 ) )
            {
                pPixelArray = (ulong)1 << bmpInfoHeader.BitPerPixel;
            }

            // multiply the number of colors in the color palette (if any) by 4 since they're stored as 32 bit DWORDs
            // add the size of the bitmap info header, then add the value of the pointer to the DIB
            // this will give us the location of the first pixel of the pixel array in memory
            pPixelArray = ( pPixelArray * 4 ) + bmpInfoHeader.HeaderSize + (ulong)hMemDib;

            // if the compression method BI_BITFIELDS is used, then 12 bytes (3 DWORDs) are used between the header
            // and the pixel array to store red, green, and blue bitmasks
            if ( bmpInfoHeader.CompressionMethod == (uint)Extern.BIHCompressionMethod.BI_BITFIELDS )
            {
                pPixelArray += 12;
            }

            return (IntPtr)pPixelArray;
        }

        /// <summary>
        /// Determines the image codec info for a given image mime type
        /// </summary>
        /// <param name="mimeType">The mime type for a specific image format</param>
        /// <returns>The image codec info for the given image mime type</returns>
        private static ImageCodecInfo GetImageCodecInfo( string mimeType )
        {
            return ImageCodecInfo.GetImageEncoders().Where( codec => codec.MimeType.IndexOf( mimeType ) >= 0 ).FirstOrDefault();
        }
    }
}
