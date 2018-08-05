using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using TWAINComm.Models;

namespace TWAINComm
{
    public partial class Twain : DependencyObject, IDisposable
    {
        private TW_EVENT evtmsg;
        private Extern.WINMSG winmsg;

        public Twain( Window appWindow, Feedback feedback )
            : this( appWindow, feedback, null ) { }

        public Twain( Window appWindow, Feedback feedback, TW_IDENTITY appIdentity )
        {
            // check for feedback == null
            if ( feedback == null )
            {
                feedbackDelegates = new Feedback();
            }
            else
            {
                feedbackDelegates = feedback;
            }

            // check for appWindow == null
            if ( appWindow == null )
            {
                Exception ex = new ArgumentNullException( "appWindow" );
                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }
            else
            {
                hAppWindow = ( new WindowInteropHelper( appWindow ) ).EnsureHandle();
                if ( hAppWindow == IntPtr.Zero )
                {
                    Exception ex = new Exception( "The provided handle to the application window cannot be null" );
                    if ( FeedbackDelegates.TwCommException( ex ) == false )
                    {
                        ShowMessage.Error( ex );
                    }
                }
            }

            // check for appIdentity == null
            if ( appIdentity == null )
            {
                // stick with the default app identity
                twCnn = new TwainCnn( HAppWindow );
            }
            else
            {
                // override any incorrect values that may have been provided
                appIdentity.Id = 0;
                appIdentity.ProtocolMajor = TWON_PROTOCOL.MAJOR;
                appIdentity.ProtocolMinor = TWON_PROTOCOL.MINOR;

                twCnn = new TwainCnn( appIdentity, HAppWindow );
            }

            // reserve memory for use by the TW_EVENT structure
            evtmsg.EventPtr = Marshal.AllocHGlobal( Marshal.SizeOf( winmsg ) );

            // check to make sure that the handle to the application window is valid and the external dlls are available
            if ( TwCnn.hWnd != IntPtr.Zero )
            {
                DSM_Load();
            }
        }

        ~Twain()
        {
            Dispose(); // just in case
        }

        public void Dispose()
        {
            try
            {
                UnhookWinMsgLoop();
                FreeMemory();
                if ( TwCnn.TwState != State.Two )
                {
                    DSDSM_ResetTwainState();
                }
            }
            catch { }
        }

        private void FreeMemory()
        {
            if ( evtmsg.EventPtr != IntPtr.Zero )
            {
                Marshal.FreeHGlobal( evtmsg.EventPtr );
                evtmsg.EventPtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Allows the user to select a data source
        /// </summary>
        public void SelectSource()
        {
            SelectDataSource();
        }

        // returns true if successful
        private bool HookWinMsgLoop()
        {
            if ( HAppWindow == IntPtr.Zero )
            {
                Exception ex = new Exception( "The provided handle to the application window is null" );
                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
                return false;
            }

            if ( !MsgFilter )
            {

                HwndSource hWndSrc = HwndSource.FromHwnd( HAppWindow );
                if ( hWndSrc != null )
                {
                    HwndSourceHook hWndSourceHook = new HwndSourceHook( this.WndProc );
                    hWndSrc.AddHook( hWndSourceHook );
                    MsgFilter = true;
                }
                else
                {
                    Exception ex = new Exception( "A hook into Windows messaging could not be created from the provided window handle" );
                    if ( FeedbackDelegates.TwCommException( ex ) == false )
                    {
                        ShowMessage.Error( ex );
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void UnhookWinMsgLoop()
        {
            if ( MsgFilter )
            {
                MsgFilter = false;

                if ( HAppWindow != IntPtr.Zero )
                {
                    HwndSource.FromHwnd( HAppWindow ).RemoveHook( new HwndSourceHook( WndProc ) );
                }
            }
        }

        /// <summary>
        /// Trigers a new scan operation
        /// </summary>
        public void ScanBegin()
        {
            bool success;

            DisposeImages(); // just in case

            // transition to state 5
            success = Acquire();
            if ( !success )
            {
                ScanEnd_Failure();
                return;
            }

            // hook into the win messages loop so that they can be passed to the selected DS
            success = HookWinMsgLoop();
            if ( !success )
            {
                ScanEnd_Failure();
                return;
            }
        }

        // The scan operation ended in success - close everything down
        private void ScanEnd_Success()
        {
            bool success = true;

            // device can potentially be in twain state 7, 6, or 5 at this point
            // step down to avoid unnecessary errors
            // must be in twain state 5 before disabling the DS's UI

            if ( success && TwCnn.TwState == State.Seven )
            {
                // end the image transfer and stepdown from twain state 7 to 6
                ushort pendXferCount = 0;
                success = DS_EndImageTransfer( ref pendXferCount );

                // DS will automatically transition to state 5 if count=0
                if ( success && pendXferCount == 0 )
                {
                    TwCnn.TwState = State.Five;
                }
            }

            // if count!=0 then reset the transfer, cancelling any further transfers
            if ( success && TwCnn.TwState == State.Six )
            {
                // reset the image transfer and stepdown from twain state 6 to 5
                success = DS_ResetTransfers();
            }

            if ( success && TwCnn.TwState == State.Five )
            {
                // disable the DSs UI steping down from state 5 to 4
                success = DS_Disable();
                if ( success )
                {
                    // close the DS steping down from state 4 to 3
                    success = DS_Close();
                    if ( success )
                    {
                        // close the DSM steping down from state 3 to 2
                        DSM_Close(); // can safely ignore errors here
                    }
                }
            }
            else // this should never happen, but just in case
            {
                success = false;
            }

            if ( !success )
            {
                DisposeImages();
            }

            UnhookWinMsgLoop();
            FeedbackDelegates.TwScanEnd( Images );
        }

        // The scan operation ended in failure - reset the operation
        private void ScanEnd_Failure()
        {
            UnhookWinMsgLoop();

            DisposeImages();
            FeedbackDelegates.TwScanEnd( Images );
        }

        private void DisposeImages()
        {
            for ( int i = 0; i < Images.Count; i++ )
            {
                if ( File.Exists( Images[i] ) )
                {
                    try
                    {
                        File.Delete( Images[i] );
                    }
                    catch { };
                }
            }

            Images.Clear();
        }

        private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            if ( handled )
            {
                return IntPtr.Zero;
            }

            Message winMsg = new Message()
            {
                HWnd = hwnd,
                Msg = msg,
                WParam = wParam,
                LParam = lParam
            };
            handled = PreFilterMessage( ref winMsg );

            return IntPtr.Zero;
        }

        private bool PreFilterMessage( ref Message winMsg )
        {
            bool handled;

            // keep the DS in the loop
            DSMessageReturn dsMessage = PassMessageToDS( ref winMsg );

            // check to see if the DS handled the event/message
            switch ( dsMessage.Twrc )
            {
                case TWRC.NOTDSEVENT:
                    handled = false;
                    break;
                case TWRC.DSEVENT:
                    handled = true;
                    break;
                default: // TWRC_FAILURE
                    handled = false;
                    ScanEnd_Failure();

                    Exception ex = new Exception( "Communication with the DS has failed.  The DS couldn't process an event that was passed to it." );
                    if ( FeedbackDelegates.TwCommException( ex ) == false )
                    {
                        ShowMessage.Error( ex );
                    }

                    return handled;
            }

            // check to see if the DS has a message for the application
            switch ( dsMessage.Msg )
            {
                case MSG.CLOSEDSREQ: // regular close request
                    FeedbackDelegates.TwActionChanged( "Notification from DS - Close DS Request" );
                    Debug.WriteLine( "MSG_CLOSEDSREQ" );

                    ScanEnd_Success();
                    break;
                case MSG.CLOSEDSOK: // special case for DG_CONTROL / DAT_USERINTERFACE / MSG_ENABLEDSUIONLY - must be negotiated
                    FeedbackDelegates.TwActionChanged( "Notification from DS - Close DS OK" );
                    Debug.WriteLine( "MSG_CLOSEDSOK" );

                    //GetCustomDsData(); // not implemented
                    ScanEnd_Success();
                    break;
                case MSG.DEVICEEVENT: // the use of this message must be negotiated beforehand - not implemented
                    break;
                case MSG.XFERREADY:
                    FeedbackDelegates.TwActionChanged( "Notification from DS - Transfer Ready" );
                    Debug.WriteLine( "MSG_XFERREADY" );

                    bool success;
                    ushort pendXferCount = 0;
                    IntPtr hMemDib = IntPtr.Zero;
                    do
                    {
                        FeedbackDelegates.TwActionChanged( "Attempting to transfer image" );

                        // transfer an image/page from the TWAIN device to an in-memory DIB
                        success = TransferImage( ref hMemDib, ref pendXferCount );
                        if ( !success )
                        {
                            ScanEnd_Failure();
                            FeedbackDelegates.TwActionChanged( "Attempt to transfer image failed" );

                            return handled;
                        }
                        FeedbackDelegates.TwActionChanged( "Attempt to transfer image was successful" );

                        // convert in-memory DIB to PNG file
                        string pngFile = string.Empty;
                        try
                        {
                            FeedbackDelegates.TwActionChanged( "Attempting to save image" );

                            pngFile = Imaging.DIB.ConvertToPng( hMemDib );
                            if ( string.IsNullOrEmpty( pngFile ) || !File.Exists( pngFile ) )
                            {
                                throw new Exception( "Attempt to convert the data source provided DIB to an image file failed" );
                            }

                            FeedbackDelegates.TwActionChanged( "Attempt to save image was successful" );
                        }
                        catch ( Exception ex )
                        {
                            DSDSM_ResetTwainState(); // we're exiting the program not driven by a twain error so reset the twain state before exiting
                            ScanEnd_Failure();
                            FeedbackDelegates.TwActionChanged( "Attempt to save image failed" );

                            if ( FeedbackDelegates.TwCommException( ex ) == false )
                            {
                                ShowMessage.Error( ex );
                            }

                            return handled;
                        }

                        Images.Add( pngFile );
                    } while ( pendXferCount != 0 ); // repeat if there are more images/pages to transfer

                    // when count=0 the DS automatically transitions back to state 5
                    TwCnn.TwState = State.Five;
                    FeedbackDelegates.TwActionChanged( "There are no additional images/pages to transfer" );
                    FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                    ScanEnd_Success();
                    break;
            }

            return handled;
        }

        private readonly Feedback feedbackDelegates;
        private Feedback FeedbackDelegates { get { return feedbackDelegates; } }

        private readonly List<string> images = new List<string>();
        private List<string> Images { get { return images; } }

        private readonly IntPtr hAppWindow = IntPtr.Zero;
        private IntPtr HAppWindow { get { return hAppWindow; } }

        private bool msgFilter = false;
        private bool MsgFilter
        {
            get { return msgFilter; }
            set { msgFilter = value; }
        }

        public bool TWAINInitialized
        {
            get { return TwCnn.TwState > State.One; }
        }

        private readonly Models.TwainCnn twCnn = null;
        internal Models.TwainCnn TwCnn { get { return twCnn; } }
    }
}
