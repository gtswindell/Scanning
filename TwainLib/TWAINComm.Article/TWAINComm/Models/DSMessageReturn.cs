namespace TWAINComm.Models
{
    internal class DSMessageReturn
    {
        private TWRC twrc = TWRC.FAILURE;
        public TWRC Twrc
        {
            get { return twrc; }
            set { twrc = value; }
        }

        private MSG msg = MSG.NULL;
        public MSG Msg
        {
            get { return msg; }
            set { msg = value; }
        }
    }
}
