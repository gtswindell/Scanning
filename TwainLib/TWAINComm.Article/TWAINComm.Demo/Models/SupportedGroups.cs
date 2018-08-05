using System;

namespace TWAINComm.Demo.Models
{
    public class SupportedGroups : ModelBase
    {
        private bool? control = null;
        public bool? Control
        {
            get { return control; }
            set
            {
                if ( value != control )
                {
                    control = value;
                    base.OnPropertyChanged( "Control" );
                }
            }
        }

        private bool? image = null;
        public bool? Image
        {
            get { return image; }
            set
            {
                if ( value != image )
                {
                    image = value;
                    base.OnPropertyChanged( "Image" );
                }
            }
        }

        private bool? audio = null;
        public bool? Audio
        {
            get { return audio; }
            set
            {
                if ( value != audio )
                {
                    audio = value;
                    base.OnPropertyChanged( "Audio" );
                }
            }
        }
    }
}
