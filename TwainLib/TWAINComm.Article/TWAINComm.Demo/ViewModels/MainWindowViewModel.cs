using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;

namespace TWAINComm.Demo.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public delegate void ScrollToBottomDelegate();
        public ScrollToBottomDelegate ScrollToBottom;

        #region ctor/dtor
        public MainWindowViewModel()
        {
            string imageSavePath = Properties.Settings.Default.ImageSavePath;
            if ( !string.IsNullOrEmpty( imageSavePath ) && Directory.Exists( imageSavePath ) )
            {
                ImageSavePath = imageSavePath;
            }
            else
            {
                ImageSavePath = System.IO.Path.Combine( Environment.ExpandEnvironmentVariables( "%userprofile%" ), "Documents" );
            }
        }
        #endregion ctor/dtor


        #region Private Methods
        private void ViewLoaded()
        {
            // need to setup the Twain property after the window has been created, so that the handle to the window won't be null
            TWAINComm.TW_IDENTITY applicationIdentity = new TWAINComm.TW_IDENTITY()
            {
                Id = 0,
                Version = new TWAINComm.TW_VERSION()
                {
                    MajorNum = 1,
                    MinorNum = 0,
                    Language = (ushort)TWAINComm.TWLG.ENGLISH_USA,
                    Country = (ushort)TWAINComm.TWCY.USA,
                    Info = "v1.0"
                },
                SupportedGroups = (uint)( TWAINComm.DG.CONTROL | TWAINComm.DG.IMAGE ),
                Manufacturer = "Demo Manufacturer",
                ProductFamily = "TWAINComm",
                ProductName = "TWAINComm Demo App"
            };

            TWAINComm.Feedback feedback = new TWAINComm.Feedback();
            feedback.ScanEnd += Twain_ScanEnd;
            feedback.ApplicationIdentityChanged += Twain_ApplicationIdentityChanged;
            feedback.DataSourceIdentityChanged += Twain_DataSourceIdentityChanged;
            feedback.TwainStateChanged += Twain_TwainStateChanged;
            feedback.TwainActionChanged += Twain_TwainActionChanged;
            feedback.TwainCommException += Twain_TwainCommException;

            Twain = new TWAINComm.Twain( App.Current.MainWindow, feedback, applicationIdentity );

            // force re-evaluation of the TwainInitialized property now that the Twain property is no longer null
            CommandManager.InvalidateRequerySuggested();
        }

        private void ViewClosed()
        {
            try
            {
                if ( Twain != null )
                {
                    Twain.Dispose();
                    Twain = null;
                }
            }
            catch { }
        }

        // requires the Windows® API Code Pack for Microsoft® .NET Framework 
        // http://archive.msdn.microsoft.com/WindowsAPICodePack/Release/ProjectReleases.aspx?ReleaseId=4906
        private void ChangeImageSavePath()
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog()
            {
                Title = "Select the image save folder",
                IsFolderPicker = true,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if ( !string.IsNullOrEmpty( ImageSavePath ) )
            {
                openFileDialog.InitialDirectory = ImageSavePath;
            }

            if ( openFileDialog.ShowDialog() == CommonFileDialogResult.Ok )
            {
                ImageSavePath = openFileDialog.FileName;
            }
        }

        private void NewScan()
        {
            IsScanning = true;
            Twain.ScanBegin();
            //Twain.
        }

        private void Twain_ScanEnd( List<string> pngImageFiles )
        {
            if ( pngImageFiles != null )
            {
                for ( int i = 0; i < pngImageFiles.Count; i++ )
                {
                    MoveImage( pngImageFiles[i] );
                }
            }

            IsScanning = false;
        }

        private void MoveImage( string pngImageFile )
        {
            const string fileNamePrefix = "Image.";
            const string fileNameNumberFormat = "000";
            const string fileNameSuffix = ".png";

            if ( string.IsNullOrEmpty( pngImageFile ) || !File.Exists( pngImageFile ) )
            {
                return;
            }

            int imageNum = 0;
            string newFileName = string.Empty;
            string newFilePath = string.Empty;
            do
            {
                imageNum++;
                newFileName = string.Concat( fileNamePrefix, imageNum.ToString( fileNameNumberFormat ), fileNameSuffix );
                newFilePath = Path.Combine( ImageSavePath, newFileName );

            } while ( File.Exists( newFilePath ) );

            File.Move( pngImageFile, newFilePath );
        }

        private bool TwainAvailable()
        {
            return
                !IsScanning &&
                TwainInitialized;
        }
        #endregion Private Methods


        #region Feedback Delegate Callbacks
        private void Twain_ApplicationIdentityChanged( TWAINComm.TW_IDENTITY applicationIdentity )
        {
            ApplicationIdentity.Id = applicationIdentity.Id;
            ApplicationIdentity.Version.MajorNum = applicationIdentity.Version.MajorNum;
            ApplicationIdentity.Version.MinorNum = applicationIdentity.Version.MinorNum;
            ApplicationIdentity.Version.Language = (TWAINComm.TWLG)applicationIdentity.Version.Language;
            ApplicationIdentity.Version.Country = (TWAINComm.TWCY)applicationIdentity.Version.Country;
            ApplicationIdentity.Version.Info = applicationIdentity.Version.Info;

            ApplicationIdentity.ProtocolMajor = applicationIdentity.ProtocolMajor;
            ApplicationIdentity.ProtocolMinor = applicationIdentity.ProtocolMinor;

            TWAINComm.DG supportedGroups = (TWAINComm.DG)applicationIdentity.SupportedGroups;
            if ( ( supportedGroups & TWAINComm.DG.CONTROL ) == TWAINComm.DG.CONTROL )
            {
                ApplicationIdentity.SupportedGroups.Control = true;
            }
            else
            {
                ApplicationIdentity.SupportedGroups.Control = false;
            }

            if ( ( supportedGroups & TWAINComm.DG.IMAGE ) == TWAINComm.DG.IMAGE )
            {
                ApplicationIdentity.SupportedGroups.Image = true;
            }
            else
            {
                ApplicationIdentity.SupportedGroups.Image = false;
            }

            if ( ( supportedGroups & TWAINComm.DG.AUDIO ) == TWAINComm.DG.AUDIO )
            {
                ApplicationIdentity.SupportedGroups.Audio = true;
            }
            else
            {
                ApplicationIdentity.SupportedGroups.Audio = false;
            }

            ApplicationIdentity.Manufacturer = applicationIdentity.Manufacturer;
            ApplicationIdentity.ProductFamily = applicationIdentity.ProductFamily;
            ApplicationIdentity.ProductName = applicationIdentity.ProductName;
        }

        private void Twain_DataSourceIdentityChanged( TWAINComm.TW_IDENTITY dataSourceIdentity )
        {
            DataSourceIdentity.Id = dataSourceIdentity.Id;
            DataSourceIdentity.Version.MajorNum = dataSourceIdentity.Version.MajorNum;
            DataSourceIdentity.Version.MinorNum = dataSourceIdentity.Version.MinorNum;
            DataSourceIdentity.Version.Language = (TWAINComm.TWLG)dataSourceIdentity.Version.Language;
            DataSourceIdentity.Version.Country = (TWAINComm.TWCY)dataSourceIdentity.Version.Country;
            DataSourceIdentity.Version.Info = dataSourceIdentity.Version.Info;

            DataSourceIdentity.ProtocolMajor = dataSourceIdentity.ProtocolMajor;
            DataSourceIdentity.ProtocolMinor = dataSourceIdentity.ProtocolMinor;

            TWAINComm.DG supportedGroups = (TWAINComm.DG)dataSourceIdentity.SupportedGroups;
            if ( ( supportedGroups & TWAINComm.DG.CONTROL ) == TWAINComm.DG.CONTROL )
            {
                DataSourceIdentity.SupportedGroups.Control = true;
            }
            else
            {
                DataSourceIdentity.SupportedGroups.Control = false;
            }

            if ( ( supportedGroups & TWAINComm.DG.IMAGE ) == TWAINComm.DG.IMAGE )
            {
                DataSourceIdentity.SupportedGroups.Image = true;
            }
            else
            {
                DataSourceIdentity.SupportedGroups.Image = false;
            }

            if ( ( supportedGroups & TWAINComm.DG.AUDIO ) == TWAINComm.DG.AUDIO )
            {
                DataSourceIdentity.SupportedGroups.Audio = true;
            }
            else
            {
                DataSourceIdentity.SupportedGroups.Audio = false;
            }

            DataSourceIdentity.Manufacturer = dataSourceIdentity.Manufacturer;
            DataSourceIdentity.ProductFamily = dataSourceIdentity.ProductFamily;
            DataSourceIdentity.ProductName = dataSourceIdentity.ProductName;
        }

        private void Twain_TwainStateChanged( TWAINComm.State twState )
        {
            TwainState = twState;

            string action = string.Empty;
            if ( TwainState >= TWAINComm.State.Six )
            {
                action = string.Concat( "\t*TWAIN State: ", twainState.ToString() );
            }
            else
            {
                action = string.Concat( "*TWAIN State: ", twainState.ToString() );
            }
            Twain_TwainActionChanged( action );
        }

        private void Twain_TwainActionChanged( string action )
        {
            if ( string.IsNullOrEmpty( action ) )
            {
                TwainActionMessages = string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder( TwainActionMessages );

                sb.AppendLine( action );
                TwainActionMessages = sb.ToString();
                sb.Clear();

                if ( ScrollToBottom != null )
                {
                    ScrollToBottom();
                }
            }
        }

        private void Twain_TwainCommException( Exception ex )
        {
            if ( ex != null )
            {
                StringBuilder sb = new StringBuilder( TwainActionMessages );

                sb.AppendLine( string.Concat( "\n**ERROR MESSAGE**\n", ex.Message.Trim() ) );
                sb.AppendLine( string.Concat( "\n**STACK TRACE**\n", ex.StackTrace.Trim(), "\n" ) );

                TwainActionMessages = sb.ToString();
                sb.Clear();

                if ( ScrollToBottom != null )
                {
                    ScrollToBottom();
                }

                MessageBox.Show( ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
            }
        }
        #endregion Feedback Delegate Callbacks


        #region Commands
        private RelayCommand selectDeviceCommand = null;
        public RelayCommand SelectDeviceCommand
        {
            get
            {
                if ( selectDeviceCommand == null )
                {
                    selectDeviceCommand = new RelayCommand( ( obj ) => Twain.SelectSource(), ( obj ) => TwainAvailable() );
                }

                return selectDeviceCommand;
            }
        }

        private RelayCommand newScanCommand = null;
        public RelayCommand NewScanCommand
        {
            get
            {
                if ( newScanCommand == null )
                {
                    newScanCommand = new RelayCommand( ( obj ) => NewScan(), ( obj ) => TwainAvailable() );
                }

                return newScanCommand;
            }
        }

        private RelayCommand imageSavePathCommand = null;
        public RelayCommand ImageSavePathCommand
        {
            get
            {
                if ( imageSavePathCommand == null )
                {
                    imageSavePathCommand = new RelayCommand( ( obj ) => ChangeImageSavePath(), ( obj ) => TwainAvailable() );
                }

                return imageSavePathCommand;
            }
        }

        private RelayCommand viewLoadedCommand = null;
        public RelayCommand ViewLoadedCommand
        {
            get
            {
                if ( viewLoadedCommand == null )
                {
                    viewLoadedCommand = new RelayCommand( ( obj ) => ViewLoaded() );
                }

                return viewLoadedCommand;
            }
        }

        private RelayCommand viewClosedCommand = null;
        public RelayCommand ViewClosedCommand
        {
            get
            {
                if ( viewClosedCommand == null )
                {
                    viewClosedCommand = new RelayCommand( ( obj ) => ViewClosed() );
                }

                return viewClosedCommand;
            }
        }
        #endregion Commands


        #region Properties
        private TWAINComm.Twain twain = null;
        private TWAINComm.Twain Twain
        {
            get { return twain; }
            set { twain = value; }
        }

        private bool isScanning = false;
        public bool IsScanning
        {
            get { return isScanning; }
            set { isScanning = value; }
        }

        private bool TwainInitialized
        {
            get
            {
                if ( Twain != null && Twain.TWAINInitialized )
                {
                    return true;
                }

                return false;
            }
        }

        private string imageSavePath = string.Empty;
        public string ImageSavePath
        {
            get { return imageSavePath; }
            set
            {
                if ( value != imageSavePath && Directory.Exists( value ) )
                {
                    if ( value != Properties.Settings.Default.ImageSavePath )
                    {
                        Properties.Settings.Default.ImageSavePath = value;
                        Properties.Settings.Default.Save();
                    }

                    imageSavePath = value;
                    base.OnPropertyChanged( "ImageSavePath" );
                }
            }
        }

        private Models.Identity applicationIdentity = new Models.Identity();
        public Models.Identity ApplicationIdentity
        {
            get { return applicationIdentity; }
            set
            {
                if ( value != applicationIdentity )
                {
                    applicationIdentity = value;
                    base.OnPropertyChanged( "ApplicationIdentity" );
                }
            }
        }

        private Models.Identity dataSourceIdentity = new Models.Identity();
        public Models.Identity DataSourceIdentity
        {
            get { return dataSourceIdentity; }
            set
            {
                if ( value != dataSourceIdentity )
                {
                    dataSourceIdentity = value;
                    base.OnPropertyChanged( "DataSourceIdentity" );
                }
            }
        }

        private TWAINComm.State twainState = TWAINComm.State.Unknown;
        public TWAINComm.State TwainState
        {
            get { return twainState; }
            private set
            {
                if ( value != twainState )
                {
                    twainState = value;
                    base.OnPropertyChanged( "TwainState" );
                }
            }
        }

        private string twainActionMessages = string.Empty;
        public string TwainActionMessages
        {
            get { return twainActionMessages; }
            private set
            {
                if ( value != twainActionMessages )
                {
                    twainActionMessages = value;
                    base.OnPropertyChanged( "TwainActionMessages" );
                }
            }
        }
        #endregion Properties
    }
}
