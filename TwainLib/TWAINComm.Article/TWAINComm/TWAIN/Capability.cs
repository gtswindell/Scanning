using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TWAINComm
{
    public class Capability : IDisposable
    {
        public Capability( CAP cap )
        {
            TwCapability.Cap = (ushort)cap;
        }

        ~Capability()
        {
            Dispose();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public void Dispose()
        {
            if ( TwCapability.hContainer != IntPtr.Zero )
            {
                Extern.GlobalFree( TwCapability.hContainer );
                TwCapability.hContainer = IntPtr.Zero;
            }
        }

        public object GetContainerValue()
        {
            if ( TwCapability.hContainer == IntPtr.Zero )
            {
                throw new Exception( "Attempted to retrieve a TWAIN capability container value without retrieving the data from the data source first" );
            }

            if ( ContainerValue == null )
            {
                CopyContainerToValue();
            }

            return ContainerValue;
        }

        public T GetContainerValue<T>() where T : class
        {
            object value = GetContainerValue();
            if ( !( value is T ) )
            {
                throw new Exception( "Attempted to retrieve a TWAIN capability container value using the wrong container type" );
            }

            return (T)value;
        }

        public void SetContainerValue( TW_ONEVALUE twOneValue )
        {
            ContainerType = TWON.ONE;
            ContainerValue = twOneValue;

            CopyValueToContainer( ContainerValue );
        }

        public void SetContainerValue( TW_RANGE twRange )
        {
            ContainerType = TWON.RANGE;
            ContainerValue = twRange;

            CopyValueToContainer( ContainerValue );
        }

        public void SetContainerValue( TW_ENUMERATION twEnum )
        {
            ContainerType = TWON.ENUM;
            ContainerValue = twEnum;

            CopyValueToContainer( ContainerValue );
        }

        public void SetContainerValue( TW_ARRAY twArray )
        {
            ContainerType = TWON.ARRAY;
            ContainerValue = twArray;

            CopyValueToContainer( ContainerValue );
        }

        public void SetContainerItem( uint item )
        {
            switch ( ContainerType )
            {
                case TWON.ONE:
                    {
                        TW_ONEVALUE twOneValue = ContainerValue as TW_ONEVALUE;
                        twOneValue.Item = item;

                        CopyValueToContainer( ContainerValue );
                        break;
                    }
                case TWON.RANGE:
                    {
                        TW_RANGE twRange = ContainerValue as TW_RANGE;

                        // check the item to make sure it's a valid number
                        if ( item >= twRange.MinValue && item <= twRange.MaxValue && ( item - twRange.MinValue ) % twRange.StepSize == 0 )
                        {
                            twRange.CurrentValue = item;
                        }
                        else
                        {
                            throw new Exception( "" );
                        }

                        CopyValueToContainer( ContainerValue );
                        break;
                    }
                case TWON.ENUM:
                    {
                        TW_ENUMERATION twEnum = ContainerValue as TW_ENUMERATION;

                        // check the item to make sure it's a valid number
                        if ( item >= 0 && item < twEnum.NumItems )
                        {
                            twEnum.CurrentIndex = item;
                        }
                        else
                        {
                            throw new Exception( "An attempt was made to set the current index on a TWAIN enumeration capability container item that was out of range" );
                        }

                        CopyValueToContainer( ContainerValue );
                        break;
                    }
                case TWON.ARRAY:
                    {
                        throw new Exception( "An attempt was made to set the item value on a TWAIN array capability container" );
                    }
                default:
                    {
                        throw new Exception( "An unknown TWON value has been set in the capability class" ); // this is impossible
                    }
            }
        }

        private void CopyContainerToValue()
        {
            const string GLOBAL_LOCK_FAILURE = "An ettempt to initiate a global lock failed while converting a DS capability container\nError code ";

            switch ( ContainerType )
            {
                case TWON.ONE:
                    {
                        Exception exception = null;
                        TW_ONEVALUE convertedCon = new TW_ONEVALUE();

                        if ( TwCapability.hContainer == IntPtr.Zero )
                        {
                            throw new Exception( "There was an attempt to copy the TWAIN capability container to value while the container was set to null" );
                        }

                        IntPtr pv = Extern.GlobalLock( TwCapability.hContainer );
                        if ( pv == IntPtr.Zero )
                        {
                            throw new Exception( string.Concat( GLOBAL_LOCK_FAILURE, Marshal.GetLastWin32Error().ToString() ) );
                        }
                        else
                        {
                            try
                            {
                                Marshal.PtrToStructure( pv, convertedCon );
                            }
                            catch ( Exception ex )
                            {
                                exception = ex;
                            }
                            finally
                            {
                                Extern.GlobalUnlock( TwCapability.hContainer );
                            }
                        }
                        Extern.GlobalFree( TwCapability.hContainer );
                        TwCapability.hContainer = IntPtr.Zero;

                        if ( exception != null )
                        {
                            throw exception;
                        }

                        ContainerValue = convertedCon;
                        break;
                    }
                case TWON.RANGE:
                    {
                        Exception exception = null;
                        TW_RANGE convertedCon = new TW_RANGE();

                        if ( TwCapability.hContainer == IntPtr.Zero )
                        {
                            throw new Exception( "There was an attempt to copy the TWAIN capability container to value while the container was set to null" );
                        }

                        IntPtr pv = Extern.GlobalLock( TwCapability.hContainer );
                        if ( pv == IntPtr.Zero )
                        {
                            throw new Exception( string.Concat( GLOBAL_LOCK_FAILURE, Marshal.GetLastWin32Error().ToString() ) );
                        }
                        else
                        {
                            try
                            {
                                Marshal.PtrToStructure( pv, convertedCon );
                            }
                            catch ( Exception ex )
                            {
                                exception = ex;
                            }
                            finally
                            {
                                Extern.GlobalUnlock( TwCapability.hContainer );
                            }
                        }
                        Extern.GlobalFree( TwCapability.hContainer );
                        TwCapability.hContainer = IntPtr.Zero;

                        if ( exception != null )
                        {
                            throw exception;
                        }

                        ContainerValue = convertedCon;
                        break;
                    }
                case TWON.ENUM:
                    {
                        Exception exception = null;
                        TW_ENUMERATION convertedCon = new TW_ENUMERATION();

                        if ( TwCapability.hContainer == IntPtr.Zero )
                        {
                            throw new Exception( "There was an attempt to copy the TWAIN capability container to value while the container was set to null" );
                        }

                        IntPtr pv = Extern.GlobalLock( TwCapability.hContainer );
                        if ( pv == IntPtr.Zero )
                        {
                            throw new Exception( string.Concat( GLOBAL_LOCK_FAILURE, Marshal.GetLastWin32Error().ToString() ) );
                        }
                        else
                        {
                            try
                            {
                                // get the item type
                                convertedCon.ItemType = (ushort)Marshal.ReadInt16( pv );

                                // get the number of elements in the byte array
                                convertedCon.NumItems = (uint)Marshal.ReadInt32( pv, 2 );
                                if ( convertedCon.NumItems > 0 )
                                {
                                    convertedCon.ItemList = new byte[convertedCon.NumItems * SizeOfTwainType( (TWTY)convertedCon.ItemType )];
                                }

                                // get the current index
                                convertedCon.CurrentIndex = (uint)Marshal.ReadInt32( pv, 6 );

                                // get the default index
                                convertedCon.CurrentIndex = (uint)Marshal.ReadInt32( pv, 10 );

                                // get the individual byes in the array
                                for ( int i = 0; i < convertedCon.ItemList.Length; i++ )
                                {
                                    convertedCon.ItemList[i] = Marshal.ReadByte( pv, 14 + i );
                                }
                            }
                            catch ( Exception ex )
                            {
                                exception = ex;
                            }
                            finally
                            {
                                Extern.GlobalUnlock( TwCapability.hContainer );
                            }
                        }
                        Extern.GlobalFree( TwCapability.hContainer );
                        TwCapability.hContainer = IntPtr.Zero;

                        if ( exception != null )
                        {
                            throw exception;
                        }

                        ContainerValue = convertedCon;
                        break;
                    }
                case TWON.ARRAY:
                    {
                        Exception exception = null;
                        TW_ARRAY convertedCon = new TW_ARRAY();

                        if ( TwCapability.hContainer == IntPtr.Zero )
                        {
                            throw new Exception( "There was an attempt to copy the TWAIN capability container to value while the container was set to null" );
                        }

                        IntPtr pv = Extern.GlobalLock( TwCapability.hContainer );
                        if ( pv == IntPtr.Zero )
                        {
                            throw new Exception( string.Concat( GLOBAL_LOCK_FAILURE, Marshal.GetLastWin32Error().ToString() ) );
                        }
                        else
                        {
                            try
                            {
                                // get the item type
                                convertedCon.ItemType = (ushort)Marshal.ReadInt16( pv );

                                // get the number of elements in the byte array
                                convertedCon.NumItems = (uint)Marshal.ReadInt32( pv, 2 );
                                if ( convertedCon.NumItems > 0 )
                                {
                                    convertedCon.ItemList = new byte[convertedCon.NumItems * SizeOfTwainType( (TWTY)convertedCon.ItemType )];
                                }

                                // get the individual byes in the array
                                for ( int i = 0; i < convertedCon.ItemList.Length; i++ )
                                {
                                    convertedCon.ItemList[i] = Marshal.ReadByte( pv, 6 + i );
                                }
                            }
                            catch ( Exception ex )
                            {
                                exception = ex;
                            }
                            finally
                            {
                                Extern.GlobalUnlock( TwCapability.hContainer );
                            }
                        }
                        Extern.GlobalFree( TwCapability.hContainer );
                        TwCapability.hContainer = IntPtr.Zero;

                        if ( exception != null )
                        {
                            throw exception;
                        }

                        ContainerValue = convertedCon;
                        break;
                    }
                default:
                    {
                        throw new Exception( "An unknown TWON value was returned by the scanner while negotiating capabilities" );
                    }
            }
        }

        private void CopyValueToContainer( object value )
        {
            if ( TwCapability.hContainer != IntPtr.Zero )
            {
                Extern.GlobalFree( TwCapability.hContainer );
                TwCapability.hContainer = IntPtr.Zero;
            }

            if ( ContainerValue != value )
            {
                ContainerValue = value;
            }

            TwCapability.hContainer = Extern.GlobalAlloc( Extern.GlobalAllocFlags.GHND, (uint)Marshal.SizeOf( value ) );
            if ( TwCapability.hContainer == IntPtr.Zero )
            {
                throw new Exception( string.Concat( "Allocating memory to a capability container failed while attempting to copy a value into it\nError Code: ", Marshal.GetLastWin32Error().ToString() ) );
            }

            IntPtr pContainer = Extern.GlobalLock( TwCapability.hContainer );
            if ( pContainer == IntPtr.Zero )
            {
                throw new Exception( string.Concat( "A global lock failed while attempting to copying a value into a capability container\nError Code: ", Marshal.GetLastWin32Error().ToString() ) );
            }

            Exception exCheck = null;
            try
            {
                Marshal.StructureToPtr( value, pContainer, false );
            }
            catch ( Exception ex )
            {
                exCheck = ex;
            }
            finally
            {
                Extern.GlobalUnlock( TwCapability.hContainer );
            }

            if ( exCheck != null )
            {
                throw exCheck;
            }
        }

        /// <summary>
        /// This routine will convert a value that's stored an a type that's different from what TWAIN type it's supposed to be
        /// </summary>
        /// <typeparam name="T">The type to convert the value to</typeparam>
        /// <param name="twty">The TWAIN type that the value is suppoed to be</param>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static T ConvertValue<T>( TWTY twty, uint value ) // twty is usually equivelent to T, but doesn't necessarily have to be
        {
            string ret = string.Empty;

            switch ( twty )
            {
                case TWTY.INT8:
                    ret = ( (sbyte)value ).ToString();
                    break;
                case TWTY.INT16:
                    ret = ( (Int16)value ).ToString();
                    break;
                case TWTY.INT32:
                    ret = ( (Int32)value ).ToString();
                    break;
                case TWTY.UINT8:
                    ret = ( (byte)value ).ToString();
                    break;
                case TWTY.UINT16:
                    ret = ( (UInt16)value ).ToString();
                    break;
                case TWTY.UINT32:
                    ret = ( (UInt32)value ).ToString();
                    break;
                case TWTY.BOOL:
                    ret = Convert.ToBoolean( value ).ToString();
                    break;
                case TWTY.FIX32:
                    throw new Exception( "An attempt was made to convert a FIX32 value using the ConvertValue capability method instead of ConvertByteArray" );
                case TWTY.FRAME:
                    throw new Exception( "An attempt was made to convert a FRAME value using the ConvertValue capability method instead of ConvertByteArray" );
                case TWTY.STR32:
                    throw new Exception( "An attempt was made to convert a STR32 value using the ConvertValue capability method instead of ConvertByteArray" );
                case TWTY.STR64:
                    throw new Exception( "An attempt was made to convert a STR64 value using the ConvertValue capability method instead of ConvertByteArray" );
                case TWTY.STR128:
                    throw new Exception( "An attempt was made to convert a STR128 value using the ConvertValue capability method instead of ConvertByteArray" );
                case TWTY.STR255:
                    throw new Exception( "An attempt was made to convert a STR255 value using the ConvertValue capability method instead of ConvertByteArray" );
                case TWTY.STR1024:
                    throw new Exception( "An attempt was made to convert a STR1024 value using the ConvertValue capability method instead of ConvertByteArray" );
                case TWTY.UNI512:
                    throw new Exception( "An attempt was made to convert a UNI512 value using the ConvertValue capability method instead of ConvertByteArray" );
            }

            TypeConverter tConv = TypeDescriptor.GetConverter( typeof( T ) );
            return (T)tConv.ConvertFromString( ret );
        }

        public List<T> ConvertByteArray<T>()
        {
            List<T> ret = new List<T>();
            TWTY twty;
            byte[] itemList;

            // check to make sure that the container is either an enum or array
            TWON twon = ContainerType;
            if ( twon == TWON.ENUM )
            {
                TW_ENUMERATION twEnum;
                if ( ContainerValue == null )
                {
                    twEnum = GetContainerValue<TW_ENUMERATION>();
                }
                else
                {
                    twEnum = ContainerValue as TW_ENUMERATION;
                }

                twty = (TWTY)twEnum.ItemType;
                itemList = twEnum.ItemList;
            }
            else if ( twon == TWON.ARRAY )
            {
                TW_ARRAY twArray;
                if ( ContainerValue == null )
                {
                    twArray = GetContainerValue<TW_ARRAY>();
                }
                else
                {
                    twArray = ContainerValue as TW_ARRAY;
                }

                twty = (TWTY)twArray.ItemType;
                itemList = twArray.ItemList;
            }
            else
            {
                throw new Exception( string.Concat( "An attempt to convert a container's ItemList to an array was made, but the container type is ", twon.ToString() ) );
            }

            // if the item list has no elements in it, then return an empty list
            if ( itemList == null || itemList.Length == 0 )
            {
                return ret;
            }

            // check to make sure that the item list size is evenly divisible by the expected item size
            ushort itemSize = SizeOfTwainType( twty );
            if ( itemList.Length % itemSize != 0 )
            {
                // leftovers are a sure indication that either the conversion isn't correct, or the container's item list is corrupt
                throw new Exception( "An attempt to convert a container's ItemList to an array was made, but the list size wasn't evenly divisible by the expected item size" );
            }

            // check to make sure that T is equivelent to ItemType and convert
            string convVal;
            Type t = typeof( T );
            TypeConverter tConv = TypeDescriptor.GetConverter( typeof( T ) );
            if ( t == typeof( sbyte ) && twty == TWTY.INT8 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = itemList[i].ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( Int16 ) && twty == TWTY.INT16 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToInt16( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( Int32 ) && twty == TWTY.INT32 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToInt32( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( byte ) && twty == TWTY.UINT8 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = itemList[i].ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( UInt16 ) && twty == TWTY.UINT16 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToUInt16( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( UInt32 ) && twty == TWTY.UINT32 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToUInt32( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( Boolean ) && twty == TWTY.BOOL )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToBoolean( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( TW_FIX32 ) && twty == TWTY.FIX32 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    TW_FIX32 twFix32 = ConvertFix32( itemList, i );
                    ret.Add( (T)tConv.ConvertFrom( twFix32 ) );
                }
            }
            else if ( t == typeof( TW_FRAME ) && twty == TWTY.FRAME )
            {
                ushort sizeFix32 = SizeOfTwainType( TWTY.FIX32 );
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    TW_FRAME twFrame = new TW_FRAME()
                    {
                        Left = ConvertFix32( itemList, i ),
                        Top = ConvertFix32( itemList, i + sizeFix32 ),
                        Right = ConvertFix32( itemList, i + ( sizeFix32 * 2 ) ),
                        Bottom = ConvertFix32( itemList, i + ( sizeFix32 * 3 ) )
                    };

                    ret.Add( (T)tConv.ConvertFrom( twFrame ) );
                }

            }
            else if ( t == typeof( String ) && twty == TWTY.STR32 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToString( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( String ) && twty == TWTY.STR64 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToString( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( String ) && twty == TWTY.STR128 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToString( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( String ) && twty == TWTY.STR255 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToString( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( String ) && twty == TWTY.STR1024 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToString( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else if ( t == typeof( String ) && twty == TWTY.UNI512 )
            {
                for ( int i = 0; i < itemList.Length; i += itemSize )
                {
                    convVal = BitConverter.ToString( itemList, i ).ToString();
                    ret.Add( (T)tConv.ConvertFromString( convVal ) );
                }
            }
            else
            {
                throw new Exception( string.Concat( "An attempt to convert a container's ItemList was made, but the requested List type doesn't match the TWAIN type specified in the container\n\nList Type: ", t.ToString(), "\nTWAIN Type: ", twty.ToString() ) );
            }

            return ret;
        }

        private static TW_FIX32 ConvertFix32( byte[] byteArray, int position )
        {
            TypeConverter convWhole = TypeDescriptor.GetConverter( typeof( Int16 ) );
            TypeConverter convFrac = TypeDescriptor.GetConverter( typeof( UInt16 ) );

            TW_FIX32 twFix32 = new TW_FIX32();

            string convWholeVal = BitConverter.ToUInt16( byteArray, position ).ToString();
            twFix32.Whole = (Int16)convWhole.ConvertFromString( convWholeVal );

            string convFracVal = BitConverter.ToUInt16( byteArray, position ).ToString();
            twFix32.Frac = (UInt16)convFrac.ConvertFromString( convFracVal );

            return twFix32;
        }

        private ushort SizeOfTwainType( TWTY twty )
        {
            switch ( twty )
            {
                case TWTY.INT8:
                    return 1;
                case TWTY.INT16:
                    return 2;
                case TWTY.INT32:
                    return 4;

                case TWTY.UINT8:
                    return 1;
                case TWTY.UINT16:
                    return 2;
                case TWTY.UINT32:
                    return 4;

                case TWTY.BOOL:
                    return 2;

                case TWTY.FIX32:
                    return 4;

                case TWTY.FRAME:
                    return 16;

                case TWTY.STR32:
                    return (ushort)TWSTR.STR32;
                case TWTY.STR64:
                    return (ushort)TWSTR.STR64;
                case TWTY.STR128:
                    return (ushort)TWSTR.STR128;
                case TWTY.STR255:
                    return (ushort)TWSTR.STR255;
                case TWTY.STR1024:
                    return (ushort)TWSTR.STR1024;
                case TWTY.UNI512:
                    return (ushort)TWSTR.UNI512;

                default:
                    return 0;
            }
        }

        public CAP CapabilityCode { get { return (CAP)TwCapability.Cap; } }

        public TWON ContainerType
        {
            get { return (TWON)TwCapability.ConType; }
            private set { TwCapability.ConType = (ushort)value; }
        }

        private object containerValue = null;
        private object ContainerValue
        {
            get { return containerValue; }
            set { containerValue = value; }
        }

        private TW_CAPABILITY twCapability = null;
        internal TW_CAPABILITY TwCapability
        {
            get
            {
                if ( twCapability == null )
                {
                    twCapability = new TW_CAPABILITY();
                }

                return twCapability;
            }
            private set { twCapability = value; }
        }
    }
}
