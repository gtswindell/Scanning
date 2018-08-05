using System;

namespace TWAINComm.Demo.Models
{
    public class Identity : ModelBase
    {
        private uint id = 0;
        public uint Id
        {
            get { return id; }
            set
            {
                if ( value != id )
                {
                    id = value;
                    base.OnPropertyChanged( "Id" );
                }
            }
        }

        private Version version = new Version();
        public Version Version
        {
            get { return version; }
            set
            {
                if ( value != version )
                {
                    version = value;
                    base.OnPropertyChanged( "Version" );
                }
            }
        }

        private ushort protocolMajor = 0;
        public ushort ProtocolMajor
        {
            get { return protocolMajor; }
            set
            {
                if ( value != protocolMajor )
                {
                    protocolMajor = value;
                    base.OnPropertyChanged( "ProtocolMajor" );
                }
            }
        }

        private ushort protocolMinor = 0;
        public ushort ProtocolMinor
        {
            get { return protocolMinor; }
            set
            {
                if ( value != protocolMinor )
                {
                    protocolMinor = value;
                    base.OnPropertyChanged( "ProtocolMinor" );
                }
            }
        }

        private SupportedGroups supportedGroups = new SupportedGroups();
        public SupportedGroups SupportedGroups
        {
            get { return supportedGroups; }
            set
            {
                if ( value != supportedGroups )
                {
                    supportedGroups = value;
                    base.OnPropertyChanged( "SupportedGroups" );
                }
            }
        }

        private string manufacturer = string.Empty;
        public string Manufacturer
        {
            get { return manufacturer; }
            set
            {
                if ( value != manufacturer )
                {
                    manufacturer = value;
                    base.OnPropertyChanged( "Manufacturer" );
                }
            }
        }

        private string productFamily = string.Empty;
        public string ProductFamily
        {
            get { return productFamily; }
            set
            {
                if ( value != productFamily )
                {
                    productFamily = value;
                    base.OnPropertyChanged( "ProductFamily" );
                }
            }
        }

        private string productName = string.Empty;
        public string ProductName
        {
            get { return productName; }
            set
            {
                if ( value != productName )
                {
                    productName = value;
                    base.OnPropertyChanged( "ProductName" );
                }
            }
        }
    }
}
