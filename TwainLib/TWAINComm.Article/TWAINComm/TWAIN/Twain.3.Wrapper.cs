using System;
using System.Collections.Generic;

namespace TWAINComm
{
    public partial class Twain
    {
        internal bool DSM_Load()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to load the DSM" );

                error = TwainErrorCheck( LoadDSM() );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to load the DSM failed" );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to load the DSM was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DSM_Open()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( string.Empty );
                FeedbackDelegates.TwActionChanged( "Attempting to open the DSM" );

                TWCC twcc = OpenDSM();
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );
                FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to open the DSM failed - ", twcc.ToString() ) );

                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to open the DSM was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DSM_SelectDs()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to select the DS" );

                TWCC twcc = SelectDs();
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );
                FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to select the DS failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to select the DS was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DSM_DefaultDsIdentity()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to obtain the identity of the selected DS" );

                TWCC twcc = DefaultDsIdentity();
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );
                FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );

                // check to make sure that the selected data source supports both the control and image groups
                if ( ( (DG)TwCnn.DsIdentity.SupportedGroups & DG.CONTROL ) != DG.CONTROL || ( (DG)TwCnn.DsIdentity.SupportedGroups & DG.IMAGE ) != DG.IMAGE )
                {
                    error = true;
                    Exception ex = new Exception( "Please double check the selected device.  It may not be a scanner.\n\n Device doesn't support one or both of the following support groups: Control, Image." );
                    FeedbackDelegates.TwActionChanged( ex.Message );
                    ShowMessage.Error( ex );

                    // we're exiting the program not driven by a twain error so reset the twain state before exiting
                    DSDSM_ResetTwainState();
                }

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to obtain the identity of the selected DS failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to obtain the identity of the selected DS was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DS_Open()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to open the selected DS" );

                TWCC twcc = OpenDS();
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to open the selected DS failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to open the selected DS was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DS_EnableUI()
        {
            bool error = true;
            TW_USERINTERFACE twUi = new TW_USERINTERFACE()
            {
                ShowUI = 1,
                hParent = TwCnn.hWnd
            };

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to enable the DS's UI" );

                TWCC twcc = EnableDS( twUi );
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attemp to enable the DS's UI failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to enable the DS's UI was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal TWRC DS_PassEvent( ref TW_EVENT evtmsg )
        {
            TWRC twrc = TWRC.FAILURE;

            try
            {
                twrc = PassEvent( ref evtmsg );
            }
            catch { }

            return twrc;
        }

        internal bool DS_TransferReady()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "\tAttempting to notify the DS that the transfer ready notification was received" );

                TWCC twcc = TransferReadyMsgReceived();
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "\tAttempt to notify the DS that the transfer ready notification was received failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "\tAttempt to notify the DS that the transfer ready notification was received was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DS_GetImageInfo()
        {
            bool error = true;

            try
            {
                TW_IMAGEINFO imgInfo = new TW_IMAGEINFO();

                FeedbackDelegates.TwActionChanged( "\tAttempting to acquire image info" );

                TWCC twcc = AquireImageInfo( imgInfo );
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "\tAttempt to acquire image info failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );

                    return false;
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "\tAttempt to acquire image info was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DS_BeginImageTransfer( ref IntPtr hMemDib )
        {
            bool error = true;

            try
            {
                IntPtr hImage = IntPtr.Zero;

                FeedbackDelegates.TwActionChanged( "\tAttempting to initiate image transfer" );

                TWCC twcc = GetImage_NativeTransfer( ref hImage );
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( !error )
                {
                    FeedbackDelegates.TwActionChanged( "\tAttempt to initiate image transfer was successful" );

                    hMemDib = hImage;
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "\tAttempt to initiate image transfer failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DS_EndImageTransfer( ref ushort count )
        {
            bool error = true;
            TW_PENDINGXFERS pendXfr = new TW_PENDINGXFERS()
            {
                Count = 0
            };

            try
            {
                FeedbackDelegates.TwActionChanged( "\tAttempting to acknowledge the end of the image transfer" );

                TWCC twcc = EndXfer( ref pendXfr );
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( !error )
                {
                    FeedbackDelegates.TwActionChanged( "\tAttempt to acknowledge the end of the image transfer was successful" );

                    count = pendXfr.Count;
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "\tAttempt to acknowledge the end of the image transfer failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DS_ResetTransfers()
        {
            bool error = true;
            TW_PENDINGXFERS pendXfr = new TW_PENDINGXFERS()
            {
                Count = 0
            };

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to reset the image transfer process" );

                TWCC twcc = ResetXfers( ref pendXfr );
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to reset the image transfer process failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to reset the image transfer process was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DS_Disable()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to disable the selected DS's UI" );

                TWCC twcc = DisableDS();
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to disable the selected DS's UI failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to disable the selected DS's UI was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DS_Close()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to close the selected DS" );

                TWCC twcc = CloseDS();
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );
                FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to close the selected DS failed - ", twcc.ToString() ) );

                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to close the selected DS was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DSM_Close()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to close the DSM" );

                TWCC twcc = CloseDSM();
                error = TwainErrorCheck( twcc );
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );
                FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );

                if ( error )
                {
                    FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to close the DSM failed - ", twcc.ToString() ) );
                }
                else
                {
                    FeedbackDelegates.TwActionChanged( "Attempt to close the DSM was successful" );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        // devices aren't required to support the device online capability
        private bool DS_GetDeviceOnline( out bool? capSupported, out bool? online )
        {
            bool error = true;
            capSupported = null;
            online = null;

            try
            {
                FeedbackDelegates.TwActionChanged( "Attempting to determine the status of the device online capability" );

                using ( Capability capability = new Capability( CAP.DEVICEONLINE ) )
                {
                    TWCC twcc = GetCapability( capability );
                    if ( twcc != TWCC.SUCCESS )
                    {
                        FeedbackDelegates.TwActionChanged( string.Concat( "Attempt to determine the status of the device online capability failed - ", twcc.ToString() ) );

                        // suppress the cap unsupported error message in this case
                        if ( twcc == TWCC.CAPUNSUPPORTED )
                        {
                            capSupported = false;
                            error = false; // if unsupported then let the process continue as if the check never happened
                        }
                        else
                        {
                            TwainErrorCheck( twcc );
                        }
                    }
                    else
                    {
                        capSupported = true;

                        if ( capability.ContainerType == TWON.ONE )
                        {
                            TW_ONEVALUE twOneValue = capability.GetContainerValue<TW_ONEVALUE>();
                            if ( (TWTY)twOneValue.ItemType == TWTY.BOOL )
                            {
                                online = Capability.ConvertValue<bool>( TWTY.BOOL, twOneValue.Item );
                                error = false;

                                if ( online == true )
                                {
                                    FeedbackDelegates.TwActionChanged( "\tDevice is online" );
                                }
                                else
                                {
                                    FeedbackDelegates.TwActionChanged( "\tDevice is offline" );

                                    // don't report to client so it doesn't get logged as an error, only display it
                                    ShowMessage.Error( new Exception( "The selected TWAIN device is currently offline" ) );
                                }
                            }
                        }

                        FeedbackDelegates.TwActionChanged( "Attempt to determine the status of the device online capability was successful" );
                    }
                }

                if ( error )
                {
                    FeedbackDelegates.TwStateChanged( TwCnn.TwState );
                    FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                    FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
                }
            }
            catch ( Exception ex )
            {
                error = true;

                if ( TwCnn.TwState > State.Two )
                {
                    DSDSM_ResetTwainState();
                }

                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }

        internal bool DSDSM_ResetTwainState()
        {
            bool error = true;

            try
            {
                FeedbackDelegates.TwActionChanged( "Resetting the TWAIN state" );

                ResetTwainState();
                FeedbackDelegates.TwStateChanged( TwCnn.TwState );
                FeedbackDelegates.AppIdentityChanged( TwCnn.AppIdentity );
                FeedbackDelegates.DsIdentityChanged( TwCnn.DsIdentity );
            }
            catch ( Exception ex )
            {
                error = true;
                if ( FeedbackDelegates.TwCommException( ex ) == false )
                {
                    ShowMessage.Error( ex );
                }
            }

            return !error;
        }
    }
}
