// the code was initially taken from NETMaster's codeproject article ".NET TWAIN image scanner" 
// http://www.codeproject.com/Articles/1376/NET-TWAIN-image-scanner
// the license associated with the code and article was specified as public domain

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TWAINComm
{
    public partial class Twain
    {
        internal bool SelectDataSource()
        {
            bool success;

            // if the DSM isn't open then open it
            success = DSM_Open();
            if ( !success )
            {
                return false;
            }

            // select the data source
            success = DSM_SelectDs();
            if ( !success )
            {
                return false;
            }

            // close the DSM
            success = DSM_Close();
            if ( !success )
            {
                return false;
            }

            return true;
        }

        internal bool Acquire()
        {
            bool success;

            // open the dsm, if it isn't already
            success = DSM_Open();
            if ( !success )
            {
                return false;
            }

            // grab the identity of the selected ds
            success = DSM_DefaultDsIdentity();
            if ( !success )
            {
                return false;
            }

            // open the ds
            success = DS_Open();
            if ( !success )
            {
                return false;
            }

            // check to see if the device is online
            // if cap is unsupported, just continue as normal
            bool? capSupported;
            bool? deviceOnline;
            success = DS_GetDeviceOnline( out capSupported, out deviceOnline );
            if ( !success )
            {
                return false;
            }
            else if ( success && capSupported == true && deviceOnline == false )
            {
                // we're exiting the program not driven by a twain error so reset the twain state before exiting
                DSDSM_ResetTwainState();
                return false;
            }

            // enable the ds's ui
            success = DS_EnableUI();
            if ( !success )
            {
                return false;
            }

            return true;
        }

        internal bool TransferImage( ref IntPtr hMemDib, ref ushort count )
        {
            bool success;

            // the data source is ready to transfer an image - update the state
            success = DS_TransferReady();
            if ( !success )
            {
                return false;
            }

            // begin image transfer
            success = DS_BeginImageTransfer( ref hMemDib );
            if ( !success )
            {
                return false;
            }

            //end the image transfer
            success = DS_EndImageTransfer( ref count );
            if ( !success )
            {
                return false;
            }

            return true;
        }

        internal Models.DSMessageReturn PassMessageToDS( ref Message m )
        {
            Models.DSMessageReturn ret = new Models.DSMessageReturn();

            if ( TwCnn.DsIdentity.Id == 0 )
            {
                ret.Twrc = TWRC.NOTDSEVENT;
            }
            else
            {
                MSG msg;
                int time = Extern.GetMessageTime();
                int pos = Extern.GetMessagePos();

                winmsg.hwnd = m.HWnd;
                winmsg.message = m.Msg;
                winmsg.wParam = m.WParam;
                winmsg.lParam = m.LParam;
                winmsg.time = time;
                winmsg.x = (short)pos;
                winmsg.y = (short)( pos >> 16 );

                Marshal.StructureToPtr( winmsg, evtmsg.EventPtr, false );
                evtmsg.Message = 0;

                ret.Twrc = DS_PassEvent( ref evtmsg );
                if ( ret.Twrc != TWRC.FAILURE && Enum.TryParse<MSG>( evtmsg.Message.ToString(), out msg ) )
                {
                    ret.Msg = msg;
                }
            }

            return ret;
        }

        private bool TwainErrorCheck( TWCC twcc )
        {
            bool ret = false;
            string failMsg = string.Empty;

            // if the twain condition code is anything other than success, then there was a failure
            if ( twcc != TWCC.SUCCESS )
            {
                ret = true;

                if ( twcc == TWCC.OPERATIONERROR )
                {
                    FeedbackDelegates.TwActionChanged( "An operation error has occured" );
                }
            }

            // check for a failure message to display
            failMsg = FailureMessage( twcc );

            // if there's a failure message, check to see how to report it
            if ( !string.IsNullOrEmpty( failMsg ) )
            {
                Exception ex = new Exception( failMsg );
                if ( FeedbackDelegates.TwainCommException != null &&
                    // don't report the following errors to the application, just display the message
                    twcc != TWCC.PAPERJAM &&
                    twcc != TWCC.PAPERDOUBLEFEED &&
                    twcc != TWCC.CUSTOMBASE + 2 ) // user cancelled
                {
                    FeedbackDelegates.TwCommException( ex );
                }
                else
                {
                    ShowMessage.Error( ex );
                }
            }

            return ret;
        }

        private string FailureMessage( TWCC twcc )
        {
            string ret = string.Empty;

            switch ( twcc )
            {
                // v1.9 twain condition code failure messages
                case TWCC.SUCCESS:
                    break; // no failure to display
                case TWCC.BUMMER:
                    ret = "Failure due to unknown causes.\n\nEnsure that all cables are properly attached and that the device is powered on.  If the problem persists, cycle the power to the device and try again.";
                    break;
                case TWCC.LOWMEMORY:
                    ret = "Not enough memory to perform the operation";
                    break;
                case TWCC.NODS:
                    ret = "No Data Source is available";
                    break;
                case TWCC.MAXCONNECTIONS:
                    ret = "The selected data source is already connected to the maximum number of applications";
                    break;
                case TWCC.OPERATIONERROR:
                    break; // the DS or DSM has already reported the failure; the application shouldn't
                case TWCC.BADCAP:
                    ret = "The application has attempted to get or set an unknown capability";
                    break;
                case TWCC.BADPROTOCOL:
                    ret = "The application attempted to send an unrecognized DG/DAT/MSG combination to the DS";
                    break;
                case TWCC.BADVALUE:
                    ret = "The provided data parameter is out of range";
                    break;
                case TWCC.SEQERROR:
                    ret = "The operation was attempted out of its proper sequence";
                    break;
                case TWCC.BADDEST:
                    ret = "The selected source is not open";
                    break;
                case TWCC.CAPUNSUPPORTED:
                    ret = "The application has attempted to get or set a capability that is not supported by the data source";
                    break;
                case TWCC.CAPBADOPERATION:
                    ret = "The attempted operation is not supported by the specified capability";
                    break;
                case TWCC.CAPSEQERROR:
                    ret = "The attempted capability is dependent on another capability";
                    break;
                case TWCC.DENIED:
                    ret = "File system operation is denied.  The file may be write protected.";
                    break;
                case TWCC.FILEEXISTS:
                    ret = "Operation failed because file already exists";
                    break;
                case TWCC.FILENOTFOUND:
                    ret = "File not found";
                    break;
                case TWCC.NOTEMPTY:
                    ret = "Operation failed because the directory is not empty";
                    break;
                case TWCC.PAPERJAM:
                    ret = "The scanner detected a feed jam";
                    break;
                case TWCC.PAPERDOUBLEFEED:
                    ret = "The scanner detected a multi-page feed error";
                    break;
                case TWCC.FILEWRITEERROR:
                    ret = "Error writing to the file system.  The drive may be full.";
                    break;
                case TWCC.CHECKDEVICEONLINE:
                    ret = "The device went offline prior to, or during, operation";
                    break;

                // application specific failure messages
                case TWCC.CUSTOMBASE + 1:
                    ret = "The operation was attempted out of its proper application sequence";
                    break;
                case TWCC.CUSTOMBASE + 2:
                    ret = "Operation was cancelled";
                    break;
                case TWCC.CUSTOMBASE + 3:
                    ret = "One, or more, of the required external DLLs could not be loaded.  Make sure that the TWAIN driver for your device has been installed.";
                    break;
                case TWCC.CUSTOMBASE + 4:
                    ret = "The handle to the application window that was provided is set to null";
                    break;

                // ???
                default:
                    ret = "An unknown error has occured";
                    break;
            }

            return ret;
        }
    }
}
