/* ======================================================================== *\

  Copyright (C) 1991, 1992 TWAIN Working Group: Aldus, Caere, Eastman-Kodak,
  Hewlett-Packard and Logitech Corporations.  All rights reserved.

  Copyright (C) 1997 TWAIN Working Group: Bell+Howell, Canon, DocuMagix, 
  Fujitsu, Genoa Technology, Hewlett-Packard, Kofax Imaging Products, and
  Ricoh Corporation.  All rights reserved.
 
  Copyright © 1998 TWAIN Working Group: Adobe Systems Incorporated, 
  Canon Information Systems, Eastman Kodak Company, 
  Fujitsu Computer Products of America, Genoa Technology, 
  Hewlett-Packard Company, Intel Corporation, Kofax Image Products, 
  JFL Peripheral Solutions Inc., Ricoh Corporation, and Xerox Corporation.  
  All rights reserved.

  Copyright © 2000 TWAIN Working Group: Adobe Systems Incorporated, 
  Canon Information Systems, Digimarc Corporation, Eastman Kodak Company, 
  Fujitsu Computer Products of America, Hewlett-Packard Company, 
  JFL Peripheral Solutions Inc., Ricoh Corporation, and Xerox Corporation.  
  All rights reserved.


  TWAIN.h -  This is the definitive include file for applications and
          data sources written to the TWAIN specification.
          It defines constants, data structures, messages etc.
          for the public interface to TWAIN.
 
  Revision History:
    version 1.0, March 6, 1992.  TWAIN 1.0.
    version 1.1, January 1993.   Tech Notes 1.1
    version 1.5, June 1993.      Specification Update 1.5
                                 Change DC to TW 
                                 Change filename from DC.H to TWAIN.H
    version 1.5, July 1993.      Remove spaces from country identifiers
 
    version 1.7, July 1997       Added Capabilities and data structure for 
                                 document imaging and digital cameras.
                                 KHL.
    version 1.7, July 1997       Inserted Borland compatibile structure packing
                                 directives provided by Mentor.  JMH
    version 1.7, Aug 1997        Expanded file tabs to spaces.  
                                 NOTE: future authors should be sure to have 
                                 their editors set to automatically expand tabs 
                                 to spaces (original tab setting was 4 spaces).
    version 1.7, Sept 1997       Added job control values
                                 Added return codes
    version 1.7, Sept 1997       changed definition of pRGBRESPONSE to 
                                 pTW_RGBRESPONSE
    version 1.7  Aug 1998        Added missing TWEI_BARCODEROTATION values
                                 TWBCOR_ types JMH
    version 1.8  August 1998     Added new types and definitions required
                                 for 1.8 Specification JMH
    version 1.8  January 1999    Changed search mode from SRCH_ to TWBD_ as
                                 in 1.8 Specification, added TWBT_MAXICODE	JMH
	  version 1.8  January 1999    Removed undocumented duplicate AUTO<cap> JMH
    version 1.8  March 1999      Removed undocumented 1.8 caps:
                                 CAP_FILESYSTEM
                                 CAP_PAPERBINDING
                                 CAP_PASSTHRU
                                 CAP_POWERDOWNTIME
                                 ICAP_AUTODISCARDBLANKPAGES
                               * CAP_PAGEMULTIPLEACQUIRE - is CAP_REACQUIREALLOWED,
							                   requires spec change.  JMH
                                 Added Mac structure packing modifications JMH
	  version 1.9  March 2000	     Added new types and definations required
	                               for 1.9 Specification MLM
	  version 1.9  March 2000	     Added ICAP_JPEGQUALITY, TWJQ_ values,
                                 updated TWON_PROTOCOLMINOR for Release v1.9 MN
\* ======================================================================== */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TWAINComm
{
    /****************************************************************************
    * Definitions and Typedefs                                                  *
    ****************************************************************************/
    #region Definitions and Typedefs
    using TW_HANDLE = System.IntPtr;
    using TW_MEMREF = System.IntPtr;

    // numeric types
    using TW_INT8 = SByte;
    using TW_INT16 = Int16;
    using TW_INT32 = Int32;
    using TW_UINT8 = Byte;
    using TW_UINT16 = UInt16;
    using TW_UINT32 = UInt32;
    using TW_BOOL = UInt16;

    // string type constants
    // these include room for the strings and a null char
    public enum TWSTR : int
    {
        STR32 = 34,
        STR64 = 66,
        STR128 = 130,
        STR255 = 256,
        STR1024 = 1026, // added 1.9
        UNI512 = 512    // added 1.9
    }

    // Fixed point structure type
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_FIX32
    {
        public TW_INT16 Whole; // maintains the sign
        public TW_UINT16 Frac;

        //public float ToFloat()
        //{
        //    return (float)Whole + ( (float)Frac / 65536.0f );
        //}

        //public void FromFloat( float f )
        //{
        //    int i = (int)( ( f * 65536.0f ) + 0.5f );
        //    Whole = (short)( i >> 16 );
        //    Frac = (ushort)( i & 0x0000ffff );
        //}
    }
    #endregion Definitions and Typedefs


    /****************************************************************************
     * TWAIN Version                                                            *
     ****************************************************************************/
    #region TWAIN Version
    internal struct TWON_PROTOCOL
    {
        public const ushort MINOR = 9; // Changed for Version 1.9
        public const ushort MAJOR = 1;
    }
    #endregion TWAIN Version


    /****************************************************************************
     * Structure Definitions                                                    *
     ****************************************************************************/
    #region Structure Definitions
    // no DAT needed.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_CIEPOINT
    {
        public TW_FIX32 X;
        public TW_FIX32 Y;
        public TW_FIX32 Z;
    }

    // no DAT needed.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_DECODEFUNCTION
    {
        public TW_FIX32 StartIn;
        public TW_FIX32 BreakIn;
        public TW_FIX32 EndIn;
        public TW_FIX32 StartOut;
        public TW_FIX32 BreakOut;
        public TW_FIX32 EndOut;
        public TW_FIX32 Gamma;
        public TW_FIX32 SampleCount;  /* if =0 use the gamma */
    }

    // no DAT needed
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_ELEMENT8
    {
        public TW_UINT8 Index;    // value used to index into the color table
        public TW_UINT8 Channel1; // first  tri-stimulus value (e.g Red)
        public TW_UINT8 Channel2; // second tri-stimulus value (e.g Green)
        public TW_UINT8 Channel3; // third  tri-stimulus value (e.g Blue)
    }

    // no DAT needed.  defines a frame rectangle in ICAP_UNITS coordinates.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_FRAME
    {
        public TW_FIX32 Left;
        public TW_FIX32 Top;
        public TW_FIX32 Right;
        public TW_FIX32 Bottom;
    }

    // no DAT needed.  used to manage memory buffers.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_MEMORY
    {
        public TW_UINT32 Flags;  /* Any combination of the TWMF_ constants.           */
        public TW_UINT32 Length; /* Number of bytes stored in buffer TheMem.          */
        public TW_MEMREF TheMem; /* Pointer or handle to the allocated memory buffer. */
    }

    // no DAT needed
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_TRANSFORMSTAGE
    {
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
        public TW_DECODEFUNCTION[] Decode;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 9 )]
        public TW_FIX32[] Mix;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_VERSION
    {
        public TW_UINT16 MajorNum;                          // Major revision number of the software
        public TW_UINT16 MinorNum;                          // Incremental revision number of the software
        public TW_UINT16 Language;                          // e.g. TWLG_SWISSFRENCH 
        public TW_UINT16 Country;                           // e.g. TWCY_SWITZERLAND

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR32 )]
        public string Info;                                 // e.g. "1.0b3 Beta release"
    }

    // TWON_ARRAY. Container for array of values (a simplified TW_ENUMERATION)
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_ARRAY
    {
        public TW_UINT16 ItemType;
        public TW_UINT32 NumItems;                          // How many items in ItemList
        public TW_UINT8[] ItemList;                         // Array of ItemType values starts here
    }

    // TWON_ENUMERATION. Container for a collection of values
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_ENUMERATION
    {
        public TW_UINT16 ItemType;
        public TW_UINT32 NumItems;                          // How many items in ItemList
        public TW_UINT32 CurrentIndex;                      // Current value is in ItemList[CurrentIndex]
        public TW_UINT32 DefaultIndex;                      // Powerup value is in ItemList[DefaultIndex]
        public TW_UINT8[] ItemList;                         // Array of ItemType values start here
    }

    // TWON_ONEVALUE. Container for one value
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_ONEVALUE
    {
        public TW_UINT16 ItemType;
        public TW_UINT32 Item;
    }

    // TWON_RANGE. Container for a range of values
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_RANGE
    {
        public TW_UINT16 ItemType;
        public TW_UINT32 MinValue;                          // Starting value in the range
        public TW_UINT32 MaxValue;                          // Final value in the range
        public TW_UINT32 StepSize;                          // Increment from MinValue to MaxValue
        public TW_UINT32 DefaultValue;                      // Power-up value
        public TW_UINT32 CurrentValue;                      // The value that is currently in effect
    }

    // DAT_CAPABILITY. Used by application to get/set capability from/to a data source
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_CAPABILITY
    {
        public TW_UINT16 Cap;                                   // id of capability to set or get, e.g. CAP_BRIGHTNESS
        public TW_UINT16 ConType = (ushort)TWON.DONTCARE16;     // TWON_ONEVALUE, _RANGE, _ENUMERATION or _ARRAY
        public TW_HANDLE hContainer = IntPtr.Zero;              // Handle to container of type Dat
    }

    // DAT_CIECOLOR
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_CIECOLOR
    {
        public TW_UINT16 ColorSpace;
        public TW_INT16 LowEndian;
        public TW_INT16 DeviceDependent;
        public TW_INT32 VersionNumber;
        public TW_TRANSFORMSTAGE StageABC;
        public TW_TRANSFORMSTAGE StageLMN;
        public TW_CIEPOINT WhitePoint;
        public TW_CIEPOINT BlackPoint;
        public TW_CIEPOINT WhitePaper;
        public TW_CIEPOINT BlackInk;
        public TW_FIX32[] Samples;
    }

    // DAT_EVENT. For passing events down from the application to the DS.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_EVENT
    {
        public TW_MEMREF EventPtr;                          // Windows pMSG
        public TW_UINT16 Message;                           // TW msg from data source, e.g. MSG_XFERREADY
    }

    /* DAT_GRAYRESPONSE */
    public class TW_GRAYRESPONSE
    {
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1 )]
        public TW_ELEMENT8[] Response;
    }

    // DAT_IDENTITY. Identifies the program/library/code resource.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_IDENTITY
    {
        public TW_UINT32 Id;                                // Unique number.  In Windows, application hWnd
        public TW_VERSION Version;                          // Identifies the piece of code
        public TW_UINT16 ProtocolMajor;                     // Application and DS must set to TWON_PROTOCOLMAJOR
        public TW_UINT16 ProtocolMinor;                     // Application and DS must set to TWON_PROTOCOLMINOR
        public TW_UINT32 SupportedGroups;                   // Bit field OR combination of DG_ constants

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR32 )]
        public string Manufacturer;                         // Manufacturer name, e.g. "Hewlett-Packard"

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR32 )]
        public string ProductFamily;                        // Product family name, e.g. "ScanJet"

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR32 )]
        public string ProductName;                          // Product name, e.g. "ScanJet Plus"
    }

    // DAT_IMAGEINFO. Application gets detailed image info from DS with this.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_IMAGEINFO
    {
        public TW_FIX32 XResolution;
        public TW_FIX32 YResolution;
        public TW_INT32 ImageWidth;
        public TW_INT32 ImageLength;
        public TW_INT16 SamplesPerPixel;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 8 )]
        public TW_INT16[] BitsPerSample;

        public TW_INT16 BitsPerPixel;
        public TW_BOOL Planar;
        public TW_INT16 PixelType;
        public TW_UINT16 Compression;
    }

    // DAT_IMAGELAYOUT. Provides image layout information in current units.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_IMAGELAYOUT
    {
        public TW_FRAME Frame;             // Frame coords within larger document
        public TW_UINT32 DocumentNumber;
        public TW_UINT32 PageNumber;       // Reset when you go to next document
        public TW_UINT32 FrameNumber;      // Reset when you go to next page
    }

    // DAT_IMAGEMEMXFER. Used to pass image data (e.g. in strips) from DS to application.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_IMAGEMEMXFER
    {
        public TW_UINT16 Compression;  // How the data is compressed
        public TW_UINT32 BytesPerRow;  // Number of bytes in a row of data
        public TW_UINT32 Columns;      // How many columns
        public TW_UINT32 Rows;         // How many rows
        public TW_UINT32 XOffset;      // How far from the side of the image
        public TW_UINT32 YOffset;      // How far from the top of the image
        public TW_UINT32 BytesWritten; // How many bytes written in Memory
        public TW_MEMORY Memory;       // Mem struct used to pass actual image data
    }

    // Changed in 1.1: QuantTable, HuffmanDC, HuffmanAC TW_MEMREF -> TW_MEMORY
    // DAT_JPEGCOMPRESSION. Based on JPEG Draft International Std, ver 10918-1.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_JPEGCOMPRESSION
    {
        public TW_UINT16 ColorSpace;        // One of the TWPT_xxxx values
        public TW_UINT32 SubSampling;       // Two word "array" for subsampling values
        public TW_UINT16 NumComponents;     // Number of color components in image
        public TW_UINT16 RestartFrequency;  // Frequency of restart marker codes in MDU's

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
        public TW_UINT16[] QuantMap;        // Mapping of components to QuantTables

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
        public TW_MEMORY[] QuantTable;      // Quantization tables

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
        public TW_UINT16[] HuffmanMap;      // Mapping of components to Huffman tables

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 2 )]
        public TW_MEMORY[] HuffmanDC;       // DC Huffman tables

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 2 )]
        public TW_MEMORY[] HuffmanAC;       // AC Huffman tables
    }

    // DAT_PALETTE8. Color palette when TWPT_PALETTE pixels xfer'd in mem buf.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_PALETTE8
    {
        public TW_UINT16 NumColors;   /* Number of colors in the color table.  */
        public TW_UINT16 PaletteType; /* TWPA_xxxx, specifies type of palette. */

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
        public TW_ELEMENT8[] Colors; /* Array of palette values starts here.  */
    }

    // DAT_PENDINGXFERS. Used with MSG_ENDXFER to indicate additional data.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_PENDINGXFERS
    {
        public TW_UINT16 Count;
        public TW_UINT32 EOJ;
    }

    // DAT_RGBRESPONSE
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_RGBRESPONSE
    {
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1 )]
        public TW_ELEMENT8[] Response;
    }

    // DAT_SETUPFILEXFER. Sets up DS to application data transfer via a file.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_SETUPFILEXFER
    {
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR255 )]
        public string FileName;

        public TW_UINT16 Format;   // Any TWFF_ constant
        public TW_INT16 VRefNum;   // Used for Mac only
    }

    // DAT_SETUPFILEXFER2. Sets up DS to application data transfer via a file.
    // Added 1.9
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_SETUPFILEXFER2
    {
        public TW_MEMREF FileName;     // Pointer to file name text
        public TW_UINT16 FileNameType; // TWTY_STR1024 or TWTY_UNI512
        public TW_UINT16 Format;       // Any TWFF_ constant
        public TW_INT16 VRefNum;       // Used for Mac only
        public TW_UINT32 parID;        // Used for Mac only
    }

    // DAT_SETUPMEMXFER. Sets up DS to application data transfer via a memory buffer.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_SETUPMEMXFER
    {
        public TW_UINT32 MinBufSize;
        public TW_UINT32 MaxBufSize;
        public TW_UINT32 Preferred;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public struct TW_STATUS
    {
        public TW_UINT16 ConditionCode; // Any TWCC_ constant
        public TW_UINT16 Reserved;      // Future expansion space
    }

    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_USERINTERFACE
    {
        public short ShowUI;    // TRUE if DS should bring up its UI
        public short ModalUI;   // For Mac only - true if the DS's UI is modal
        public IntPtr hParent;  // For windows only - Application window handle
    }

    // SDH - 03/21/95 - TWUNK
    // DAT_TWUNKIDENTITY. Provides DS identity and 'other' information necessary
    //                    across thunk link.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_TWUNKIDENTITY
    {
        public TW_IDENTITY identity;   // Identity of data source.

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR255 )]
        public string[] dsPath;        // Full path and file name of data source.
    }

    // SDH - 03/21/95 - TWUNK
    // Provides DS_Entry parameters over thunk link.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_TWUNKDSENTRYPARAMS
    {
        public TW_INT8 destFlag;        // TRUE if dest is not NULL
        public TW_IDENTITY dest;        // Identity of data source (if used)
        public TW_INT32 dataGroup;      // DSM_Entry dataGroup parameter
        public TW_INT16 dataArgType;    // DSM_Entry dataArgType parameter
        public TW_INT16 message;        // DSM_Entry message parameter
        public TW_INT32 pDataSize;      // Size of pData (0 if NULL)
        // public  TW_MEMREF   pData;   // Based on implementation specifics, a
        // pData parameter makes no sense in this
        // structure, but data (if provided) will be
        // appended in the data block.
    }

    // SDH - 03/21/95 - TWUNK
    // Provides DS_Entry results over thunk link.
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_TWUNKDSENTRYRETURN
    {
        public TW_UINT16 returnCode;        // Thunker DsEntry return code.
        public TW_UINT16 conditionCode;     // Thunker DsEntry condition code.
        public TW_INT32 pDataSize;          // Size of pData (0 if NULL)
        // public  TW_MEMREF   pData;       // Based on implementation specifics, a
        // pData parameter makes no sense in this
        // structure, but data (if provided) will be
        // appended in the data block.
    }

    // WJD - 950818
    // Added for 1.6 Specification
    // TWAIN 1.6 CAP_SUPPORTEDCAPSEXT structure
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_CAPEXT
    {
        public TW_UINT16 Cap;           // Which CAP/ICAP info is relevant to
        public TW_UINT16 Properties;    // Messages this CAP/ICAP supports
    }

    /* ----------------------------------------------------------------------- *\
    Version 1.7:    Added Following data structure for Document Imaging 
    July 1997       Enhancement.
    KHL             TW_CUSTOMDSDATA --  For Saving and Restoring Source's 
                                        state.
                    TW_INFO         --  Each attribute for extended image
                                        information.
                    TW_EXTIMAGEINFO --  Extended image information structure.
    \* ----------------------------------------------------------------------- */
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_CUSTOMDSDATA
    {
        public TW_UINT32 InfoLength;    // Length of Information in bytes.
        public TW_HANDLE hData;         // Place holder for data, DS Allocates
    }

    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_INFO
    {
        public TW_UINT16 InfoID;
        public TW_UINT16 ItemType;
        public TW_UINT16 NumItems;
        public TW_UINT16 CondCode;
        public TW_UINT32 Item;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_EXTIMAGEINFO
    {
        public TW_UINT32 NumInfos;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1 )]
        public TW_INFO[] Info;
    }

    /* Added 1.8 */
    // DAT_AUDIOINFO, information about audio data
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_AUDIOINFO
    {
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR255 )]
        public string Name;         // name of audio data

        public TW_UINT32 Reserved;  // reserved space
    }

    // DAT_DEVICEEVENT, information about events
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_DEVICEEVENT
    {
        public TW_UINT32 Event;                     /* One of the TWDE_xxxx values. */

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR255 )]
        public string DeviceName;                   /* The name of the device that generated the event */

        public TW_UINT32 BatteryMinutes;            /* Battery Minutes Remaining    */
        public TW_INT16 BatteryPercentage;          /* Battery Percentage Remaining */
        public TW_INT32 PowerSupply;                /* Power Supply                 */
        public TW_FIX32 XResolution;                /* Resolution                   */
        public TW_FIX32 YResolution;                /* Resolution                   */
        public TW_UINT32 FlashUsed2;                /* Flash Used2                  */
        public TW_UINT32 AutomaticCapture;          /* Automatic Capture            */
        public TW_UINT32 TimeBeforeFirstCapture;    /* Automatic Capture            */
        public TW_UINT32 TimeBetweenCaptures;       /* Automatic Capture            */
    }

    // DAT_FILESYSTEM, information about TWAIN file system
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_FILESYSTEM
    {
        // DG_CONTROL / DAT_FILESYSTEM / MSG_xxxx fields
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR255 )]
        public string InputName;            // The name of the input or source file

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR255 )]
        public string OutputName;           // The result of an operation or the name of a destination file

        public TW_MEMREF Context;           // Source specific data used to remember state information

        // DG_CONTROL / DAT_FILESYSTEM / MSG_DELETE field
        public int Recursive;               // recursively delete all sub-directories

        // DG_CONTROL / DAT_FILESYSTEM / MSG_GETINFO fields
        public TW_INT32 FileType;           // One of the TWFT_xxxx values
        public TW_UINT32 Size;              // Size of current FileType

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR32 )]
        public string CreateTimeDate;       // creation date of the file

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR32 )]
        public string ModifiedTimeDate;     // last date the file was modified

        public TW_UINT32 FreeSpace;         // bytes of free space on the current device
        public TW_INT32 NewImageSize;       // estimate of the amount of space a new image would take up
        public TW_UINT32 NumberOfFiles;     // number of files, depends on FileType
        public TW_UINT32 NumberOfSnippets;  // number of audio snippets
        public TW_UINT32 DeviceGroupMask;   // used to group cameras (ex: front/rear bitonal, front/rear grayscale...)

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 508 )]
        public char[] Reserved; /**/
    }

    // DAT_PASSTHRU, device dependant data to pass through Data Source
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_PASSTHRU
    {
        public TW_MEMREF pCommand;          // Pointer to Command buffer
        public TW_UINT32 CommandBytes;      // Number of bytes in Command buffer
        public TW_INT32 Direction;          // One of the TWDR_xxxx values.  Defines the direction of data flow
        public TW_MEMREF pData;             // Pointer to Data buffer
        public TW_UINT32 DataBytes;         // Number of bytes in Data buffer
        public TW_UINT32 DataBytesXfered;   // Number of bytes successfully transferred
    }

    // DAT_SETUPAUDIOFILEXFER, information required to setup an audio file transfer
    [StructLayout( LayoutKind.Sequential, Pack = 2 )]
    public class TW_SETUPAUDIOFILEXFER
    {
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = (int)TWSTR.STR255 )]
        public string FileName;     // full path target file

        public TW_UINT16 Format;    // one of TWAF_xxxx
        public TW_INT16 VRefNum;
    }
    #endregion Structure Definitions


    /****************************************************************************
    * Generic Constants                                                        *
    ****************************************************************************/
    #region Generic Constants
    // TWON_
    public enum TWON : uint
    {
        ARRAY = 3,      // indicates TW_ARRAY container
        ENUM = 4,       // indicates TW_ENUMERATION container
        ONE = 5,        // indicates TW_ONEVALUE container
        RANGE = 6,      // indicates TW_RANGE container

        ICONID = 962,   // res Id of icon used in USERSELECT lbox
        DSMID = 461,    // res Id of the DSM version num resource
        DSMCODEID = 63, // res Id of the Mac SM Code resource

        DONTCARE8 = 0xff,
        DONTCARE16 = 0xffff,
        DONTCARE32 = 0xffffffff
    }

    // Flags used in TW_MEMORY structure.
    public enum TWMF : uint // TW_UINT32
    {
        APPOWNS = 0x1,
        DSMOWNS = 0x2,
        DSOWNS = 0x4,
        POINTER = 0x8,
        HANDLE = 0x10
    }

    // Palette types for TW_PALETTE8
    public enum TWPA : ushort // TW_UINT16
    {
        RGB = 0,
        GRAY = 1,
        CMY = 2
    }

    /* There are four containers used for capabilities negotiation:
    *    TWON_ONEVALUE, TWON_RANGE, TWON_ENUMERATION, TWON_ARRAY
    * In each container structure ItemType can be TWTY_INT8, TWTY_INT16, etc.
    * The kind of data stored in the container can be determined by doing
    * DCItemSize[ItemType] where the following is defined in TWAIN glue code:
    *          DCItemSize[]= { sizeof(TW_INT8),
    *                          sizeof(TW_INT16),
    *                          etc.
    *                          sizeof(TW_UINT32) };
    */

    // Types - TWTY_
    public enum TWTY : ushort
    {
        INT8 = 0x0000,      // Means Item is a TW_INT8
        INT16 = 0x0001,     // Means Item is a TW_INT16
        INT32 = 0x0002,     // Means Item is a TW_INT32

        UINT8 = 0x0003,     // Means Item is a TW_UINT8
        UINT16 = 0x0004,    // Means Item is a TW_UINT16
        UINT32 = 0x0005,    // Means Item is a TW_UINT32

        BOOL = 0x0006,      // Means Item is a TW_BOOL

        FIX32 = 0x0007,     // Means Item is a TW_FIX32

        FRAME = 0x0008,     // Means Item is a TW_FRAME

        STR32 = 0x0009,     // Means Item is a TW_STR32
        STR64 = 0x000a,     // Means Item is a TW_STR64
        STR128 = 0x000b,    // Means Item is a TW_STR128
        STR255 = 0x000c,    // Means Item is a TW_STR255
        STR1024 = 0x000d,   // Means Item is a TW_STR1024...added 1.9
        UNI512 = 0x000e     // Means Item is a TW_UNI512...added 1.9
    }
    #endregion Generic Constants


    /****************************************************************************
    * Capability Constants                                                     *
    ****************************************************************************/
    #region Capability Constants
    // ICAP_BITORDER values (BO_ means Bit Order)
    public enum TWBO : ushort // TW_UINT16
    {
        LSBFIRST = 0,
        MSBFIRST = 1
    }

    // ICAP_COMPRESSION values (CP_ means ComPression )
    public enum TWCP : ushort // TW_UINT16
    {
        NONE = 0,
        PACKBITS = 1,
        GROUP31D = 2,      // Follows CCITT spec (no End Of Line)
        GROUP31DEOL = 3,   // Follows CCITT spec (has End Of Line)
        GROUP32D = 4,      // Follows CCITT spec (use cap for K Factor)
        GROUP4 = 5,        // Follows CCITT spec
        JPEG = 6,          // Use capability for more info
        LZW = 7,           // Must license from Unisys and IBM to use
        JBIG = 8,          // For Bitonal images  -- Added 1.7 KHL

        /* Added 1.8 */
        PNG = 9,
        RLE4 = 10,
        RLE8 = 11,
        BITFIELDS = 12
    }

    // ICAP_IMAGEFILEFORMAT values (FF_means File Format)
    public enum TWFF : ushort // TW_UINT16
    {
        TIFF = 0,       // Tagged Image File Format
        PICT = 1,       // Macintosh PICT
        BMP = 2,        // Windows Bitmap
        XBM = 3,        // X-Windows Bitmap
        JFIF = 4,       // JPEG File Interchange Format
        FPX = 5,        // Flash Pix
        TIFFMULTI = 6,  // Multi-page tiff file
        PNG = 7,
        SPIFF = 8,
        EXIF = 9,
    }

    // ICAP_FILTER values (FT_ means Filter Type)
    public enum TWFT : ushort // TW_UINT16
    {
        RED = 0,
        GREEN = 1,
        BLUE = 2,
        NONE = 3,
        WHITE = 4,
        CYAN = 5,
        MAGENTA = 6,
        YELLOW = 7,
        BLACK = 8
    }

    // ICAP_LIGHTPATH values (LP_ means Light Path)
    public enum TWLP : ushort // TW_UINT16
    {
        REFLECTIVE = 0,
        TRANSMISSIVE = 1
    }

    // ICAP_LIGHTSOURCE values (LS_ means Light Source)
    public enum TWLS : ushort // TW_UINT16
    {
        RED = 0,
        GREEN = 1,
        BLUE = 2,
        NONE = 3,
        WHITE = 4,
        UV = 5,
        IR = 6
    }

    // ICAP_ORIENTATION values (OR_ means ORientation)
    public enum TWOR : ushort // TW_UINT16
    {
        ROT0 = 0,
        ROT90 = 1,
        ROT180 = 2,
        ROT270 = 3,
        PORTRAIT = ROT0,
        LANDSCAPE = ROT270
    }

    // ICAP_PLANARCHUNKY values (PC_ means Planar/Chunky )
    public enum TWPC : ushort // TW_UINT16
    {
        CHUNKY = 0,
        PLANAR = 1
    }

    // ICAP_PIXELFLAVOR values (PF_ means Pixel Flavor)
    public enum TWPF : ushort // TW_UINT16
    {
        CHOCOLATE = 0,  // zero pixel represents darkest shade 
        VANILLA = 1     // zero pixel represents lightest shade
    }

    // ICAP_PIXELTYPE values (PT_ means Pixel Type)
    public enum TWPT : ushort // TW_UINT16
    {
        BW = 0,     /* Black and White */
        GRAY = 1,
        RGB = 2,
        PALETTE = 3,
        CMY = 4,
        CMYK = 5,
        YUV = 6,
        YUVK = 7,
        CIEXYZ = 8
    }

    // ICAP_SUPPORTEDSIZES values (SS_ means Supported Sizes)
    public enum TWSS : ushort // TW_UINT16
    {
        NONE = 0,
        A4LETTER = 1,
        B5LETTER = 2,
        USLETTER = 3,
        USLEGAL = 4,

        /* Added 1.5 */
        A5 = 5,
        B4 = 6,
        B6 = 7,
        //B = 8,

        /* Added 1.7 */
        USLEDGER = 9,
        USEXECUTIVE = 10,
        A3 = 11,
        B3 = 12,
        A6 = 13,
        C4 = 14,
        C5 = 15,
        C6 = 16,

        /* Added 1.8 */
        FOURA0 = 17, // 4A0 - enums can't start with a number
        TWOA0 = 18, // 2A0 - enums can't start with a number
        A0 = 19,
        A1 = 20,
        A2 = 21,
        A4 = A4LETTER,
        A7 = 22,
        A8 = 23,
        A9 = 24,
        A10 = 25,
        ISOB0 = 26,
        ISOB1 = 27,
        ISOB2 = 28,
        ISOB3 = B3,
        ISOB4 = B4,
        ISOB5 = 29,
        ISOB6 = B6,
        ISOB7 = 30,
        ISOB8 = 31,
        ISOB9 = 32,
        ISOB10 = 33,
        JISB0 = 34,
        JISB1 = 35,
        JISB2 = 36,
        JISB3 = 37,
        JISB4 = 38,
        JISB5 = B5LETTER,
        JISB6 = 39,
        JISB7 = 40,
        JISB8 = 41,
        JISB9 = 42,
        JISB10 = 43,
        C0 = 44,
        C1 = 45,
        C2 = 46,
        C3 = 47,
        C7 = 48,
        C8 = 49,
        C9 = 50,
        C10 = 51,
        USSTATEMENT = 52,
        BUSINESSCARD = 53
    }

    // ICAP_XFERMECH values (SX_ means Setup XFer)
    public enum TWSX : ushort // TW_UINT16
    {
        NATIVE = 0,
        FILE = 1,
        MEMORY = 2,
        FILE2 = 3   // added 1.9
    }

    // ICAP_UNITS values (UN_ means UNits)
    public enum TWUN : ushort // TW_UINT16
    {
        INCHES = 0,
        CENTIMETERS = 1,
        PICAS = 2,
        POINTS = 3,
        TWIPS = 4,
        PIXELS = 5
    }

    /* Added 1.5 */
    // ICAP_BITDEPTHREDUCTION values (BR_ means Bitdepth Reduction)
    public enum TWBR : ushort // TW_UINT16
    {
        THRESHOLD = 0,
        HALFTONE = 1,
        CUSTHALFTONE = 2,
        DIFFUSION = 3
    }

    /* Added 1.7 */
    // ICAP_DUPLEX values
    public enum TWDX : ushort // TW_UINT16
    {
        NONE = 0,
        ONEPASSDUPLEX = 1,  // 1PASSDUPLEX - enums can't start with a number
        TWOPASSDUPLEX = 2   // 2PASSDUPLEX - enums can't start with a number
    }

    /* Added 1.7 */
    // TWEI_BARCODETYPE values
    public enum TWBT : ushort // TW_UINT16
    {
        THREE_OF_NINE = 0,               // 3OF9 - enums can't start with a number
        TWOOF5INTERLEAVED = 1,      // 2OF5INTERLEAVED - enums can't start with a number
        TWOOF5NONINTERLEAVED = 2,   // 2OF5NONINTERLEAVED - enums can't start with a number
        CODE93 = 3,
        CODE128 = 4,
        UCC128 = 5,
        CODABAR = 6,
        UPCA = 7,
        UPCE = 8,
        EAN8 = 9,
        EAN13 = 10,
        POSTNET = 11,
        PDF417 = 12,

        /* Added 1.8 */
        TWO_OF_FIVE_INDUSTRIAL = 13,      // 2OF5INDUSTRIAL - enums can't start with a number
        TWO_OF_FIVE_MATRIX = 14,          // 2OF5MATRIX - enums can't start with a number
        TWO_OF_FIVE_DATALOGIC = 15,       // 2OF5DATALOGIC - enums can't start with a number
        TWO_OF_FIVE_IATA = 16,            // 2OF5IATA - enums can't start with a number
        THREE_OF_NINE_FULLASCII = 17,     // 3OF9FULLASCII - enums can't start with a number
        CODABARWITHSTARTSTOP = 18,
        MAXICODE = 19
    }

    /* Added 1.7 */
    // TWEI_DESKEWSTATUS values
    public enum TWDSK : ushort // TW_UINT16
    {
        SUCCESS = 0,
        REPORTONLY = 1,
        FAIL = 2,
        DISABLED = 3
    }

    /* Added 1.7 */
    // TWEI_PATCHCODE values
    public enum TWPCH : ushort // TW_UINT16
    {
        PATCH1 = 0,
        PATCH2 = 1,
        PATCH3 = 2,
        PATCH4 = 3,
        PATCH6 = 4,
        PATCHT = 5
    }

    /* Added 1.7 */
    // CAP_JOBCONTROL values
    public enum TWJC : ushort // TW_UINT16
    {
        NONE = 0,
        JSIC = 1,
        JSIS = 2,
        JSXC = 3,
        JSXS = 4
    }

    /* Added 1.7 */
    // TWEI_BARCODEROTATION values (BCOR_ means barcode rotation)
    public enum TWBCOR : ushort // TW_UINT16
    {
        ROT0 = 0,
        ROT90 = 1,
        ROT180 = 2,
        ROT270 = 3,
        ROTX = 4
    }

    /* Added 1.8 */
    // ACAP_AUDIOFILEFORMAT values (AF_ means audio format)
    public enum TWAF : ushort // TW_UINT16
    {
        WAV = 0,
        AIFF = 1,
        AU = 3,
        SND = 4
    }

    // CAP_ALARMS values (AL_ means alarms)
    public enum TWAL : ushort // TW_UINT16
    {
        ALARM = 0,
        FEEDERERROR = 1,
        FEEDERWARNING = 2,
        BARCODE = 3,
        DOUBLEFEED = 4,
        JAM = 5,
        PATCHCODE = 6,
        POWER = 7,
        SKEW = 8
    }

    // CAP_CLEARBUFFERS values (CB_ means clear buffers)
    public enum TWCB : ushort // TW_UINT16
    {
        AUTO = 0,
        CLEAR = 1,
        NOCLEAR = 2
    }

    // CAP_DEVICEEVENT values (DE_ means device event)
    public enum TWDE : ushort // TW_UINT16
    {
        CUSTOMEVENTS = 0x8000,
        CHECKAUTOMATICCAPTURE = 0,
        CHECKBATTERY = 1,
        CHECKDEVICEONLINE = 2,
        CHECKFLASH = 3,
        CHECKPOWERSUPPLY = 4,
        CHECKRESOLUTION = 5,
        DEVICEADDED = 6,
        DEVICEOFFLINE = 7,
        DEVICEREADY = 8,
        DEVICEREMOVED = 9,
        IMAGECAPTURED = 10,
        IMAGEDELETED = 11,
        PAPERDOUBLEFEED = 12,
        PAPERJAM = 13,
        LAMPFAILURE = 14,
        POWERSAVE = 15,
        POWERSAVENOTIFY = 16
    }

    // CAP_FEEDERALIGNMENT values (FA_ means feeder alignment)
    public enum TWFA : ushort // TW_UINT16
    {
        NONE = 0,
        LEFT = 1,
        CENTER = 2,
        RIGHT = 3
    }

    // CAP_FEEDERORDER values (FO_ means feeder order)
    public enum TWFO : ushort // TW_UINT16
    {
        FIRSTPAGEFIRST = 0,
        LASTPAGEFIRST = 1
    }

    // CAP_FILESYSTEM values (FS_ means file system)
    public enum TWFS : ushort // TW_UINT16
    {
        FILESYSTEM = 0,
        RECURSIVEDELETE = 1
    }

    // CAP_POWERSUPPLY values (PS_ means power supply)
    public enum TWPS : ushort // TW_UINT16
    {
        EXTERNAL = 0,
        BATTERY = 1
    }

    // CAP_PRINTER values (PR_ means printer)
    public enum TWPR : ushort // TW_UINT16
    {
        IMPRINTERTOPBEFORE = 0,
        IMPRINTERTOPAFTER = 1,
        IMPRINTERBOTTOMBEFORE = 2,
        IMPRINTERBOTTOMAFTER = 3,
        ENDORSERTOPBEFORE = 4,
        ENDORSERTOPAFTER = 5,
        ENDORSERBOTTOMBEFORE = 6,
        ENDORSERBOTTOMAFTER = 7
    }

    // CAP_PRINTERMODE values (PM_ means printer mode)
    public enum TWPM : ushort // TW_UINT16
    {
        SINGLESTRING = 0,
        MULTISTRING = 1,
        COMPOUNDSTRING = 2
    }

    // ICAP_BARCODESEARCHMODE values (TWBD_ means search)
    public enum TWBD : ushort // TW_UINT16
    {
        HORZ = 0,
        VERT = 1,
        HORZVERT = 2,
        VERTHORZ = 3
    }

    // ICAP_FLASHUSED2 values (FL_ means flash)
    public enum TWFL : ushort // TW_UINT16
    {
        NONE = 0,
        OFF = 1,
        ON = 2,
        AUTO = 3,
        REDEYE = 4
    }

    // ICAP_FLIPROTATION values (FR_ means flip rotation)
    public enum TWFR : ushort // TW_UINT16
    {
        BOOK = 0,
        FANFOLD = 1
    }

    // ICAP_IMAGEFILTER values (IF_ means image filter)
    public enum TWIF : ushort // TW_UINT16
    {
        NONE = 0,
        AUTO = 1,
        LOWPASS = 2,
        BANDPASS = 3,
        HIGHPASS = 4,
        TEXT = BANDPASS,
        FINELINE = HIGHPASS
    }

    // ICAP_NOISEFILTER values (NF_ means noise filter)
    public enum TWNF : ushort // TW_UINT16
    {
        NONE = 0,
        AUTO = 1,
        LONEPIXEL = 2,
        MAJORITYRULE = 3
    }

    // ICAP_OVERSCAN values (OV_ means overscan)
    public enum TWOV : ushort // TW_UINT16
    {
        NONE = 0,
        AUTO = 1,
        TOPBOTTOM = 2,
        LEFTRIGHT = 3,
        ALL = 4
    }

    // TW_FILESYSTEM.FileType values (FT_ means file type)
    public enum TWFY : ushort // TW_UINT16
    {
        CAMERA = 0,
        CAMERATOP = 1,
        CAMERABOTTOM = 2,
        CAMERAPREVIEW = 3,
        DOMAIN = 4,
        HOST = 5,
        DIRECTORY = 6,
        IMAGE = 7,
        UNKNOWN = 8
    }

    // ICAP_JPEGQUALITY values (JQ_ means jpeg quality)
    public enum TWJQ //: ??? ushort // TW_UINT16
    {
        UNKNOWN = -4,
        LOW = -3,
        MEDIUM = -2,
        HIGH = -1
    }
    #endregion Capability Constants


    /****************************************************************************
    * Country Constants                                                        *
    ****************************************************************************/
    #region Country Constants
    /// <summary>
    /// TWAIN Country Codes
    /// </summary>
    public enum TWCY : short
    {
        AFGHANISTAN = 1001,
        ALGERIA = 213,
        AMERICANSAMOA = 684,
        ANDORRA = 033,
        ANGOLA = 1002,
        ANGUILLA = 8090,
        ANTIGUA = 8091,
        ARGENTINA = 54,
        ARUBA = 297,
        ASCENSIONI = 247,
        AUSTRALIA = 61,
        AUSTRIA = 43,
        BAHAMAS = 8092,
        BAHRAIN = 973,
        BANGLADESH = 880,
        BARBADOS = 8093,
        BELGIUM = 32,
        BELIZE = 501,
        BENIN = 229,
        BERMUDA = 8094,
        BHUTAN = 1003,
        BOLIVIA = 591,
        BOTSWANA = 267,
        BRITAIN = 6,
        BRITVIRGINIS = 8095,
        BRAZIL = 55,
        BRUNEI = 673,
        BULGARIA = 359,
        BURKINAFASO = 1004,
        BURMA = 1005,
        BURUNDI = 1006,
        CAMAROON = 237,
        CANADA = 2,
        CAPEVERDEIS = 238,
        CAYMANIS = 8096,
        CENTRALAFREP = 1007,
        CHAD = 1008,
        CHILE = 56,
        CHINA = 86,
        CHRISTMASIS = 1009,
        COCOSIS = 1009,
        COLOMBIA = 57,
        COMOROS = 1010,
        CONGO = 1011,
        COOKIS = 1012,
        COSTARICA = 506,
        CUBA = 005,
        CYPRUS = 357,
        CZECHOSLOVAKIA = 42,
        DENMARK = 45,
        DJIBOUTI = 1013,
        DOMINICA = 8097,
        DOMINCANREP = 8098,
        EASTERIS = 1014,
        ECUADOR = 593,
        EGYPT = 20,
        ELSALVADOR = 503,
        EQGUINEA = 1015,
        ETHIOPIA = 251,
        FALKLANDIS = 1016,
        FAEROEIS = 298,
        FIJIISLANDS = 679,
        FINLAND = 358,
        FRANCE = 33,
        FRANTILLES = 596,
        FRGUIANA = 594,
        FRPOLYNEISA = 689,
        FUTANAIS = 1043,
        GABON = 241,
        GAMBIA = 220,
        GERMANY = 49,
        GHANA = 233,
        GIBRALTER = 350,
        GREECE = 30,
        GREENLAND = 299,
        GRENADA = 8099,
        GRENEDINES = 8015,
        GUADELOUPE = 590,
        GUAM = 671,
        GUANTANAMOBAY = 5399,
        GUATEMALA = 502,
        GUINEA = 224,
        GUINEABISSAU = 1017,
        GUYANA = 592,
        HAITI = 509,
        HONDURAS = 504,
        HONGKONG = 852,
        HUNGARY = 36,
        ICELAND = 354,
        INDIA = 91,
        INDONESIA = 62,
        IRAN = 98,
        IRAQ = 964,
        IRELAND = 353,
        ISRAEL = 972,
        ITALY = 39,
        IVORYCOAST = 225,
        JAMAICA = 8010,
        JAPAN = 81,
        JORDAN = 962,
        KENYA = 254,
        KIRIBATI = 1018,
        KOREA = 82,
        KUWAIT = 965,
        LAOS = 1019,
        LEBANON = 1020,
        LIBERIA = 231,
        LIBYA = 218,
        LIECHTENSTEIN = 41,
        LUXENBOURG = 352,
        MACAO = 853,
        MADAGASCAR = 1021,
        MALAWI = 265,
        MALAYSIA = 60,
        MALDIVES = 960,
        MALI = 1022,
        MALTA = 356,
        MARSHALLIS = 692,
        MAURITANIA = 1023,
        MAURITIUS = 230,
        MEXICO = 3,
        MICRONESIA = 691,
        MIQUELON = 508,
        MONACO = 33,
        MONGOLIA = 1024,
        MONTSERRAT = 8011,
        MOROCCO = 212,
        MOZAMBIQUE = 1025,
        NAMIBIA = 264,
        NAURU = 1026,
        NEPAL = 977,
        NETHERLANDS = 31,
        NETHANTILLES = 599,
        NEVIS = 8012,
        NEWCALEDONIA = 687,
        NEWZEALAND = 64,
        NICARAGUA = 505,
        NIGER = 227,
        NIGERIA = 234,
        NIUE = 1027,
        NORFOLKI = 1028,
        NORWAY = 47,
        OMAN = 968,
        PAKISTAN = 92,
        PALAU = 1029,
        PANAMA = 507,
        PARAGUAY = 595,
        PERU = 51,
        PHILLIPPINES = 63,
        PITCAIRNIS = 1030,
        PNEWGUINEA = 675,
        POLAND = 48,
        PORTUGAL = 351,
        QATAR = 974,
        REUNIONI = 1031,
        ROMANIA = 40,
        RWANDA = 250,
        SAIPAN = 670,
        SANMARINO = 39,
        SAOTOME = 1033,
        SAUDIARABIA = 966,
        SENEGAL = 221,
        SEYCHELLESIS = 1034,
        SIERRALEONE = 1035,
        SINGAPORE = 65,
        SOLOMONIS = 1036,
        SOMALI = 1037,
        SOUTHAFRICA = 27,
        SPAIN = 34,
        SRILANKA = 94,
        STHELENA = 1032,
        STKITTS = 8013,
        STLUCIA = 8014,
        STPIERRE = 508,
        STVINCENT = 8015,
        SUDAN = 1038,
        SURINAME = 597,
        SWAZILAND = 268,
        SWEDEN = 46,
        SWITZERLAND = 41,
        SYRIA = 1039,
        TAIWAN = 886,
        TANZANIA = 255,
        THAILAND = 66,
        TOBAGO = 8016,
        TOGO = 228,
        TONGAIS = 676,
        TRINIDAD = 8016,
        TUNISIA = 216,
        TURKEY = 90,
        TURKSCAICOS = 8017,
        TUVALU = 1040,
        UGANDA = 256,
        USSR = 7,
        UAEMIRATES = 971,
        UNITEDKINGDOM = 44,
        USA = 1,
        URUGUAY = 598,
        VANUATU = 1041,
        VATICANCITY = 39,
        VENEZUELA = 58,
        WAKE = 1042,
        WALLISIS = 1043,
        WESTERNSAHARA = 1044,
        WESTERNSAMOA = 1045,
        YEMEN = 1046,
        YUGOSLAVIA = 38,
        ZAIRE = 243,
        ZAMBIA = 260,
        ZIMBABWE = 263,

        /* Added for 1.8 */
        ALBANIA = 355,
        ARMENIA = 374,
        AZERBAIJAN = 994,
        BELARUS = 375,
        BOSNIAHERZGO = 387,
        CAMBODIA = 855,
        CROATIA = 385,
        CZECHREPUBLIC = 420,
        DIEGOGARCIA = 246,
        ERITREA = 291,
        ESTONIA = 372,
        GEORGIA = 995,
        LATVIA = 371,
        LESOTHO = 266,
        LITHUANIA = 370,
        MACEDONIA = 389,
        MAYOTTEIS = 269,
        MOLDOVA = 373,
        MYANMAR = 95,
        NORTHKOREA = 850,
        PUERTORICO = 787,
        RUSSIA = 7,
        SERBIA = 381,
        SLOVAKIA = 421,
        SLOVENIA = 386,
        SOUTHKOREA = 82,
        UKRAINE = 380,
        USVIRGINIS = 340,
        VIETNAM = 84
    }
    #endregion Country Constants


    /****************************************************************************
    * Language Constants                                                       *
    ****************************************************************************/
    #region Language Constants
    /// <summary>
    /// TWAIN Language Codes
    /// </summary>
    public enum TWLG : short
    {
        DAN = 0,  // Danish
        DUT = 1,  // Dutch
        ENG = 2,  // International English
        FCF = 3,  // French Canadian
        FIN = 4,  // Finnish
        FRN = 5,  // French
        GER = 6,  // German
        ICE = 7,  // Icelandic
        ITN = 8,  // Italian
        NOR = 9,  // Norwegian
        POR = 10, // Portuguese
        SPA = 11, // Spanish
        SWE = 12, // Swedish
        USA = 13, // U.S. English

        /* Added for 1.8 */
        USERLOCALE = -1,
        AFRIKAANS = 14,
        ALBANIA = 15,
        ARABIC = 16,
        ARABIC_ALGERIA = 17,
        ARABIC_BAHRAIN = 18,
        ARABIC_EGYPT = 19,
        ARABIC_IRAQ = 20,
        ARABIC_JORDAN = 21,
        ARABIC_KUWAIT = 22,
        ARABIC_LEBANON = 23,
        ARABIC_LIBYA = 24,
        ARABIC_MOROCCO = 25,
        ARABIC_OMAN = 26,
        ARABIC_QATAR = 27,
        ARABIC_SAUDIARABIA = 28,
        ARABIC_SYRIA = 29,
        ARABIC_TUNISIA = 30,
        ARABIC_UAE = 31, /* United Arabic Emirates */
        ARABIC_YEMEN = 32,
        BASQUE = 33,
        BYELORUSSIAN = 34,
        BULGARIAN = 35,
        CATALAN = 36,
        CHINESE = 37,
        CHINESE_HONGKONG = 38,
        CHINESE_PRC = 39, /* People's Republic of China */
        CHINESE_SINGAPORE = 40,
        CHINESE_SIMPLIFIED = 41,
        CHINESE_TAIWAN = 42,
        CHINESE_TRADITIONAL = 43,
        CROATIA = 44,
        CZECH = 45,
        DANISH = DAN,
        DUTCH = DUT,
        DUTCH_BELGIAN = 46,
        ENGLISH = ENG,
        ENGLISH_AUSTRALIAN = 47,
        ENGLISH_CANADIAN = 48,
        ENGLISH_IRELAND = 49,
        ENGLISH_NEWZEALAND = 50,
        ENGLISH_SOUTHAFRICA = 51,
        ENGLISH_UK = 52,
        ENGLISH_USA = USA,
        ESTONIAN = 53,
        FAEROESE = 54,
        FARSI = 55,
        FINNISH = FIN,
        FRENCH = FRN,
        FRENCH_BELGIAN = 56,
        FRENCH_CANADIAN = FCF,
        FRENCH_LUXEMBOURG = 57,
        FRENCH_SWISS = 58,
        GERMAN = GER,
        GERMAN_AUSTRIAN = 59,
        GERMAN_LUXEMBOURG = 60,
        GERMAN_LIECHTENSTEIN = 61,
        GERMAN_SWISS = 62,
        GREEK = 63,
        HEBREW = 64,
        HUNGARIAN = 65,
        ICELANDIC = ICE,
        INDONESIAN = 66,
        ITALIAN = ITN,
        ITALIAN_SWISS = 67,
        JAPANESE = 68,
        KOREAN = 69,
        KOREAN_JOHAB = 70,
        LATVIAN = 71,
        LITHUANIAN = 72,
        NORWEGIAN = NOR,
        NORWEGIAN_BOKMAL = 73,
        NORWEGIAN_NYNORSK = 74,
        POLISH = 75,
        PORTUGUESE = POR,
        PORTUGUESE_BRAZIL = 76,
        ROMANIAN = 77,
        RUSSIAN = 78,
        SERBIAN_LATIN = 79,
        SLOVAK = 80,
        SLOVENIAN = 81,
        SPANISH = SPA,
        SPANISH_MEXICAN = 82,
        SPANISH_MODERN = 83,
        SWEDISH = SWE,
        THAI = 84,
        TURKISH = 85,
        UKRANIAN = 86,

        /* More stuff added for 1.8 */
        ASSAMESE = 87,
        BENGALI = 88,
        BIHARI = 89,
        BODO = 90,
        DOGRI = 91,
        GUJARATI = 92,
        HARYANVI = 93,
        HINDI = 94,
        KANNADA = 95,
        KASHMIRI = 96,
        MALAYALAM = 97,
        MARATHI = 98,
        MARWARI = 99,
        MEGHALAYAN = 100,
        MIZO = 101,
        NAGA = 102,
        ORISSI = 103,
        PUNJABI = 104,
        PUSHTU = 105,
        SERBIAN_CYRILLIC = 106,
        SIKKIMI = 107,
        SWEDISH_FINLAND = 108,
        TAMIL = 109,
        TELUGU = 110,
        TRIPURI = 111,
        URDU = 112,
        VIETNAMESE = 113
    }
    #endregion Language Constants


    /****************************************************************************
    * Data Groups                                                              *
    ****************************************************************************/
    #region Data Groups
    /* More Data Groups may be added in the future.
     * Possible candidates include text, vector graphics, sound, etc.
     * NOTE: Data Group constants must be powers of 2 as they are used
     *       as bitflags when Application asks DSM to present a list of DSs.
     */

    // Operation Triplet #1
    [Flags]
    public enum DG : uint
    {
        CONTROL = 0x0001,
        IMAGE = 0x0002,
        AUDIO = 0x0004
    }
    #endregion Data Groups


    /****************************************************************************
    * Data Argument Types                                                      *
    ****************************************************************************/
    #region Data Argument Types
    /*  SDH - 03/23/95 - WATCH                                                  */
    /*  The thunker requires knowledge about size of data being passed in the   */
    /*  lpData parameter to DS_Entry (which is not readily available due to     */
    /*  type LPVOID.  Thus, we key off the DAT_ argument to determine the size. */
    /*  This has a couple implications:                                         */
    /*  1) Any additional DAT_ features require modifications to the thunk code */
    /*     for thunker support.                                                 */
    /*  2) Any applications which use the custom capabailites are not supported */
    /*     under thunking since we have no way of knowing what size data (if    */
    /*     any) is being passed.                                                */

    // Operation Triplet #2
    public enum DAT : ushort
    {
        NULL = 0x0000,                          // No data or structure
        CUSTOMBASE = 0x8000,                    // Base of custom DATs

        // Data Argument Types for the DG_CONTROL Data Group
        CAPABILITY = 0x0001,                    // TW_CAPABILITY
        EVENT = 0x0002,                         // TW_EVENT
        IDENTITY = 0x0003,                      // TW_IDENTITY
        PARENT = 0x0004,                        // TW_HANDLE, application win handle in Windows
        PENDINGXFERS = 0x0005,                  // TW_PENDINGXFERS
        SETUPMEMXFER = 0x0006,                  // TW_SETUPMEMXFER
        SETUPFILEXFER = 0x0007,                 // TW_SETUPFILEXFER
        STATUS = 0x0008,                        // TW_STATUS
        USERINTERFACE = 0x0009,                 // TW_USERINTERFACE
        XFERGROUP = 0x000a,                     // TW_UINT32

        //  SDH - 03/21/95 - TWUNK
        //  Additional message required for thunker to request the special
        //  identity information.
        TWUNKIDENTITY = 0x000b,                 // TW_TWUNKIDENTITY
        CUSTOMDSDATA = 0x000c,                  // TW_CUSTOMDSDATA

        // Added 1.8
        DEVICEEVENT = 0x000d,                   // TW_DEVICEEVENT
        FILESYSTEM = 0x000e,                    // TW_FILESYSTEM
        PASSTHRU = 0x000f,                      // TW_PASSTHRU

        // Data Argument Types for the DG_IMAGE Data Group
        IMAGEINFO = 0x0101,                     // TW_IMAGEINFO
        IMAGELAYOUT = 0x0102,                   // TW_IMAGELAYOUT
        IMAGEMEMXFER = 0x0103,                  // TW_IMAGEMEMXFER
        IMAGENATIVEXFER = 0x0104,               // TW_UINT32 loword is hDIB, PICHandle
        IMAGEFILEXFER = 0x0105,                 // Null data
        CIECOLOR = 0x0106,                      // TW_CIECOLOR
        GRAYRESPONSE = 0x0107,                  // TW_GRAYRESPONSE
        RGBRESPONSE = 0x0108,                   // TW_RGBRESPONSE
        JPEGCOMPRESSION = 0x0109,               // TW_JPEGCOMPRESSION
        PALETTE8 = 0x010a,                      // TW_PALETTE8
        EXTIMAGEINFO = 0x010b,                  // TW_EXTIMAGEINFO -- for 1.7 Spec

        // Added 1.8
        // Data Argument Types for the DG_AUDIO Data Group
        AUDIOFILEXFER = 0x0201,                 // Null data
        AUDIOINFO = 0x0202,                     // TW_AUDIOINFO
        AUDIONATIVEXFER = 0x0203,               // TW_UINT32 handle to WAV, (AIFF Mac)

        // Added 1.9
        SETUPFILEXFER2 = 0x0301                 // New file xfer operation
    }
    #endregion Data Argument Types

    /****************************************************************************
    * Messages                                                                 *
    ****************************************************************************/
    #region Messages
    // Messages are grouped according to which DATs they are used with.

    // Operation Triplet #3
    public enum MSG : ushort
    {
        NULL = 0x0000,                          // Used in TW_EVENT structure
        CUSTOMBASE = 0x8000,                    // Base of custom messages

        // Generic messages may be used with any of several DATs
        GET = 0x0001,                           // Get one or more values
        GETCURRENT = 0x0002,                    // Get current value
        GETDEFAULT = 0x0003,                    // Get default (e.g. power up) value
        GETFIRST = 0x0004,                      // Get first of a series of items, e.g. DSs
        GETNEXT = 0x0005,                       // Iterate through a series of items
        SET = 0x0006,                           // Set one or more values
        RESET = 0x0007,                         // Set current value to default value
        QUERYSUPPORT = 0x0008,                  // Get supported operations on the cap

        // Messages used with DAT_NULL
        XFERREADY = 0x0101,                     // The data source has data ready
        CLOSEDSREQ = 0x0102,                    // Request for Application. to close DS
        CLOSEDSOK = 0x0103,                     // Tell the Application. to save the state
        // Added 1.8
        DEVICEEVENT = 0x0104,                   // Some event has taken place

        // Messages used with a pointer to a DAT_STATUS structure
        CHECKSTATUS = 0x0201,                   // Get status information

        // Messages used with a pointer to DAT_PARENT data
        OPENDSM = 0x0301,                       // Open the DSM
        CLOSEDSM = 0x0302,                      // Close the DSM

        // Messages used with a pointer to a DAT_IDENTITY structure
        OPENDS = 0x0401,                        // Open a data source
        CLOSEDS = 0x0402,                       // Close a data source
        USERSELECT = 0x0403,                    // Put up a dialog of all DS

        // Messages used with a pointer to a DAT_USERINTERFACE structure
        DISABLEDS = 0x0501,                     // Disable data transfer in the DS
        ENABLEDS = 0x0502,                      // Enable data transfer in the DS
        ENABLEDSUIONLY = 0x0503,                // Enable for saving DS state only

        // Messages used with a pointer to a DAT_EVENT structure
        PROCESSEVENT = 0x0601,

        // Messages used with a pointer to a DAT_PENDINGXFERS structure
        ENDXFER = 0x0701,
        STOPFEEDER = 0x0702,

        // Added 1.8
        // Messages used with a pointer to a DAT_FILESYSTEM structure
        CHANGEDIRECTORY = 0x0801,
        CREATEDIRECTORY = 0x0802,
        DELETE = 0x0803,
        FORMATMEDIA = 0x0804,
        GETCLOSE = 0x0805,
        GETFIRSTFILE = 0x0806,
        GETINFO = 0x0807,
        GETNEXTFILE = 0x0808,
        RENAME = 0x0809,
        COPY = 0x080A,
        AUTOMATICCAPTUREDIRECTORY = 0x080B,

        // Messages used with a pointer to a DAT_PASSTHRU structure
        PASSTHRU = 0x0901
    }
    #endregion Messages


    /****************************************************************************
    * Capabilities                                                             *
    ****************************************************************************/
    #region Capabilities
    // general capabilities
    public enum CAP : ushort
    {
        CUSTOMBASE = 0x8000,                    // Base of custom capabilities

        // all data sources are REQUIRED to support these caps
        XFERCOUNT = 0x0001,

        // all data sources MAY support these caps
        AUTHOR = 0x1000,
        CAPTION = 0x1001,
        FEEDERENABLED = 0x1002,
        FEEDERLOADED = 0x1003,
        TIMEDATE = 0x1004,
        SUPPORTEDCAPS = 0x1005,
        EXTENDEDCAPS = 0x1006,
        AUTOFEED = 0x1007,
        CLEARPAGE = 0x1008,
        FEEDPAGE = 0x1009,
        REWINDPAGE = 0x100a,
        INDICATORS = 0x100b,                    // Added 1.1
        SUPPORTEDCAPSEXT = 0x100c,              // Added 1.6
        PAPERDETECTABLE = 0x100d,               // Added 1.6
        UICONTROLLABLE = 0x100e,                // Added 1.6
        DEVICEONLINE = 0x100f,                  // Added 1.6
        AUTOSCAN = 0x1010,                      // Added 1.6
        THUMBNAILSENABLED = 0x1011,             // Added 1.7
        DUPLEX = 0x1012,                        // Added 1.7
        DUPLEXENABLED = 0x1013,                 // Added 1.7
        ENABLEDSUIONLY = 0x1014,                // Added 1.7
        CUSTOMDSDATA = 0x1015,                  // Added 1.7
        ENDORSER = 0x1016,                      // Added 1.7
        JOBCONTROL = 0x1017,                    // Added 1.7
        ALARMS = 0x1018,                        // Added 1.8
        ALARMVOLUME = 0x1019,                   // Added 1.8
        AUTOMATICCAPTURE = 0x101a,              // Added 1.8
        TIMEBEFOREFIRSTCAPTURE = 0x101b,        // Added 1.8
        TIMEBETWEENCAPTURES = 0x101c,           // Added 1.8
        CLEARBUFFERS = 0x101d,                  // Added 1.8
        MAXBATCHBUFFERS = 0x101e,               // Added 1.8
        DEVICETIMEDATE = 0x101f,                // Added 1.8
        POWERSUPPLY = 0x1020,                   // Added 1.8
        CAMERAPREVIEWUI = 0x1021,               // Added 1.8
        DEVICEEVENT = 0x1022,                   // Added 1.8
        SERIALNUMBER = 0x1024,                  // Added 1.8
        PRINTER = 0x1026,                       // Added 1.8
        PRINTERENABLED = 0x1027,                // Added 1.8
        PRINTERINDEX = 0x1028,                  // Added 1.8
        PRINTERMODE = 0x1029,                   // Added 1.8
        PRINTERSTRING = 0x102a,                 // Added 1.8
        PRINTERSUFFIX = 0x102b,                 // Added 1.8
        LANGUAGE = 0x102c,                      // Added 1.8
        FEEDERALIGNMENT = 0x102d,               // Added 1.8
        FEEDERORDER = 0x102e,                   // Added 1.8
        REACQUIREALLOWED = 0x1030,              // Added 1.8
        BATTERYMINUTES = 0x1032,                // Added 1.8
        BATTERYPERCENTAGE = 0x1033,             // Added 1.8
    }

    // image capabilities
    public enum ICAP : ushort
    {
        // image data sources are REQUIRED to support these caps
        COMPRESSION = 0x0100,
        PIXELTYPE = 0x0101,
        UNITS = 0x0102,                        // default is TWUN_INCHES
        XFERMECH = 0x0103,

        // image data sources MAY support these caps
        AUTOBRIGHT = 0x1100,
        BRIGHTNESS = 0x1101,
        CONTRAST = 0x1103,
        CUSTHALFTONE = 0x1104,
        EXPOSURETIME = 0x1105,
        FILTER = 0x1106,
        FLASHUSED = 0x1107,
        GAMMA = 0x1108,
        HALFTONES = 0x1109,
        HIGHLIGHT = 0x110a,
        IMAGEFILEFORMAT = 0x110c,
        LAMPSTATE = 0x110d,
        LIGHTSOURCE = 0x110e,
        ORIENTATION = 0x1110,
        PHYSICALWIDTH = 0x1111,
        PHYSICALHEIGHT = 0x1112,
        SHADOW = 0x1113,
        FRAMES = 0x1114,
        XNATIVERESOLUTION = 0x1116,
        YNATIVERESOLUTION = 0x1117,
        XRESOLUTION = 0x1118,
        YRESOLUTION = 0x1119,
        MAXFRAMES = 0x111a,
        TILES = 0x111b,
        BITORDER = 0x111c,
        CCITTKFACTOR = 0x111d,
        LIGHTPATH = 0x111e,
        PIXELFLAVOR = 0x111f,
        PLANARCHUNKY = 0x1120,
        ROTATION = 0x1121,
        SUPPORTEDSIZES = 0x1122,
        THRESHOLD = 0x1123,
        XSCALING = 0x1124,
        YSCALING = 0x1125,
        BITORDERCODES = 0x1126,
        PIXELFLAVORCODES = 0x1127,
        JPEGPIXELTYPE = 0x1128,
        TIMEFILL = 0x112a,
        BITDEPTH = 0x112b,
        BITDEPTHREDUCTION = 0x112c,             // Added 1.5
        UNDEFINEDIMAGESIZE = 0x112d,            // Added 1.6
        IMAGEDATASET = 0x112e,                  // Added 1.7
        EXTIMAGEINFO = 0x112f,                  // Added 1.7
        MINIMUMHEIGHT = 0x1130,                 // Added 1.7
        MINIMUMWIDTH = 0x1131,                  // Added 1.7
        FLIPROTATION = 0x1136,                  // Added 1.8
        BARCODEDETECTIONENABLED = 0x1137,       // Added 1.8
        SUPPORTEDBARCODETYPES = 0x1138,         // Added 1.8
        BARCODEMAXSEARCHPRIORITIES = 0x1139,    // Added 1.8
        BARCODESEARCHPRIORITIES = 0x113a,       // Added 1.8
        BARCODESEARCHMODE = 0x113b,             // Added 1.8
        BARCODEMAXRETRIES = 0x113c,             // Added 1.8
        BARCODETIMEOUT = 0x113d,                // Added 1.8
        ZOOMFACTOR = 0x113e,                    // Added 1.8
        PATCHCODEDETECTIONENABLED = 0x113f,     // Added 1.8
        SUPPORTEDPATCHCODETYPES = 0x1140,       // Added 1.8
        PATCHCODEMAXSEARCHPRIORITIES = 0x1141,  // Added 1.8
        PATCHCODESEARCHPRIORITIES = 0x1142,     // Added 1.8
        PATCHCODESEARCHMODE = 0x1143,           // Added 1.8
        PATCHCODEMAXRETRIES = 0x1144,           // Added 1.8
        PATCHCODETIMEOUT = 0x1145,              // Added 1.8
        FLASHUSED2 = 0x1146,                    // Added 1.8
        IMAGEFILTER = 0x1147,                   // Added 1.8
        NOISEFILTER = 0x1148,                   // Added 1.8
        OVERSCAN = 0x1149,                      // Added 1.8
        AUTOMATICBORDERDETECTION = 0x1150,      // Added 1.8
        AUTOMATICDESKEW = 0x1151,               // Added 1.8
        AUTOMATICROTATE = 0x1152,               // Added 1.8
        JPEGQUALITY = 0x1153,                   // Added 1.9
    }

    // audio capabilities
    public enum ACAP : ushort
    {
        // image data sources MAY support these audio caps
        AUDIOFILEFORMAT = 0x1201,              // Added 1.8
        XFERMECH = 0x1202                      // Added 1.8
    }
    #endregion Capabilities


    /****************************************************************************
    * Extended Image Info Attributes                                            *
    ****************************************************************************/
    #region Extended Image Info Attributes
    public enum TWEI
    {
        BARCODEX = 0x1200,
        BARCODEY = 0x1201,
        BARCODETEXT = 0x1202,
        BARCODETYPE = 0x1203,
        DESHADETOP = 0x1204,
        DESHADELEFT = 0x1205,
        DESHADEHEIGHT = 0x1206,
        DESHADEWIDTH = 0x1207,
        DESHADESIZE = 0x1208,
        SPECKLESREMOVED = 0x1209,
        HORZLINEXCOORD = 0x120A,
        HORZLINEYCOORD = 0x120B,
        HORZLINELENGTH = 0x120C,
        HORZLINETHICKNESS = 0x120D,
        VERTLINEXCOORD = 0x120E,
        VERTLINEYCOORD = 0x120F,
        VERTLINELENGTH = 0x1210,
        VERTLINETHICKNESS = 0x1211,
        PATCHCODE = 0x1212,
        ENDORSEDTEXT = 0x1213,
        FORMCONFIDENCE = 0x1214,
        FORMTEMPLATEMATCH = 0x1215,
        FORMTEMPLATEPAGEMATCH = 0x1216,
        FORMHORZDOCOFFSET = 0x1217,
        FORMVERTDOCOFFSET = 0x1218,
        BARCODECOUNT = 0x1219,
        BARCODECONFIDENCE = 0x121A,
        BARCODEROTATION = 0x121B,
        BARCODETEXTLENGTH = 0x121C,
        DESHADECOUNT = 0x121D,
        DESHADEBLACKCOUNTOLD = 0x121E,
        DESHADEBLACKCOUNTNEW = 0x121F,
        DESHADEBLACKRLMIN = 0x1220,
        DESHADEBLACKRLMAX = 0x1221,
        DESHADEWHITECOUNTOLD = 0x1222,
        DESHADEWHITECOUNTNEW = 0x1223,
        DESHADEWHITERLMIN = 0x1224,
        DESHADEWHITERLAVE = 0x1225,
        DESHADEWHITERLMAX = 0x1226,
        BLACKSPECKLESREMOVED = 0x1227,
        WHITESPECKLESREMOVED = 0x1228,
        HORZLINECOUNT = 0x1229,
        VERTLINECOUNT = 0x122A,
        DESKEWSTATUS = 0x122B,
        SKEWORIGINALANGLE = 0x122C,
        SKEWFINALANGLE = 0x122D,
        SKEWCONFIDENCE = 0x122E,
        SKEWWINDOWX1 = 0x122F,
        SKEWWINDOWY1 = 0x1230,
        SKEWWINDOWX2 = 0x1231,
        SKEWWINDOWY2 = 0x1232,
        SKEWWINDOWX3 = 0x1233,
        SKEWWINDOWY3 = 0x1234,
        SKEWWINDOWX4 = 0x1235,
        SKEWWINDOWY4 = 0x1236,
        BOOKNAME = 0x1238,          /* added 1.9 */
        CHAPTERNUMBER = 0x1239,     /* added 1.9 */
        DOCUMENTNUMBER = 0x123A,    /* added 1.9 */
        PAGENUMBER = 0x123B,        /* added 1.9 */
        CAMERA = 0x123C,            /* added 1.9 */
        FRAMENUMBER = 0x123D,       /* added 1.9 */
        FRAME = 0x123E,             /* added 1.9 */
        PIXELFLAVOR = 0x123F        /* added 1.9 */
    }

    public enum TWEJ
    {
        NONE = 0x0000,
        MIDSEPARATOR = 0x0001,
        PATCH1 = 0x0002,
        PATCH2 = 0x0003,
        PATCH3 = 0x0004,
        PATCH4 = 0x0005,
        PATCH6 = 0x0006,
        PATCHT = 0x0007
    }

    /* Added 1.8 */
    /* TW_PASSTHRU.Direction values */
    public enum TWDR
    {
        TWDR_GET = 1,
        TWDR_SET = 2
    }
    #endregion Extended Image Info Attributes


    /****************************************************************************
    * Return Codes and Condition Codes                                          *
    ****************************************************************************/
    #region Return Codes and Condition Codes
    // Return Codes: DSM_Entry and DS_Entry may return any one of these values.
    public enum TWRC : ushort
    {
        CUSTOMBASE = 0x8000,

        SUCCESS = 0,
        FAILURE = 1,           // Application may get TW_STATUS for info on failure
        CHECKSTATUS = 2,       // "tried hard"; get status
        CANCEL = 3,
        DSEVENT = 4,
        NOTDSEVENT = 5,
        XFERDONE = 6,
        ENDOFLIST = 7,         // After MSG_GETNEXT if nothing left
        INFONOTSUPPORTED = 8,
        DATANOTAVAILABLE = 9
    }

    // Condition Codes: Application gets these by doing DG_CONTROL DAT_STATUS MSG_GET.
    public enum TWCC : ushort
    {
        CUSTOMBASE = 0x8000,

        SUCCESS = 0,            // It worked!
        BUMMER = 1,             // Failure due to unknown causes
        LOWMEMORY = 2,          // Not enough memory to perform operation
        NODS = 3,               // No Data Source
        MAXCONNECTIONS = 4,     // DS is connected to max possible applications
        OPERATIONERROR = 5,     // DS or DSM reported error, application shouldn't
        BADCAP = 6,             // Unknown capability
        BADPROTOCOL = 9,        // Unrecognized MSG DG DAT combination
        BADVALUE = 10,          // Data parameter out of range
        SEQERROR = 11,          // DG DAT MSG out of expected sequence
        BADDEST = 12,           // Unknown destination Application/Source in DSM_Entry
        CAPUNSUPPORTED = 13,    // Capability not supported by source
        CAPBADOPERATION = 14,   // Operation not supported by capability
        CAPSEQERROR = 15,       // Capability has dependancy on other capability

        // Added 1.8
        DENIED = 16,            // File System operation is denied (file is protected)
        FILEEXISTS = 17,        // Operation failed because file already exists
        FILENOTFOUND = 18,      // File not found
        NOTEMPTY = 19,          // Operation failed because directory is not empty
        PAPERJAM = 20,          // The feeder is jammed
        PAPERDOUBLEFEED = 21,   // The feeder detected multiple pages
        FILEWRITEERROR = 22,    // Error writing the file (meant for things like disk full conditions)
        CHECKDEVICEONLINE = 23  // The device went offline prior to or during this operation
    }

    // bit patterns: for query operations that are supported by the data source on a capability
    // Application gets these through DG_CONTROL/DAT_CAPABILITY/MSG_QUERYSUPPORT
    // Added 1.6
    [Flags]
    internal enum TWQC : ushort
    {
        GET = 0x0001,
        SET = 0x0002,
        GETDEFAULT = 0x0004,
        GETCURRENT = 0x0008,
        RESET = 0x0010,
    }
    #endregion Return Codes and Condition Codes
}
