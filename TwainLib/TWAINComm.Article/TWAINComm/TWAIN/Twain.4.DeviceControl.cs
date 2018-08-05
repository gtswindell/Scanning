using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TWAINComm.Models;

namespace TWAINComm
{
    public partial class Twain
    {
        /// <summary>
        /// Checks to make sure that all external libraries that this library uses are available.  Transitions the DSM to state 2.
        /// </summary>
        /// <returns></returns>
        private TWCC LoadDSM()
        {
            if ( TwCnn.TwState > State.One )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            if ( TwCnn.hWnd == IntPtr.Zero )
            {
                return TWCC.CUSTOMBASE + 4; // application handle is null
            }

            TWCC twcc = TWCC.SUCCESS;
            try
            {
                Marshal.PrelinkAll( typeof( Extern ) );

                TwCnn.TwState = State.Two;
            }
            catch
            {
                twcc = TWCC.CUSTOMBASE + 3; // Data Source Manager could not be loaded
                TwCnn.TwState = State.One;
            }

            return twcc;
        }

        /// <summary>
        /// Open the Data Source Manager.  An action that can only be performed in state 2 that transitions the DSM to state 3.
        /// </summary>
        /// <returns>TWAIN condition code</returns>
        private TWCC OpenDSM()
        {
            // possible return codes (p198)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_LOWMEMORY
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Two )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TwCnn.AppIdentity.Id = 0; // make sure id is set to 0
            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DSM_Parent( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.PARENT, MSG.OPENDSM, ref TwCnn.hWnd );
            if ( twrc == TWRC.SUCCESS )
            {
                TwCnn.TwState = State.Three;
            }
            else // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSMStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Select a Data Source.  An action that can be performed in states 3 through 7 (usually 3).
        /// </summary>
        /// <returns>TWAIN condition code</returns>
        private TWCC SelectDs()
        {
            // possible return codes (p190)
            // TWRC_SUCCESS
            // TWRC_CANCEL
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_LOWMEMORY

            if ( TwCnn.TwState < State.Three )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            // reset if populated
            TwCnn.DsIdentity.Id = 0;
            TwCnn.DsIdentity.ProductName = string.Empty;

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DSM_DataSource( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.IDENTITY, MSG.USERSELECT, TwCnn.DsIdentity );

            if ( twrc == TWRC.CANCEL )
            {
                twcc = TWCC.CUSTOMBASE + 2; // user cancelled
                ResetTwainState();
            }
            else if ( twrc != TWRC.SUCCESS ) // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSMStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Grab the identity of the selected Data Source.  An action that can only be performed in states 3 though 7 (usually 3).
        /// </summary>
        /// <returns>TWAIN condition code</returns>
        private TWCC DefaultDsIdentity()
        {
            // possible return codes (p181)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_NODS
            // TWCC_LOWMEMORY

            if ( TwCnn.TwState < State.Three )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            // reset if populated
            TwCnn.DsIdentity.Id = 0;
            TwCnn.DsIdentity.ProductName = string.Empty;

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DSM_DataSource( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.IDENTITY, MSG.GETDEFAULT, TwCnn.DsIdentity );

            if ( twrc != TWRC.SUCCESS ) // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSMStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Open the selected Data Source.  An action that can only be performed in state 3 that transitions the DS to state 4.
        /// </summary>
        /// <returns>TWAIN condition code</returns>
        private TWCC OpenDS()
        {
            // possible return codes (p186)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_LOWMEMORY
            // TWCC_MAXCONNECTIONS
            // TWCC_NODS
            // TWCC_OPERATIONERROR

            if ( TwCnn.TwState != State.Three )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DSM_DataSource( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.IDENTITY, MSG.OPENDS, TwCnn.DsIdentity );
            if ( twrc == TWRC.SUCCESS )
            {
                TwCnn.TwState = State.Four;
            }
            else // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSMStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Get a capability from the Data Source.  An action that can be performed in states 4-7.
        /// </summary>
        /// <param name="capability">Specific capability to set</param>
        /// <returns></returns>
        private TWCC GetCapability( Capability capability )
        {
            // possible return codes (p145)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADPROTOCOL
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Four && TwCnn.TwState != State.Five && TwCnn.TwState != State.Six && TwCnn.TwState != State.Seven )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DS_Capability( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.CAPABILITY, MSG.GET, capability.TwCapability );
            if ( twrc != TWRC.SUCCESS ) // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                // don't attempt to reset the scanner if the capability is unsupported
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else if ( twcc != TWCC.CAPUNSUPPORTED )
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Set a capability in the Data Source.  An action that can only be performed in state 4.
        /// </summary>
        /// <param name="capability">Specific capability to set</param>
        /// <returns></returns>
        private TWCC SetCapability( Capability capability )
        {
            // possible return codes (p157)
            // TWRC_SUCCESS
            // TWRC_CHECKSTATUS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADCAP
            // TWCC_CAPUNSUPPORTED
            // TWCC_CAPBADOPERATION
            // TWCC_CAPSEQERROR
            // TWCC_BADDEST
            // TWCC_BADVALUE
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Four )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DS_Capability( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.CAPABILITY, MSG.SET, capability.TwCapability );
            if ( twrc == TWRC.CHECKSTATUS ) // cap couldn't be set to the provided value; retrieve the current setting
            {
                Capability checkCap = new Capability( capability.CapabilityCode );
                twcc = GetCapability( checkCap );
                if ( twcc == TWCC.SUCCESS )
                {
                    // replace the provided capability with the retrieved one
                    capability.Dispose();
                    capability = checkCap;
                }
                else
                {
                    // couldn't retrive the current setting, cleanup the new cap and return the condition code from the get operation
                    checkCap.Dispose();
                }
            }
            else if ( twrc != TWRC.SUCCESS ) // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                // don't attempt to reset the scanner if the capability is unsupported
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else if ( twcc != TWCC.CAPUNSUPPORTED )
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Enable (open) the Data Source's user interface.  An action that can only be performed in state 4 that transitions the DS to state 5.
        /// </summary>
        /// <param name="dsUi"></param>
        /// <returns>TWAIN condition code</returns>
        private TWCC EnableDS( TW_USERINTERFACE dsUi )
        {
            // possible return codes (p224)
            // TWRC_SUCCESS
            // TWRC_CHECKSTATUS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST
            // TWCC_LOWMEMORY
            // TWCC_OPERATIONERROR
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Four )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DS_UserInterface( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.USERINTERFACE, MSG.ENABLEDS, dsUi );
            if ( twrc == TWRC.SUCCESS )
            {
                TwCnn.TwState = State.Five;
            }
            else if ( twrc == TWRC.CHECKSTATUS )
            {
                // the application tried to disable the ui, but the ds doesn't support this
                // it opened the ui and transitioned to state 5
                TwCnn.TwState = State.Five;
            }
            else // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Once a Data Source has been opened the application is required to pass all messages/events to the Data Source and handle messages from the Data Souce intended for the application.  An action that can only be performed in states 5-7.
        /// </summary>
        /// <param name="evtmsg"></param>
        /// <returns>TWAIN return code</returns>
        private TWRC PassEvent( ref TW_EVENT evtmsg )
        {
            // possible return codes (p164)
            // TWRC_DSEVENT
            // TWRC_NOTDSEVENT
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST
            // TWCC_SEQERROR

            TWRC twrc = Extern.DS_DeviceEvent( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.EVENT, MSG.PROCESSEVENT, ref evtmsg );
            if ( twrc != TWRC.DSEVENT && twrc != TWRC.NOTDSEVENT ) // TWRC_FAILURE
            {
                ResetTwainState();
            }

            return twrc;
        }

        /// <summary>
        /// The Data Source has notified the program that it is ready to transfer an image.  An action that can only be performed by the DS in state 5 that transitions the DS to state 6.
        /// </summary>
        /// <returns>TWAIN condition code</returns>
        private TWCC TransferReadyMsgReceived()
        {
            // if it's already in state 6 then just exit
            if ( TwCnn.TwState == State.Six )
            {
                return TWCC.SUCCESS;
            }

            if ( TwCnn.TwState != State.Five )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TwCnn.TwState = State.Six;

            return TWCC.SUCCESS;
        }

        /// <summary>
        /// Grab the image details from the Data Source that the user specified in the DS's user interface.  An action that can only be performed in state 6.
        /// </summary>
        /// <param name="imageInfo"></param>
        /// <returns>TWAIN condition code</returns>
        private TWCC AquireImageInfo( TW_IMAGEINFO imageInfo )
        {
            // possible return codes (p238)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Six )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DS_ImageInfo( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.IMAGE, DAT.IMAGEINFO, MSG.GET, imageInfo );
            if ( twrc != TWRC.SUCCESS ) // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Tell the Data Source to initiate the image transfer and return a handle to the resulting DIB.  An action that can only be performed in state 6 that transitions the DS to state 7.
        /// </summary>
        /// <param name="hDib"></param>
        /// <returns>TWAIN condition code</returns>
        private TWCC GetImage_NativeTransfer( ref IntPtr hDib )
        {
            // possible return codes (p251)
            // TWRC_XFERDONE
            // TWRC_CANCEL
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST
            // TWCC_LOWMEMORY
            // TWCC_OPERATIONERROR
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Six )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }
            
            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DS_ImageNativeXfer( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.IMAGE, DAT.IMAGENATIVEXFER, MSG.GET, ref hDib );
            if ( twrc == TWRC.XFERDONE )
            {
                TwCnn.TwState = State.Seven;
            }
            else if ( twrc == TWRC.CANCEL )
            {
                // free the memory allocated to the DIB, if any
                if ( hDib != IntPtr.Zero )
                {
                    Extern.GlobalFree( hDib );
                    hDib = IntPtr.Zero;
                }

                twcc = TWCC.CUSTOMBASE + 2; // user cancelled
                ResetTwainState();
            }
            else //TWRC.FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Acknowlege or request the end of transfer for the image.  An action that can only be performed in state 7 that transitions the DS to state 6.
        /// </summary>
        /// <param name="pendingXfers"></param>
        /// <returns>TWAIN condition code</returns>
        private TWCC EndXfer( ref TW_PENDINGXFERS pendingXfers )
        {
            // possible return codes (p200)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Seven )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DS_PendingXfers( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.PENDINGXFERS, MSG.ENDXFER, ref pendingXfers );
            if ( twrc == TWRC.SUCCESS )
            {
                TwCnn.TwState = State.Six;
            }
            else // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Notify the Data Source to reset the image transfers.  An action that can only be performed in state 6 that transitions the DS to state 5.
        /// </summary>
        /// <param name="pendingXfers"></param>
        /// <returns>TWAIN condition code</returns>
        private TWCC ResetXfers( ref TW_PENDINGXFERS pendingXfers )
        {
            // possible return codes (p204)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Six )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DS_PendingXfers( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.PENDINGXFERS, MSG.RESET, ref pendingXfers );
            if ( twrc == TWRC.SUCCESS )
            {
                TwCnn.TwState = State.Five;
            }
            else // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Disable (close) the Data Source's user interface.  An action that can only be performed in state 5 that transitions the DS to state 4.
        /// </summary>
        /// <param name="dsUi"></param>
        /// <returns>TWAIN condition code</returns>
        private TWCC DisableDS()
        {
            // possible return codes (p223)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Five )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DS_UserInterface( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.USERINTERFACE, MSG.DISABLEDS, new TW_USERINTERFACE() );
            if ( twrc == TWRC.SUCCESS )
            {
                TwCnn.TwState = State.Four;
            }
            else // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Close a previously opened Data Source.  An action that can only be performed in state 4 that transitions the DS to state 3.
        /// </summary>
        /// <returns>TWAIN condition code</returns>
        private TWCC CloseDS()
        {
            // possible return codes (p178)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Four )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DSM_DataSource( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.IDENTITY, MSG.CLOSEDS, TwCnn.DsIdentity );
            if ( twrc == TWRC.SUCCESS )
            {
                TwCnn.DsIdentity.Id = 0;
                TwCnn.TwState = State.Three;
            }
            else // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSMStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Close the previously opened Data Source Manager.  An action that can only be performed in state 3 that transitions the DSM to state 2.
        /// </summary>
        /// <returns>TWAIN condition code</returns>
        private TWCC CloseDSM()
        {
            // possible return codes (p197)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_SEQERROR

            if ( TwCnn.TwState != State.Three )
            {
                ResetTwainState();
                return TWCC.CUSTOMBASE + 1; // application sequence error
            }

            TWCC twcc = TWCC.SUCCESS;
            TWRC twrc = Extern.DSM_Parent( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.PARENT, MSG.CLOSEDSM, ref TwCnn.hWnd );
            if ( twrc == TWRC.SUCCESS )
            {
                TwCnn.AppIdentity.Id = 0;
                TwCnn.TwState = State.Two;
            }
            else // TWRC_FAILURE
            {
                TW_STATUS twStatus = new TW_STATUS();
                twrc = DSMStatus( twStatus );
                if ( twrc == TWRC.SUCCESS )
                {
                    if ( (TWCC)twStatus.ConditionCode == TWCC.SUCCESS )
                    {
                        Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString(), "; TWCC=", ( (TWCC)twStatus.ConditionCode ).ToString() ) );
                        twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                    }
                    else
                    {
                        twcc = (TWCC)twStatus.ConditionCode;
                    }
                }
                else
                {
                    Debug.WriteLine( string.Concat( "TWRC=", twrc.ToString() ) );
                    twcc = TWCC.CUSTOMBASE + 0xFFF; // unknown error
                }

                // don't attempt to reset the scanner if it's offline
                if ( twcc == TWCC.CHECKDEVICEONLINE )
                {
                    TwCnn.DsIdentity.Id = 0;
                    TwCnn.AppIdentity.Id = 0;
                    TwCnn.TwState = State.Unknown;
                    LoadDSM();
                }
                else
                {
                    ResetTwainState();
                }
            }

            return twcc;
        }

        /// <summary>
        /// Talk to the Data Source Manager and discover it's current status throught the means of TWAIN condition codes.  An action that can only be performed in states 2-7.
        /// </summary>
        /// <param name="dsmStatus"></param>
        /// <returns>TWAIN return code</returns>
        private TWRC DSMStatus( TW_STATUS dsmStatus )
        {
            // possible return codes (p221)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST

            // don't reset twain if it fails
            TWRC twrc = Extern.DSM_Status( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.STATUS, MSG.GET, dsmStatus );

            return twrc;
        }

        /// <summary>
        /// Talk to the Data Source and discover it's current status throught the means of TWAIN condition codes.   An action that can only be performed in states 4-7.
        /// </summary>
        /// <param name="dsStatus"></param>
        /// <returns>TWAIN return code</returns>
        private TWRC DSStatus( TW_STATUS dsStatus )
        {
            // possible return codes (p221)
            // TWRC_SUCCESS
            // TWRC_FAILURE

            // possible failure codes
            // TWCC_BADDEST

            // don't reset twain if it fails
            TWRC twrc = Extern.DS_Status( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.STATUS, MSG.GET, dsStatus );

            return twrc;
        }

        /// <summary>
        /// The DS and/or DSM is in an unknown state.  Attempt to reset it back to state 2.
        /// </summary>
        private void ResetTwainState()
        {
            // assume that the scanner will either be properly reset, or that the user will have to reset the device manually
            TwCnn.TwState = State.Unknown;
            LoadDSM();

            // fall through the states, ignoring errors, and attempt to reset the DS back to state 2
            // only stop if the source stops responding
            if ( TwCnn.TwState != State.Unknown && TwCnn.AppIdentity.Id != 0 )
            {
                if ( TwCnn.DsIdentity.Id != 0 )
                {
                    TWRC twrc;
                    TW_PENDINGXFERS pendXfer = new TW_PENDINGXFERS();

                    // attempt to step down from state 7 to 6 - end transfer
                    twrc = Extern.DS_PendingXfers( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.PENDINGXFERS, MSG.ENDXFER, ref pendXfer );
                    if ( twrc == TWRC.FAILURE && DsUnresponsive() )
                    {
                        return;
                    }

                    // attempt to step down from state 6 to 5 - reset tranfers
                    twrc = Extern.DS_PendingXfers( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.PENDINGXFERS, MSG.RESET, ref pendXfer );
                    if ( twrc == TWRC.FAILURE && DsUnresponsive() )
                    {
                        return;
                    }

                    // attempt to step down from state 5 to 4 - disable ds
                    twrc = Extern.DS_UserInterface( TwCnn.AppIdentity, TwCnn.DsIdentity, DG.CONTROL, DAT.USERINTERFACE, MSG.DISABLEDS, new TW_USERINTERFACE() );
                    if ( twrc == TWRC.FAILURE && DsUnresponsive() )
                    {
                        return;
                    }

                    // attempt to step down from state 4 to 3 - close ds
                    twrc = Extern.DSM_DataSource( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.IDENTITY, MSG.CLOSEDS, TwCnn.DsIdentity );
                    if ( twrc == TWRC.FAILURE && DsmUnresponsive() )
                    {
                        return;
                    }

                    TwCnn.DsIdentity.Id = 0;
                }

                // attempt to step down from state 3 to 2 - close dsm
                Extern.DSM_Parent( TwCnn.AppIdentity, IntPtr.Zero, DG.CONTROL, DAT.PARENT, MSG.CLOSEDSM, ref TwCnn.hWnd );
                TwCnn.AppIdentity.Id = 0;
            }
        }

        // check to see if the DS/device is responsive
        private bool DsUnresponsive()
        {
            TWCC twcc;
            TW_STATUS twStatus = new TW_STATUS();

            if ( DSStatus( twStatus ) == TWRC.FAILURE )
            {
                // the source isn't responsive
                return true;
            }

            if ( Enum.TryParse<TWCC>( twStatus.ConditionCode.ToString(), out twcc ) && twcc == TWCC.CHECKDEVICEONLINE )
            {
                // the device can't respond if it's offline
                return true;
            }

            return false;
        }

        // check to see if the DSM/device is responsive
        private bool DsmUnresponsive()
        {
            TWCC twcc;
            TW_STATUS twStatus = new TW_STATUS();

            if ( DSMStatus( twStatus ) == TWRC.FAILURE )
            {
                // the source manager isn't responsive
                return true;
            }

            if ( Enum.TryParse<TWCC>( twStatus.ConditionCode.ToString(), out twcc ) && twcc == TWCC.CHECKDEVICEONLINE )
            {
                // the device can't respond if it's offline
                return true;
            }

            return false;
        }
    }
}
