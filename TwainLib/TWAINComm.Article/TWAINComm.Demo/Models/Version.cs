using System;

namespace TWAINComm.Demo.Models
{
    public class Version : ModelBase
    {
        private ushort majorNum = 0;
        public ushort MajorNum
        {
            get { return majorNum; }
            set
            {
                if ( value != majorNum )
                {
                    majorNum = value;
                    base.OnPropertyChanged( "MajorNum" );
                }
            }
        }

        private ushort minorNum = 0;
        public ushort MinorNum
        {
            get { return minorNum; }
            set
            {
                if ( value != minorNum )
                {
                    minorNum = value;
                    base.OnPropertyChanged( "MinorNum" );
                }
            }
        }

        private TWAINComm.TWLG language = TWAINComm.TWLG.ENGLISH_USA;
        public TWAINComm.TWLG Language
        {
            get { return language; }
            set
            {
                if ( value != language )
                {
                    language = value;
                    base.OnPropertyChanged( "Language" );
                }
            }
        }

        private TWAINComm.TWCY country = TWAINComm.TWCY.USA;
        public TWAINComm.TWCY Country
        {
            get { return country; }
            set
            {
                if ( value != country )
                {
                    country = value;
                    base.OnPropertyChanged( "Country" );
                }
            }
        }

        private string info;
        public string Info
        {
            get { return info; }
            set
            {
                if ( value != info )
                {
                    info = value;
                    base.OnPropertyChanged( "Info" );
                }
            }
        }
    }
}
