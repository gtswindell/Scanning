using System;
using System.Collections.Generic;

namespace TWAINComm
{
    public class Feedback
    {
        public delegate void TwainCommExceptionDelegate( Exception ex );
        public TwainCommExceptionDelegate TwainCommException;

        public delegate void TwainStateChangedDelegate( State twState );
        public TwainStateChangedDelegate TwainStateChanged;

        public delegate void TwainActionDelegate( string action );
        public TwainActionDelegate TwainActionChanged;

        public delegate void TWAINIdentityChangedDelegate( TW_IDENTITY twIdentity );
        public TWAINIdentityChangedDelegate ApplicationIdentityChanged;
        public TWAINIdentityChangedDelegate DataSourceIdentityChanged;

        public delegate void ScanEndDelegate( List<string> pngImageFiles );
        public ScanEndDelegate ScanEnd;

        internal bool TwCommException( Exception ex )
        {
            if ( TwainCommException != null )
            {
                TwainCommException( ex );
                return true;
            }
            
            return false;
        }

        internal bool TwStateChanged( State twState )
        {
            if ( TwainStateChanged != null )
            {
                TwainStateChanged( twState );
                return true;
            }
            
            return false;
        }

        internal bool TwActionChanged( string action )
        {
            if ( TwainActionChanged != null )
            {
                TwainActionChanged( action );
                return true;
            }

            return false;
        }

        internal bool AppIdentityChanged( TW_IDENTITY appIdentity )
        {
            if ( ApplicationIdentityChanged != null )
            {
                ApplicationIdentityChanged( appIdentity );
                return true;
            }

            return false;
        }

        internal bool DsIdentityChanged( TW_IDENTITY dsIdentity )
        {
            if ( DataSourceIdentityChanged != null )
            {
                DataSourceIdentityChanged( dsIdentity );
                return true;
            }

            return false;
        }

        internal bool TwScanEnd( List<string> pngImageFiles )
        {
            if ( ScanEnd != null )
            {
                ScanEnd( pngImageFiles );
                return true;
            }

            return false;
        }
    }
}
