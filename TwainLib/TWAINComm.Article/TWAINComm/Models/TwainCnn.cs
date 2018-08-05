using System;
using System.Diagnostics;

namespace TWAINComm.Models
{
    internal class TwainCnn
    {
        public IntPtr hWnd = IntPtr.Zero;

        public TwainCnn( IntPtr hWnd )
        {
            this.hWnd = hWnd;
        }

        public TwainCnn( TW_IDENTITY applicationIdentity, IntPtr hWnd )
        {
            this.appIdentity = applicationIdentity;
            this.hWnd = hWnd;
        }

        // TWAINComm library defaults
        private TW_IDENTITY appIdentity = new TW_IDENTITY()
        {
            Id = 0,
            Version = new TW_VERSION()
            {
                MajorNum = 1,
                MinorNum = 0,
                Language = (ushort)TWLG.ENGLISH_USA,
                Country = (ushort)TWCY.USA,
                Info = "1.0"
            },

            ProtocolMajor = TWON_PROTOCOL.MAJOR,
            ProtocolMinor = TWON_PROTOCOL.MINOR,
            SupportedGroups = (uint)( DG.IMAGE | DG.CONTROL ),

            Manufacturer = string.Empty,
            ProductFamily = "C# TWAIN",
            ProductName = "TWAINComm"
        };
        public TW_IDENTITY AppIdentity
        {
            get { return appIdentity; }
            set { appIdentity = value; }
        }

        private TW_IDENTITY dsIdentity = new TW_IDENTITY()
        {
            Id = 0
        };
        public TW_IDENTITY DsIdentity
        {
            get { return dsIdentity; }
        }

        private State twState = State.One;
        public State TwState
        {
            get { return twState; }
            set
            {
                twState = value;
                Debug.WriteLine( string.Concat( "TWAIN State: ", twState ) );
            }
        }
    }
}
