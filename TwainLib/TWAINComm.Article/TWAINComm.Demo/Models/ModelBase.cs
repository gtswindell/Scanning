// http://msdn.microsoft.com/en-us/magazine/dd419663.aspx

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace TWAINComm.Demo.Models
{
    public abstract class ModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            this.OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

        protected virtual void OnPropertyChanged( string propertyName )
        {
            this.VerifyPropertyName( propertyName );

            if ( this.PropertyChanged != null )
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs( propertyName );
                this.PropertyChanged( this, e );
            }
        }

        [Conditional( "DEBUG" )]
        [DebuggerStepThrough]
        public void VerifyPropertyName( string propertyName )
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if ( TypeDescriptor.GetProperties( this )[propertyName] == null )
            {
                string msg = string.Concat( "Invalid property name: ", propertyName );
                if ( this.ThrowOnInvalidPropertyName )
                {
                    throw new Exception( msg );
                }
                else
                {
                    Debug.Fail( msg );
                }
            }
        }

        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }
    }
}
