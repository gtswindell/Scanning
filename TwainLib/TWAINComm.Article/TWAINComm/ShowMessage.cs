using System;
using System.Windows;

namespace TWAINComm
{
    internal static class ShowMessage
    {
        internal static void Info( string msg, string caption )
        {
            MessageBox.Show( msg, caption, MessageBoxButton.OK, MessageBoxImage.Information );
        }

        internal static void Warn( string msg )
        {
            MessageBox.Show( msg, "TWAIN Warning", MessageBoxButton.OK, MessageBoxImage.Warning );
        }

        internal static void Error( string msg )
        {
            MessageBox.Show( msg, "TWAIN Error", MessageBoxButton.OK, MessageBoxImage.Error );
        }

        internal static void Error( Exception ex )
        {
            //Error( string.Concat( ex.Message, "\n\n", ex.StackTrace ) );
            Error( ex.Message );
        }
    }
}
