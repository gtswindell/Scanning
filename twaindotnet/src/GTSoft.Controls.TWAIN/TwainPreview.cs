using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwainDotNet;
using System.IO;
using TwainDotNet.WinFroms;

namespace GTSoft.Controls.TWAIN
{
    using TwainDotNet.TwainNative;

    public partial class TwainPreview : UserControl
    {
        Twain _twain;
        ScanSettings _settings;

        public TwainPreview()
        {
            InitializeComponent();
            _twain = new Twain(new WinFormsWindowMessageHook(this));
            _twain.TransferImage += delegate (Object sender, TransferImageEventArgs args)
            {
                if (args.Image != null)
                {
                    pictureBox1.Image = args.Image;
                }
            };
            _twain.ScanningComplete += delegate
            {
                Enabled = true;
            };
        }

        public bool UseADF { get; set; }
        public bool ShowGUI { get; set; }
        public bool ShowProgress { get; set; }
        public bool UseDuplex { get; set; }
        public bool UseColour { get; set; }

        public void SelectSource()
        {
            _twain.SelectSource();
        }

        public void Scan()
        {
            Enabled = false;

            _settings = new ScanSettings();
            _settings.UseDocumentFeeder = UseADF;
            _settings.ShowTwainUI = ShowGUI;
            _settings.ShowProgressIndicatorUI = ShowProgress;
            _settings.UseDuplex = UseDuplex;
            _settings.Resolution = UseColour ? ResolutionSettings.Fax : ResolutionSettings.ColourPhotocopier;
            //_settings.Area = !checkBoxArea.Checked ? null : AreaSettings;
            _settings.Area = null;
            _settings.ShouldTransferAllPages = true;

            _settings.Rotation = new RotationSettings()
            {
                AutomaticRotate = false,
                AutomaticBorderDetection = false
            };

            try
            {
                _twain.StartScanning(_settings);
            }
            catch (TwainException ex)
            {
                MessageBox.Show(ex.Message);
                Enabled = true;
            }
        }
    }
}
