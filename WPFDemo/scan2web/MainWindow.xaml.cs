//////////////////////////////////////////////////////////////////
//
// Copyright (c) 2011, IBN Labs, Ltd All rights reserved.
// Please Contact rnd@ibn-labs.com 
//
// This Code is released under Code Project Open License (CPOL) 1.02,
//
// To emphesize - # Representations, Warranties and Disclaimer. THIS WORK IS PROVIDED "AS IS", "WHERE IS" AND "AS AVAILABLE", WITHOUT ANY EXPRESS OR IMPLIED WARRANTIES OR CONDITIONS OR GUARANTEES. YOU, THE USER, ASSUME ALL RISK IN ITS USE, INCLUDING COPYRIGHT INFRINGEMENT, PATENT INFRINGEMENT, SUITABILITY, ETC. AUTHOR EXPRESSLY DISCLAIMS ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES OR CONDITIONS, INCLUDING WITHOUT LIMITATION, WARRANTIES OR CONDITIONS OF MERCHANTABILITY, MERCHANTABLE QUALITY OR FITNESS FOR A PARTICULAR PURPOSE, OR ANY WARRANTY OF TITLE OR NON-INFRINGEMENT, OR THAT THE WORK (OR ANY PORTION THEREOF) IS CORRECT, USEFUL, BUG-FREE OR FREE OF VIRUSES. YOU MUST PASS THIS DISCLAIMER ON WHENEVER YOU DISTRIBUTE THE WORK OR DERIVATIVE WORKS.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

using System.IO;


namespace scan2web
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected WpfTwain TwainInterface = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region  window lifecycle

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TwainInterface = new WpfTwain();
            TwainInterface.TwainTransferReady += new TwainTransferReadyHandler(TwainWin_TwainTransferReady);
            TwainInterface.TwainCloseRequest += new TwainEventHandler(TwainUIClose);

            // a demo image was added in design mode for better WYSIWYG - we don't need it at runtime
            ClearThumbnails();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        #endregion

        #region   Twain event handlers

        private void TwainWin_TwainTransferReady(WpfTwain sender, List<ImageSource> imageSources)
        {
            UpdateScanButtons(!TwainInterface.IsScanning);
            foreach (ImageSource ims in imageSources)
                AddImageThumbnail(ims);

            // alteratively you can use imageSources[0] if the program should only support one-image scans
            this.Activate();
        }

        private void TwainUIClose(WpfTwain sender)
        {
            // scan is canceled
            UpdateScanButtons(true);
        }

        #endregion

        #region  control events handlers

        private void SelecctButton_Click(object sender, RoutedEventArgs e)
        {
            TwainInterface.Select();
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateScanButtons(false);
            TwainInterface.Acquire(false);
            // a patch to refresh the window.  (no refresh after scan usig the Canon scanner)
        }

        private void ScanUIButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateScanButtons(false);
            TwainInterface.Acquire(true);
        }

        void UpdateScanButtons(bool enabled)
        {
            SelectButton.IsEnabled = ScanButton.IsEnabled = ScanUIButton.IsEnabled = enabled;
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            UploadImage();
        }

        private void Thumbnail_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Image img = (Image)btn.Content;
            image1.Source = img.Source;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearThumbnails();
        }

        #endregion

        #region functionality

        // Add an image to the image list (thumbnails)
        public void AddImageThumbnail(ImageSource src)
        {
            Image img = new Image();
            img.Source = src;
            img.Stretch = Stretch.Uniform;
            Button btn = new Button();
            btn.Content = img;
            btn.Width = Double.NaN; // This is how you specify auto sizing behavior 
            btn.Height = Double.NaN;
            btn.Click += Thumbnail_Click;
            ThumbnailStackPanel.Children.Add(btn);
            // auto select if not set
            if (image1.Source == null)
                image1.Source = src;
        }

        public void ClearThumbnails()
        {
            ThumbnailStackPanel.Children.Clear();
            image1.Source = null;
        }

        /// <summary>
        /// Upload the current image to a web server
        /// </summary>
        public void UploadImage()
        {
            MemoryStream stream = new MemoryStream();
            try {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                // OPtional: encoding parameters (quality etc.)

                BitmapSource bs = image1.Source as BitmapSource;
                BitmapFrame bf = BitmapFrame.Create(bs);
                //encoder.Frames.Add(BitmapFrame.Create(image1.Source));
                encoder.Frames.Add(bf);
                encoder.Save(stream);
                stream.Flush();

                // upload
                scan2web.ScanServer.Scanner scanerServerProxy = new scan2web.ScanServer.Scanner();
                string result = scanerServerProxy.UploadScan(stream.GetBuffer(), "test 1");
                UploadResultLabel.Content = result;
            } catch (Exception ex) {
                UploadResultLabel.Content = "Error: " + ex.Message;
            }
            stream.Close();
            // This will come handy if we want to annotate the imageor to resize
            //RenderTargetBitmap rendered = new RenderTargetBitmap( (int)bs.Width, (int)bs.Height, bs.DpiX, bs.DpiY, bs.Format);
            //rendered.Render(image1);
        }
        #endregion

    }
}
