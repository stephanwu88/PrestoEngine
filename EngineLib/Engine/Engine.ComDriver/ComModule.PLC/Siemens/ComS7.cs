using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Engine.Common;
using Engine.Mod;
using Engine.MVVM.Messaging;
using PlcTypes = Engine.ComDriver.Types;

namespace Engine.ComDriver.Siemens
{
    /// <summary>
    /// 西门子S7通信协议
    /// </summary>
    public partial class ComS7 
    {
        #region 内部变量
        /// <summary>
        /// PLC CPU类型
        /// </summary>
        private CpuType CPU { get; set; }
        /// <summary>
        /// PLC Rack
        /// </summary>
        private Int16 Rack { get; set; }
        /// <summary>
        /// PLC CPU Slot
        /// </summary>
        private Int16 Slot { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ComS7()
        {

        }
        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="eCpuType"></param>
        /// <param name="ComIP"></param>
        public ComS7(CpuType eCpuType, string ComIP)
        {
            _DriverItem.CpuType = eCpuType.ToString();
            _DriverItem.ComParam.ComIP = ComIP;
            _DriverItem.ComParam.ComPort = 102;
            _DriverItem.ComParam.CycleTime = 100;
            Rack = 0;
            if (CPU == CpuType.S7300 || CPU == CpuType.S7400)
                Slot = 2;
            Slot = 0;
        }
        #endregion

        #region 公共方法

        /// <summary>
        /// Reads multiple vars in a single request. 
        /// You have to create and pass a list of DataItems and you obtain in response the same list with the values.
        /// Values are stored in the property "Value" of the dataItem and are already converted.
        /// If you don't want the conversion, just create a dataItem of bytes. 
        /// DataItems must not be more than 20 (protocol restriction) and bytes must not be more than 200 + 22 of header (protocol restriction).
        /// </summary>
        /// <param name="dataItems">List of dataitems that contains the list of variables that must be read. Maximum 20 dataitems are accepted.</param>
        public void ReadMultipleVars(List<DataItem> dataItems)
        {
            int cntBytes = dataItems.Sum(dataItem => VarTypeToByteLength(dataItem.VarType, dataItem.Count));

            if (dataItems.Count > 20) throw new Exception("Too many vars requested");
            if (cntBytes > 222) throw new Exception("Too many bytes requested"); //todo, proper TDU check + split in multiple requests

            try
            {
                // first create the header
                int packageSize = 19 + (dataItems.Count * 12);
                PlcTypes.ByteArray package = new PlcTypes.ByteArray(packageSize);
                package.Add(ReadHeaderPackage(dataItems.Count));
                // package.Add(0x02);  // datenart
                foreach (var dataItem in dataItems)
                {
                    package.Add(CreateReadDataRequestPackage(dataItem.DataType, dataItem.DB, dataItem.StartByteAdr, VarTypeToByteLength(dataItem.VarType, dataItem.Count)));
                }

                mClient.Send(package.array, package.array.Length, SocketFlags.None);

                byte[] bReceive = new byte[512];
                int numReceived = mClient.Receive(bReceive, 512, SocketFlags.None);
                if (bReceive[21] != 0xff) throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());

                int offset = 25;
                foreach (var dataItem in dataItems)
                {
                    int byteCnt = VarTypeToByteLength(dataItem.VarType, dataItem.Count);
                    byte[] bytes = new byte[byteCnt];

                    for (int i = 0; i < byteCnt; i++)
                    {
                        bytes[i] = bReceive[i + offset];
                    }

                    offset += byteCnt + 4;

                    dataItem.Value = ParseBytes(dataItem.VarType, bytes, dataItem.Count);
                }
            }
            catch (SocketException socketException)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = socketException.Message;
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = exc.Message;
            }
        }

        /// <summary>
        /// Reads a single variable from the plc, takes in input strings like "DB1.DBX0.0", "DB20.DBD200", "MB20", "T45", etc.
        /// If the read was not successful, check LastErrorCode or LastErrorString.
        /// </summary>
        /// <param name="variable">Input strings like "DB1.DBX0.0", "DB20.DBD200", "MB20", "T45", etc.</param>
        /// <returns>Returns an object that contains the value. This object must be cast accordingly.</returns>
        public object Read(string variable)
        {
            DataType mDataType;
            int mDB;
            int mByte;
            int mBit;

            byte objByte;
            UInt16 objUInt16;
            UInt32 objUInt32;
            double objDouble;
            BitArray objBoolArray;

            string txt = variable.ToUpper();
            txt = txt.Replace(" ", "").Replace("%", ""); ;     // remove spaces

            try
            {
                switch (txt.Substring(0, 2))
                {
                    case "DB":
                        string[] strings = txt.Split(new char[] { '.' });
                        if (strings.Length < 2)
                            throw new Exception();

                        mDB = int.Parse(strings[0].Substring(2));
                        string dbType = strings[1].Substring(0, 3);
                        int dbIndex = int.Parse(strings[1].Substring(3));

                        switch (dbType)
                        {
                            case "DBB":
                                byte obj = (byte)Read(DataType.DataBlock, mDB, dbIndex, VarType.Byte, 1);
                                return obj;
                            case "DBW":
                                UInt16 objI = (UInt16)Read(DataType.DataBlock, mDB, dbIndex, VarType.Word, 1);
                                return objI;
                            case "DBD":
                                UInt32 objU = (UInt32)Read(DataType.DataBlock, mDB, dbIndex, VarType.DWord, 1);
                                return objU;
                            case "DBX":
                                mByte = dbIndex;
                                mBit = int.Parse(strings[2]);
                                if (mBit > 7) throw new Exception();
                                byte obj2 = (byte)Read(DataType.DataBlock, mDB, mByte, VarType.Byte, 1);
                                objBoolArray = new BitArray(new byte[] { obj2 });
                                return objBoolArray[mBit];
                            default:
                                throw new Exception();
                        }
                    case "EB":
                        // Input byte
                        objByte = (byte)Read(DataType.Input, 0, int.Parse(txt.Substring(2)), VarType.Byte, 1);
                        return objByte;
                    case "EW":
                        // Input word
                        objUInt16 = (UInt16)Read(DataType.Input, 0, int.Parse(txt.Substring(2)), VarType.Word, 1);
                        return objUInt16;
                    case "ED":
                        // Input double-word
                        objUInt32 = (UInt32)Read(DataType.Input, 0, int.Parse(txt.Substring(2)), VarType.DWord, 1);
                        return objUInt32;
                    case "AB":
                        // Output byte
                        objByte = (byte)Read(DataType.Output, 0, int.Parse(txt.Substring(2)), VarType.Byte, 1);
                        return objByte;
                    case "AW":
                        // Output word
                        objUInt16 = (UInt16)Read(DataType.Output, 0, int.Parse(txt.Substring(2)), VarType.Word, 1);
                        return objUInt16;
                    case "AD":
                        // Output double-word
                        objUInt32 = (UInt32)Read(DataType.Output, 0, int.Parse(txt.Substring(2)), VarType.DWord, 1);
                        return objUInt32;
                    case "MB":
                        // Memory byte
                        objByte = (byte)Read(DataType.Memory, 0, int.Parse(txt.Substring(2)), VarType.Byte, 1);
                        return objByte;
                    case "MW":
                        // Memory word
                        objUInt16 = (UInt16)Read(DataType.Memory, 0, int.Parse(txt.Substring(2)), VarType.Word, 1);
                        return objUInt16;
                    case "MD":
                        // Memory double-word
                        objUInt32 = (UInt32)Read(DataType.Memory, 0, int.Parse(txt.Substring(2)), VarType.DWord, 1);
                        return objUInt32;
                    default:
                        switch (txt.Substring(0, 1))
                        {
                            case "E":
                            case "I":
                                // Input
                                mDataType = DataType.Input;
                                break;
                            case "A":
                            case "Q":
                                // Output
                                mDataType = DataType.Output;
                                break;
                            case "M":
                                // Memory
                                mDataType = DataType.Memory;
                                break;
                            case "T":
                                // Timer
                                objDouble = (double)Read(DataType.Timer, 0, int.Parse(txt.Substring(1)), VarType.Timer, 1);
                                return objDouble;
                            case "Z":
                            case "C":
                                // Counter
                                objUInt16 = (UInt16)Read(DataType.Counter, 0, int.Parse(txt.Substring(1)), VarType.Counter, 1);
                                return objUInt16;
                            default:
                                throw new Exception();
                        }

                        string txt2 = txt.Substring(1);
                        if (txt2.IndexOf(".") == -1) throw new Exception();

                        mByte = int.Parse(txt2.Substring(0, txt2.IndexOf(".")));
                        mBit = int.Parse(txt2.Substring(txt2.IndexOf(".") + 1));
                        if (mBit > 7) throw new Exception();
                        var obj3 = (byte)Read(mDataType, 0, mByte, VarType.Byte, 1);
                        objBoolArray = new BitArray(new byte[] { obj3 });
                        return objBoolArray[mBit];
                }
            }
            catch
            {
                LastErrorCode = ErrorCode.WrongVarFormat;
                LastErrorString = "The variable'" + variable + "' could not be read. Please check the syntax and try again.";
                return LastErrorCode;
            }
        }

        /// <summary>
        /// 请求PLC格式化数据
        /// Read and decode a certain number of bytes of the "VarType" provided. 
        /// This can be used to read multiple consecutive variables of the same type (Word, DWord, Int, etc).
        /// If the read was not successful, check LastErrorCode or LastErrorString.
        /// </summary>
        /// <param name="dataType">Data type of the memory area, can be DB, Timer, Counter, Merker(Memory), Input, Output.</param>
        /// <param name="numOfBytes(db)">Address of the memory area (if you want to read DB1, this is set to 1). This must be set also for other memory area types: counters, timers,etc.</param>
        /// <param name="startByteAdr">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <param name="varType">Type of the variable/s that you are reading</param>
        /// <param name="bitAdr">Address of bit. If you want to read DB1.DBX200.6, set 6 to this parameter.</param>
        /// <param name="varCount"></param>
        public object Read(DataType dataType, int db, int startByteAdr, VarType varType, int varCount, byte bitAdr = 0)
        {
            int cntBytes = VarTypeToByteLength(varType, varCount);
            byte[] bytes = ReadBytes(dataType, db, startByteAdr, cntBytes);
            return ParseBytes(varType, bytes, varCount, bitAdr);
        }

        /// <summary>
        /// 请求PLC字节序数据
        /// Reads a number of bytes from a DB starting from a specified index. This handles more than 200 bytes with multiple requests.
        /// If the read was not successful, check LastErrorCode or LastErrorString.
        /// </summary>
        /// <param name="dataType">Data type of the memory area, can be DB, Timer, Counter, Merker(Memory), Input, Output.</param>
        /// <param name="numOfBytes(db)">Address of the memory area (if you want to read DB1, this is set to 1). This must be set also for other memory area types: counters, timers,etc.</param>
        /// <param name="startAddr(startByteAdr)">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <param name="count">Byte count, if you want to read 120 bytes, set this to 120.</param>
        /// <returns>Returns the bytes in an array</returns>
        public byte[] ReadBytes(DataType dataType, int db, int startByteAdr, int count)
        {
            List<byte> resultBytes = new List<byte>();
            int index = startByteAdr;
            while (count > 0)
            {
                var maxToRead = (int)Math.Min(count, 200);
                byte[] bytes = ReadBytesWithASingleRequest(dataType, db, index, maxToRead);
                if (bytes == null)
                    return resultBytes.ToArray();
                resultBytes.AddRange(bytes);
                count -= maxToRead;
                index += maxToRead;
            }
            return resultBytes.ToArray();
        }

        /// <summary>
        /// Reads all the bytes needed to fill a struct in C#, starting from a certain address, and return an object that can be casted to the struct.
        /// </summary>
        /// <param name="structType">Type of the struct to be readed (es.: TypeOf(MyStruct)).</param>
        /// <param name="db">Address of the DB.</param>
        /// <param name="startByteAdr">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <returns>Returns a struct that must be cast.</returns>
        public object ReadStruct(Type structType, int db, int startByteAdr = 0)
        {
            int numBytes = Types.Struct.GetStructSize(structType);
            // now read the package
            var resultBytes = ReadBytes(DataType.DataBlock, db, startByteAdr, numBytes);

            // and decode it
            return Types.Struct.FromBytes(structType, resultBytes);
        }

        /// <summary>
        /// Reads all the bytes needed to fill a struct in C#, starting from a certain address, and returns the struct or null if nothing was read.
        /// </summary>
        /// <typeparam name="T">The struct type</typeparam>
        /// <param name="db">Address of the DB.</param>
        /// <param name="startByteAdr">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <returns>Returns a nulable struct. If nothing was read null will be returned.</returns>
        public T? ReadStruct<T>(int db, int startByteAdr = 0) where T : struct
        {
            return ReadStruct(typeof(T), db, startByteAdr) as T?;
        }

        /// <summary>
        /// Reads all the bytes needed to fill a class in C#, starting from a certain address, and set all the properties values to the value that are read from the plc. 
        /// This reads only properties, it doesn't read private variable or public variable without {get;set;} specified.
        /// </summary>
        /// <param name="sourceClass">Instance of the class that will store the values</param>       
        /// <param name="db">Index of the DB; es.: 1 is for DB1</param>
        /// <param name="startByteAdr">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <returns>The number of read bytes</returns>
        public int ReadClass(object sourceClass, int db, int startByteAdr = 0)
        {
            int numBytes = PlcTypes.SiemensClass.GetClassSize(sourceClass);
            if (numBytes <= 0)
            {
                throw new Exception("The size of the class is less than 1 byte and therefore cannot be read");
            }

            // now read the package
            var resultBytes = ReadBytes(DataType.DataBlock, db, startByteAdr, numBytes);
            // and decode it
            PlcTypes.SiemensClass.FromBytes(sourceClass, resultBytes);

            return resultBytes.Length;
        }

        /// <summary>
        /// Reads all the bytes needed to fill a class in C#, starting from a certain address, and set all the properties values to the value that are read from the plc. 
        /// This reads only properties, it doesn't read private variable or public variable without {get;set;} specified. To instantiate the class defined by the generic
        /// type, the class needs a default constructor.
        /// </summary>
        /// <typeparam name="T">The class that will be instantiated. Requires a default constructor</typeparam>
        /// <param name="db">Index of the DB; es.: 1 is for DB1</param>
        /// <param name="startByteAdr">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <returns>An instance of the class with the values read from the plc. If no data has been read, null will be returned</returns>
        public T ReadClass<T>(int db, int startByteAdr = 0) where T : class
        {
            return ReadClass(() => Activator.CreateInstance<T>(), db, startByteAdr);
        }

        /// <summary>
        /// Reads all the bytes needed to fill a class in C#, starting from a certain address, and set all the properties values to the value that are read from the plc. 
        /// This reads only properties, it doesn't read private variable or public variable without {get;set;} specified.
        /// </summary>
        /// <typeparam name="T">The class that will be instantiated</typeparam>
        /// <param name="classFactory">Function to instantiate the class</param>
        /// <param name="db">Index of the DB; es.: 1 is for DB1</param>
        /// <param name="startByteAdr">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <returns>An instance of the class with the values read from the plc. If no data has been read, null will be returned</returns>
        public T ReadClass<T>(Func<T> classFactory, int db, int startByteAdr = 0) where T : class
        {
            var instance = classFactory();
            int readBytes = ReadClass(instance, db, startByteAdr);
            if (readBytes <= 0)
            {
                return null;
            }

            return instance;
        }

        /// <summary>
        /// 写入PLC格式化数据
        /// 通过 LastErrorCode or LastErrorString判别成功与否
        /// </summary>
        /// <param name="dataType"> DB, Timer, Counter, Merker(Memory), Input, Output.</param>
        /// <param name="numOfBytes(db)">Address of the memory area (if you want to read DB1, this is set to 1). This must be set also for other memory area types: counters, timers,etc.</param>
        /// <param name="startByteAdr">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <param name="value">Bytes to write. The lenght of this parameter can't be higher than 200. If you need more, use recursion.</param>
        /// <param name="bitAdr">The address of the bit. (0-7)</param>
        /// <returns>NoError if it was successful, or the error is specified</returns>
        public ErrorCode Write(DataType dataType, int db, int startByteAdr, VarType varType, object value, int bitAdr = -1)
        {
            byte[] package = null;

            if (bitAdr != -1)
            {
                //Must be writing a bit value as bitAdr is specified
                bool bitValue = false;
                if (value is bool)
                {
                    bitValue = (bool)value;
                }
                else if (value is int)
                {
                    var intValue = (int)value;
                    if (intValue < 0 || intValue > 7)
                        throw new Exception(string.Format("Addressing Error: You can only reference bitwise locations 0-7. Address {0} is invalid", bitAdr));

                    bitValue = intValue == 1;
                }
                else
                {
                    throw new ArgumentException("Value must be a bool or an int to write a bit", nameof(value));
                }

                return WriteBit(dataType, db, startByteAdr, bitAdr, bitValue);
            }

            switch (varType)
            {
                case VarType.Byte:
                    package = PlcTypes.Byte.ToByteArray((byte)value);
                    break;
                case VarType.Int:
                    package = PlcTypes.Int.ToByteArray((Int16)value, SystemDefault.SiemensByteOrder16);
                    break;
                case VarType.Word:
                    package = PlcTypes.Word.ToByteArray((UInt16)value, SystemDefault.SiemensByteOrder16);
                    break;
                case VarType.DInt:
                    package = PlcTypes.DInt.ToByteArray((Int32)value, SystemDefault.SiemensByteOrder32);
                    break;
                case VarType.DWord:
                    package = PlcTypes.DWord.ToByteArray((UInt32)value, SystemDefault.SiemensByteOrder32);
                    break;
                case VarType.Real:
                    package = PlcTypes.Real.ToByteArray((Double)value, SystemDefault.SiemensByteOrder32);
                    break;
                case VarType.ByteArray:
                    package = (byte[])value;
                    break;
                case VarType.IntArray:
                    package = PlcTypes.Int.ToByteArray((Int16[])value, SystemDefault.SiemensByteOrder16);
                    break;
                case VarType.WordArray:
                    package = PlcTypes.Word.ToByteArray((UInt16[])value, SystemDefault.SiemensByteOrder16);
                    break;
                case VarType.DIntArray:
                    package = PlcTypes.DInt.ToByteArray((Int32[])value, SystemDefault.SiemensByteOrder32);
                    break;
                case VarType.DWordArray:
                    package = PlcTypes.DWord.ToByteArray((UInt32[])value, SystemDefault.SiemensByteOrder32);
                    break;
                case VarType.RealArray:
                    package = PlcTypes.Real.ToByteArray((double[])value, SystemDefault.SiemensByteOrder32);
                    break;
                case VarType.String:
                    package = PlcTypes.String.ToByteArray(value as string);
                    break;
                case VarType.S7String:
                    package = PlcTypes.S7String.ToByteArray(value as string);
                    break;
                case VarType.S7WString:
                    var temp = Encoding.BigEndianUnicode.GetBytes(value as string);
                    package = PlcTypes.S7WString.ToByteArray(value as string, temp.Length);
                    break;
                default:
                    return ErrorCode.WrongVarFormat;
            }
            return WriteBytes(dataType, db, startByteAdr, package);
        }

        /// <summary>
        /// 写入PLC字节序数据
        /// Write a number of bytes from a DB starting from a specified index. This handles more than 200 bytes with multiple requests.
        /// If the write was not successful, check LastErrorCode or LastErrorString.
        /// </summary>
        /// <param name="dataType">Data type of the memory area, can be DB, Timer, Counter, Merker(Memory), Input, Output.</param>
        /// <param name="numOfBytes(db)">Address of the memory area (if you want to read DB1, this is set to 1). This must be set also for other memory area types: counters, timers,etc.</param>
        /// <param name="startByteAdr">Start byte address. If you want to write DB1.DBW200, this is 200.</param>
        /// <param name="value">Bytes to write. If more than 200, multiple requests will be made.</param>
        /// <returns>NoError if it was successful, or the error is specified</returns>
        public ErrorCode WriteBytes(DataType dataType, int db, int startByteAdr, byte[] value)
        {
            int localIndex = 0;
            int count = value.Length;
            while (count > 0)
            {
                var maxToWrite = (int)Math.Min(count, 200);
                ErrorCode lastError = WriteBytesWithASingleRequest(dataType, db, startByteAdr + localIndex, value.Skip(localIndex).Take(maxToWrite).ToArray());
                if (lastError != ErrorCode.NoError)
                {
                    return lastError;
                }
                count -= maxToWrite;
                localIndex += maxToWrite;
            }
            return ErrorCode.NoError;
        }

        /// <summary>
        /// 写入PLC位
        /// </summary>
        /// <param name="dataType">Data type of the memory area, can be DB, Timer, Counter, Merker(Memory), Input, Output.</param>
        /// <param name="numOfBytes(db)">Address of the memory area (if you want to read DB1, this is set to 1). This must be set also for other memory area types: counters, timers,etc.</param>
        /// <param name="startByteAddr">Start byte address. If you want to write DB1.DBW200, this is 200.</param>
        /// <param name="bitAddr">The address of the bit. (0-7)</param>
        /// <param name="value">Bytes to write. If more than 200, multiple requests will be made.</param>
        /// <returns>NoError if it was successful, or the error is specified</returns>
        public ErrorCode WriteBit(DataType dataType, int db, int startByteAddr, int bitAddr, object value)
        {
            if (bitAddr < 0 || bitAddr > 7)
                throw new Exception(string.Format("Addressing Error: You can only reference bitwise locations 0-7. Address {0} is invalid", bitAddr));
            bool bitVal = false;
            if (value is int)
                bitVal = (int)value == 1;
            else if (value is byte)
                bitVal = (byte)value == 1;
            else if (value is bool)
                bitVal = (bool)value;
            return WriteBitWithASingleRequest(dataType, db, startByteAddr, bitAddr, bitVal);
        }

        /// <summary>
        /// Writes a C# struct to a DB in the plc
        /// </summary>
        /// <param name="structValue">The struct to be written</param>
        /// <param name="db">Db address</param>
        /// <param name="startByteAdr">Start bytes on the plc</param>
        /// <returns>NoError if it was successful, or the error is specified</returns>
        public ErrorCode WriteStruct(object structValue, int db, int startByteAdr = 0)
        {
            var bytes = Types.Struct.ToBytes(structValue).ToList();
            var errCode = WriteBytes(DataType.DataBlock, db, startByteAdr, bytes.ToArray());
            return errCode;
        }

        /// <summary>
        /// Writes a C# class to a DB in the plc
        /// </summary>
        /// <param name="classValue">The class to be written</param>
        /// <param name="db">Db address</param>
        /// <param name="startByteAdr">Start bytes on the plc</param>
        /// <returns>NoError if it was successful, or the error is specified</returns>
        public ErrorCode WriteClass(object classValue, int db, int startByteAdr = 0)
        {
            var bytes = PlcTypes.SiemensClass.ToBytes(classValue).ToList();
            var errCode = WriteBytes(DataType.DataBlock, db, startByteAdr, bytes.ToArray());
            return errCode;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// Creates the header to read bytes from the plc
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private PlcTypes.ByteArray ReadHeaderPackage(int amount = 1)
        {
            //header size = 19 bytes
            var package = new PlcTypes.ByteArray(19);
            package.Add(new byte[] { 0x03, 0x00, 0x00 });
            //complete package size
            package.Add((byte)(19 + (12 * amount)));
            package.Add(new byte[] { 0x02, 0xf0, 0x80, 0x32, 0x01, 0x00, 0x00, 0x00, 0x00 });
            //data part size
            package.Add(PlcTypes.Word.ToByteArray((ushort)(2 + (amount * 12)), SystemDefault.SiemensByteOrder16));
            package.Add(new byte[] { 0x00, 0x00, 0x04 });
            //amount of requests
            package.Add((byte)amount);

            return package;
        }

        /// <summary>
        /// Create the bytes-package to request data from the plc. You have to specify the memory type (dataType), 
        /// the address of the memory, the address of the byte and the bytes count. 
        /// </summary>
        /// <param name="dataType">MemoryType (DB, Timer, Counter, etc.)</param>
        /// <param name="db">Address of the memory to be read</param>
        /// <param name="startByteAdr">Start address of the byte</param>
        /// <param name="count">Number of bytes to be read</param>
        /// <returns></returns>
        private PlcTypes.ByteArray CreateReadDataRequestPackage(DataType dataType, int db, int startByteAdr, int count = 1)
        {
            //single data req = 12
            var package = new PlcTypes.ByteArray(12);
            package.Add(new byte[] { 0x12, 0x0a, 0x10 });
            switch (dataType)
            {
                case DataType.Timer:
                case DataType.Counter:
                    package.Add((byte)dataType);
                    break;
                default:
                    package.Add(0x02);
                    break;
            }

            package.Add(PlcTypes.Word.ToByteArray((ushort)(count), SystemDefault.SiemensByteOrder16));
            package.Add(PlcTypes.Word.ToByteArray((ushort)(db), SystemDefault.SiemensByteOrder16));
            package.Add((byte)dataType);
            var overflow = (int)(startByteAdr * 8 / 0xffffU); // handles words with address bigger than 8191
            package.Add((byte)overflow);
            switch (dataType)
            {
                case DataType.Timer:
                case DataType.Counter:
                    package.Add(Types.Word.ToByteArray((ushort)(startByteAdr), SystemDefault.SiemensByteOrder16));
                    break;
                default:
                    package.Add(Types.Word.ToByteArray((ushort)((startByteAdr) * 8), SystemDefault.SiemensByteOrder16));
                    break;
            }

            return package;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="db"></param>
        /// <param name="startByteAdr"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] ReadBytesWithASingleRequest(DataType dataType, int db, int startByteAdr, int count)
        {
            byte[] bytes = new byte[count];

            try
            {
                // first create the header
                int packageSize = 31;
                PlcTypes.ByteArray package = new PlcTypes.ByteArray(packageSize);
                package.Add(ReadHeaderPackage());
                // package.Add(0x02);  // datenart
                package.Add(CreateReadDataRequestPackage(dataType, db, startByteAdr, count));

                mClient.Send(package.array, package.array.Length, SocketFlags.None);

                byte[] bReceive = new byte[512];
                int numReceived = mClient.Receive(bReceive, 512, SocketFlags.None);
                if (bReceive[21] != 0xff)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());

                for (int cnt = 0; cnt < count; cnt++)
                    bytes[cnt] = bReceive[cnt + 25];

                return bytes;
            }
            catch (SocketException socketException)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = socketException.Message;
                return null;
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = exc.Message;
                return null;
            }
        }

        /// <summary>
        /// Writes up to 200 bytes to the plc and returns NoError if successful. You must specify the memory area type, memory are address, byte start address and bytes count.
        /// If the write was not successful, check LastErrorCode or LastErrorString.
        /// </summary>
        /// <param name="dataType">Data type of the memory area, can be DB, Timer, Counter, Merker(Memory), Input, Output.</param>
        /// <param name="db">Address of the memory area (if you want to read DB1, this is set to 1). This must be set also for other memory area types: counters, timers,etc.</param>
        /// <param name="startByteAdr">Start byte address. If you want to read DB1.DBW200, this is 200.</param>
        /// <param name="value">Bytes to write. The lenght of this parameter can't be higher than 200. If you need more, use recursion.</param>
        /// <returns>NoError if it was successful, or the error is specified</returns>
        private ErrorCode WriteBytesWithASingleRequest(DataType dataType, int db, int startByteAdr, byte[] value)
        {
            byte[] bReceive = new byte[513];
            int varCount = 0;

            try
            {
                varCount = value.Length;
                // first create the header
                int packageSize = 35 + value.Length;
                Types.ByteArray package = new Types.ByteArray(packageSize);

                package.Add(new byte[] { 3, 0, 0 });
                package.Add((byte)packageSize);
                package.Add(new byte[] { 2, 0xf0, 0x80, 0x32, 1, 0, 0 });
                package.Add(Types.Word.ToByteArray((ushort)(varCount - 1), SystemDefault.SiemensByteOrder16));
                package.Add(new byte[] { 0, 0x0e });
                package.Add(Types.Word.ToByteArray((ushort)(varCount + 4), SystemDefault.SiemensByteOrder16));
                package.Add(new byte[] { 0x05, 0x01, 0x12, 0x0a, 0x10, 0x02 });
                package.Add(Types.Word.ToByteArray((ushort)varCount, SystemDefault.SiemensByteOrder16));
                package.Add(Types.Word.ToByteArray((ushort)(db), SystemDefault.SiemensByteOrder16));
                package.Add((byte)dataType);
                var overflow = (int)(startByteAdr * 8 / 0xffffU); // handles words with address bigger than 8191
                package.Add((byte)overflow);
                package.Add(Types.Word.ToByteArray((ushort)(startByteAdr * 8), SystemDefault.SiemensByteOrder16));
                package.Add(new byte[] { 0, 4 });
                package.Add(Types.Word.ToByteArray((ushort)(varCount * 8), SystemDefault.SiemensByteOrder16));

                // now join the header and the data
                package.Add(value);

                mClient.Send(package.array, package.array.Length, SocketFlags.None);

                int numReceived = mClient.Receive(bReceive, 512, SocketFlags.None);
                if (bReceive[21] != 0xff)
                {
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                }

                return ErrorCode.NoError;
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = exc.Message;
                return LastErrorCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="db"></param>
        /// <param name="startByteAdr"></param>
        /// <param name="bitAdr"></param>
        /// <param name="bitValue"></param>
        /// <returns></returns>
        private ErrorCode WriteBitWithASingleRequest(DataType dataType, int db, int startByteAdr, int bitAdr, bool bitValue)
        {
            byte[] bReceive = new byte[513];
            int varCount = 0;

            try
            {
                var value = new[] { bitValue ? (byte)1 : (byte)0 };
                varCount = value.Length;
                // first create the header
                int packageSize = 35 + value.Length;
                Types.ByteArray package = new Types.ByteArray(packageSize);

                package.Add(new byte[] { 3, 0, 0 });
                package.Add((byte)packageSize);
                package.Add(new byte[] { 2, 0xf0, 0x80, 0x32, 1, 0, 0 });
                package.Add(Types.Word.ToByteArray((ushort)(varCount - 1), SystemDefault.SiemensByteOrder16));
                package.Add(new byte[] { 0, 0x0e });
                package.Add(Types.Word.ToByteArray((ushort)(varCount + 4), SystemDefault.SiemensByteOrder16));
                package.Add(new byte[] { 0x05, 0x01, 0x12, 0x0a, 0x10, 0x01 }); //ending 0x01 is used for writing a sinlge bit
                package.Add(Types.Word.ToByteArray((ushort)varCount, SystemDefault.SiemensByteOrder16));
                package.Add(Types.Word.ToByteArray((ushort)(db), SystemDefault.SiemensByteOrder16));
                package.Add((byte)dataType);
                var overflow = (int)(startByteAdr * 8 / 0xffffU); // handles words with address bigger than 8191
                package.Add((byte)overflow);
                package.Add(Types.Word.ToByteArray((ushort)(startByteAdr * 8 + bitAdr), SystemDefault.SiemensByteOrder16));
                package.Add(new byte[] { 0, 0x03 }); //ending 0x03 is used for writing a sinlge bit
                package.Add(Types.Word.ToByteArray((ushort)(varCount), SystemDefault.SiemensByteOrder16));

                // now join the header and the data
                package.Add(value);

                mClient.Send(package.array, package.array.Length, SocketFlags.None);

                int numReceived = mClient.Receive(bReceive, 512, SocketFlags.None);
                if (bReceive[21] != 0xff)
                {
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                }

                return ErrorCode.NoError;
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = exc.Message;
                return LastErrorCode;
            }
        }

        /// <summary>
        /// Given a S7 variable type (Bool, Word, DWord, etc.), it converts the bytes in the appropriate C# format.
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="bytes"></param>
        /// <param name="varCount"></param>
        /// <param name="bitAdr"></param>
        /// <returns></returns>
        private object ParseBytes(VarType varType, byte[] bytes, int varCount, byte bitAdr = 0)
        {
            if (bytes == null) return null;

            switch (varType)
            {
                case VarType.Byte:
                    if (varCount == 1)
                        return bytes[0];
                    else
                        return bytes;
                case VarType.Word:
                    if (varCount == 1)
                        return PlcTypes.Word.FromByteArray(bytes, SystemDefault.SiemensByteOrder16);
                    else
                        return PlcTypes.Word.ToArray(bytes, SystemDefault.SiemensByteOrder16);
                case VarType.Int:
                    if (varCount == 1)
                        return PlcTypes.Int.FromByteArray(bytes, SystemDefault.SiemensByteOrder16);
                    else
                        return PlcTypes.Int.ToArray(bytes, SystemDefault.SiemensByteOrder16);
                case VarType.DWord:
                    if (varCount == 1)
                        return PlcTypes.DWord.FromByteArray(bytes, SystemDefault.SiemensByteOrder32);
                    else
                        return PlcTypes.DWord.ToArray(bytes, SystemDefault.SiemensByteOrder32);
                case VarType.DInt:
                    if (varCount == 1)
                        return PlcTypes.DInt.FromByteArray(bytes, SystemDefault.SiemensByteOrder32);
                    else
                        return PlcTypes.DInt.ToArray(bytes, SystemDefault.SiemensByteOrder32);
                case VarType.Real:
                    if (varCount == 1)
                        return PlcTypes.Real.FromByteArray(bytes, SystemDefault.SiemensByteOrder32);
                    else
                        return PlcTypes.Real.ToArray(bytes, SystemDefault.SiemensByteOrder32);
                case VarType.Timer:
                    if (varCount == 1)
                        return PlcTypes.Timer.FromByteArray(bytes);
                    else
                        return PlcTypes.Timer.ToArray(bytes);
                case VarType.Counter:
                    if (varCount == 1)
                        return PlcTypes.Counter.FromByteArray(bytes);
                    else
                        return PlcTypes.Counter.ToArray(bytes);
                case VarType.Bit:
                    if (varCount == 1)
                        if (bitAdr > 7)
                            return null;
                        else
                            return PlcTypes.Bit.FromByte(bytes[0], bitAdr);
                    else
                        return PlcTypes.Bit.ToBitArray(bytes);
                case VarType.String:
                    return PlcTypes.String.FromByteArray(bytes);

                case VarType.S7String:
                    return PlcTypes.S7String.FromByteArray(bytes);

                case VarType.S7WString:
                    return PlcTypes.S7WString.FromByteArray(bytes);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Given a S7 variable type (Bool, Word, DWord, etc.), it returns how many bytes to read.
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varCount"></param>
        /// <returns></returns>
        private int VarTypeToByteLength(VarType varType, int varCount = 1)
        {
            switch (varType)
            {
                case VarType.Bit:
                    return varCount; //TODO
                case VarType.Byte:
                    return (varCount < 1) ? 1 : varCount;
         
                case VarType.Word:
                case VarType.Timer:
                case VarType.Int:
                case VarType.Counter:
                    return varCount * 2;
                case VarType.DWord:
                case VarType.DInt:
                case VarType.Real:
                    return varCount * 4;

                case VarType.String:
                    return varCount;
                case VarType.S7String:
                    return ((((varCount + 2) & 1) == 1) ? (varCount + 3) : (varCount + 2));
                case VarType.S7WString:
                    return ((varCount * 2) + 4);
            }
            return 0;
        }
        #endregion
    }

    /// <summary>
    /// 接口实现 
    /// 主要实现IComDriverNetCom接口下的IComDriver<Socket,NetworkCommParam>
    /// </summary>
    public partial class ComS7: sComNetDevice
    {
        /// <summary>
        /// 重写西门子PLC连接，与微软原始Connect不同
        /// </summary>
        /// <returns>连接成功返回ErrorCode.NoError</returns>
        public override ErrorCode Open()
        {
            byte[] bReceive = new byte[256];
            mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            mClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 500);
            if (Connect(mClient) != ErrorCode.NoError)
            {
                return LastErrorCode;
            }
            try
            {
                byte[] bSend1 = { 3, 0, 0, 22, 17, 224, 0, 0, 0, 46, 0, 193, 2, 1, 0, 194, 2, 3, 0, 192, 1, 9 };

                switch (CPU)
                {
                    case CpuType.S7200SMART:
                        //S7200: Chr(193) & Chr(2) & Chr(16) & Chr(0) 'Eigener Tsap
                        bSend1[11] = 193;
                        bSend1[12] = 2;
                        bSend1[13] = 16;
                        bSend1[14] = 0;
                        //S7200: Chr(194) & Chr(2) & Chr(16) & Chr(0) 'Fremder Tsap
                        bSend1[15] = 194;
                        bSend1[16] = 2;
                        bSend1[17] = 16;
                        bSend1[18] = 0;
                        break;
                    case CpuType.S71200:
                    case CpuType.S7300:
                        //S7300: Chr(193) & Chr(2) & Chr(1) & Chr(0)  'Eigener Tsap
                        bSend1[11] = 193;
                        bSend1[12] = 2;
                        bSend1[13] = 1;
                        bSend1[14] = 0;
                        //S7300: Chr(194) & Chr(2) & Chr(3) & Chr(2)  'Fremder Tsap
                        bSend1[15] = 194;
                        bSend1[16] = 2;
                        bSend1[17] = 3;
                        bSend1[18] = (byte)(Rack * 2 * 16 + Slot);
                        break;
                    case CpuType.S7400:
                        //S7400: Chr(193) & Chr(2) & Chr(1) & Chr(0)  'Eigener Tsap
                        bSend1[11] = 193;
                        bSend1[12] = 2;
                        bSend1[13] = 1;
                        bSend1[14] = 0;
                        //S7400: Chr(194) & Chr(2) & Chr(3) & Chr(3)  'Fremder Tsap
                        bSend1[15] = 194;
                        bSend1[16] = 2;
                        bSend1[17] = 3;
                        bSend1[18] = (byte)(Rack * 2 * 16 + Slot);
                        break;
                    case CpuType.S71500:
                        // Eigener Tsap
                        bSend1[11] = 193;
                        bSend1[12] = 2;
                        bSend1[13] = 0x10;
                        bSend1[14] = 0x2;
                        // Fredmer Tsap
                        bSend1[15] = 194;
                        bSend1[16] = 2;
                        bSend1[17] = 0x3;
                        bSend1[18] = (byte)(Rack * 2 * 16 + Slot);
                        break;
                    default:
                        return ErrorCode.WrongCPU_Type;
                }

                mClient.Send(bSend1, 22, SocketFlags.None);
                if (mClient.Receive(bReceive, 22, SocketFlags.None) != 22)
                {
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                }

                byte[] bsend2 = { 3, 0, 0, 25, 2, 240, 128, 50, 1, 0, 0, 255, 255, 0, 8, 0, 0, 240, 0, 0, 3, 0, 3, 1, 0 };
                mClient.Send(bsend2, 25, SocketFlags.None);

                if (mClient.Receive(bReceive, 27, SocketFlags.None) != 27)
                {
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                }
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.ConnectionError;
                LastErrorString = "Couldn't establish the connection to " + DriverItem.ComParam.ComIP + ".\nMessage: " + exc.Message;
                return ErrorCode.ConnectionError;
            }
            return ErrorCode.NoError;
        }
    }

    /// <summary>
    /// 接口实现 - PLC访问
    /// 主要实现IComDriverNetCom下的IComPlcWorker
    /// </summary>
    public partial class ComS7 : IComPlcWorker, IComDriverNetCom
    {
        #region 属性
        /// <summary>
        /// 变量列表
        /// </summary>
        protected ObservableCollection<ModelComPLC> _CommList = 
            new ObservableCollection<ModelComPLC>();
        public ObservableCollection<ModelComPLC> CommList
        {
            get => _CommList; set { _CommList = value; }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 通讯轮询接收到数据
        /// </summary>
        public event Action<object, ObservableCollection<ModelComPLC>> ComData_Received;
        /// <summary>
        /// 通讯启动通知
        /// </summary>
        public event Action<object> Com_Started;
        #endregion

        #region PLC接口方法
        /// <summary>
        /// 通讯连续采样开启
        /// </summary>
        /// <returns></returns>
        public void ComStart()
        {
            if (ComWorking)
                return;
            ComWorking = true;
            AutoSetDataComRange();
            Open();
            ThreadPool.QueueUserWorkItem(QueueRcvWorker, DriverItem);
            if (DriverItem.ServerCmdEn.ToMyString() != "0")
                ThreadPool.QueueUserWorkItem(QueueSendWorker, DriverItem);
            if (Com_Started != null)
                Com_Started(this);
            EventRise_ComNote(EventType.ComReadStarted, string.Format("PLC({0})已启动周期循环", _DriverItem.ComParam.ComIP));
        }

        /// <summary>
        /// 通讯采样停止
        /// </summary>
        /// <returns></returns>
        public void ComStop()
        {
            ComWorking = false;
            Close();
        }

        /// <summary>
        /// 通讯轮训读取队列
        /// </summary>
        /// <param name="state"></param>
        private void QueueRcvWorker(object state)
        {
            while (ComWorking)
            {
                //通讯轮询队列
                try
                {
                    if (GrpRange.Count > 0 && IsConnected)  
                    {
                        lock (GrpRange)
                        {
                            foreach (AddrRange addrRange in GrpRange)
                            {
                                bool ReadFail = false;
                                string strAddrKey = addrRange.addrKey;
                                int iDataBlockID = addrRange.dbNo;
                                int iStartByte = addrRange.min;
                                int iLen = addrRange.len;
                                DataType eDataType = (DataType)strAddrKey.ToMyVarStruct("DataType");
                                byte[] byBuffer = ReadBytes(eDataType, iDataBlockID, iStartByte, iLen);
                                //判定结果
                                if (LastErrorCode != ErrorCode.NoError)
                                    ReadFail = true;
                                Dictionary<string, string> DicVals = ComDataArrayToDic(addrRange, byBuffer, ReadFail);
                                UpdateComDataValueToVarlist(DicVals);
                            }
                        }
                    }

                    if (LastErrorCode != ErrorCode.NoError || !IsConnected)
                    {
                        foreach (ModelComPLC node in _CommList)
                            node.DataValue = string.Empty;
                    }

                    ComDataReceived();

                    if (ComData_Received != null)
                        ComData_Received(this, _CommList);

                    //发送数据
                    try
                    {
                        if (!string.IsNullOrEmpty(_DriverItem?.DriverName))
                            Messenger.Default.Send<ObservableCollection<ModelComPLC>>(_CommList, _DriverItem.DriverName);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error.Write(LOG_TYPE.ERROR, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    LogError(string.Format("[{0}] Error:{1}", _DriverItem.ComParam.ComIP, ex.Message));
                }

                //判定重连机制
                try
                {
                    if (LastErrorCode != ErrorCode.NoError || !IsConnected)
                    {
                        LogError(string.Format("[{0}] Error:{1}", _DriverItem.ComParam.ComIP, LastErrorString));
                        ClearLastError();
                        Close();
                        if (!IsConnected)
                            Open();
                    }
                }
                catch (Exception)
                {

                }

                Thread.Sleep(_DriverItem.ComParam.CycleTime);
            }
        }

        /// <summary>
        /// 数据发送队列
        /// </summary>
        /// <param name="state"></param>
        protected virtual void QueueSendWorker(object state)
        {

        }

        /// <summary>
        /// 从PLC读取单个变量
        /// 通过 LastErrorCode or LastErrorString判别成功与否
        /// </summary>
        /// <param name="variable">ex: "IX0.0", "MX10.0", "MB20", "T45", etc.</param>
        /// <param name="Format"></param>
        /// <returns></returns>
        public object Read(string variable, DataFormat Format = DataFormat.Default)
        {
            return null;
        }

        /// <summary>
        /// 写入PLC格式化数据
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        public ErrorCode Write(string variable, object value, DataFormat Format = DataFormat.Default)
        {
            DataType mDataType;
            int mDB;
            int mByte;
            int mBit;

            string addressLocation;
            byte _byte;
            object objValue;

            string txt = variable.ToUpper();
            txt = txt.Replace(" ", ""); // Remove spaces
            txt = txt.MySmartVToDB();

            try
            {
                switch (txt.Substring(0, 2))
                {
                    case "DB":
                        string[] strings = txt.Split(new char[] { '.' });
                        if (strings.Length < 2)
                            throw new Exception();

                        mDB = int.Parse(strings[0].Substring(2));
                        string dbType = strings[1].Substring(0, 3);
                        int dbIndex = int.Parse(strings[1].Substring(3));

                        switch (dbType)
                        {
                            case "DBB":
                                objValue = Convert.ChangeType(value, typeof(byte));
                                return Write(DataType.DataBlock, mDB, dbIndex, VarType.Byte, (byte)objValue);
                            case "DBW":
                                if (value is short)
                                {
                                    objValue = ((short)value).ConvertToUshort();
                                }
                                else
                                {
                                    objValue = Convert.ChangeType(value, typeof(UInt16));
                                }
                                return Write(DataType.DataBlock, mDB, dbIndex, VarType.Word, (UInt16)objValue);
                            case "DBD":
                                if (value is int)
                                {
                                    return Write(DataType.DataBlock, mDB, dbIndex, VarType.DInt, (Int32)value);
                                }
                                else
                                {
                                    objValue = Convert.ChangeType(value, typeof(UInt32));
                                }
                                return Write(DataType.DataBlock, mDB, dbIndex, VarType.DWord, (UInt32)objValue);
                            case "DBX":
                                mByte = dbIndex;
                                mBit = int.Parse(strings[2]);
                                if (mBit > 7)
                                {
                                    throw new Exception(string.Format("Addressing Error: You can only reference bitwise locations 0-7. Address {0} is invalid", mBit));
                                }

                                return Write(DataType.DataBlock, mDB, mByte, VarType.Bit, value, mBit);
                            case "DBS":
                                // DB-String
                                return Write(DataType.DataBlock, mDB, dbIndex, VarType.S7String, (string)value);
                            default:
                                throw new Exception(string.Format("Addressing Error: Unable to parse address {0}. Supported formats include DBB (byte), DBW (word), DBD (dword), DBX (bitwise), DBS (string).", dbType));
                        }
                    case "EB":
                        // Input Byte
                        objValue = Convert.ChangeType(value, typeof(byte));
                        return Write(DataType.Input, 0, int.Parse(txt.Substring(2)), VarType.Byte, (byte)objValue);
                    case "EW":
                        // Input Word
                        objValue = Convert.ChangeType(value, typeof(UInt16));
                        return Write(DataType.Input, 0, int.Parse(txt.Substring(2)), VarType.Word, (UInt16)objValue);
                    case "ED":
                        // Input Double-Word
                        objValue = Convert.ChangeType(value, typeof(UInt32));
                        return Write(DataType.Input, 0, int.Parse(txt.Substring(2)), VarType.DWord, (UInt32)objValue);
                    case "AB":
                        // Output Byte
                        objValue = Convert.ChangeType(value, typeof(byte));
                        return Write(DataType.Output, 0, int.Parse(txt.Substring(2)), VarType.Byte, (byte)objValue);
                    case "AW":
                        // Output Word
                        objValue = Convert.ChangeType(value, typeof(UInt16));
                        return Write(DataType.Output, 0, int.Parse(txt.Substring(2)), VarType.Word, (UInt16)objValue);
                    case "AD":
                        // Output Double-Word
                        objValue = Convert.ChangeType(value, typeof(UInt32));
                        return Write(DataType.Output, 0, int.Parse(txt.Substring(2)), VarType.DWord, (UInt32)objValue);
                    case "MB":
                        // Memory Byte
                        objValue = Convert.ChangeType(value, typeof(byte));
                        return Write(DataType.Memory, 0, int.Parse(txt.Substring(2)), VarType.Byte, (byte)objValue);
                    case "MW":
                        // Memory Word
                        objValue = Convert.ChangeType(value, typeof(UInt16));
                        return Write(DataType.Memory, 0, int.Parse(txt.Substring(2)), VarType.Word, (UInt16)objValue);
                    case "MD":
                        // Memory Double-Word
                        return Write(DataType.Memory, 0, int.Parse(txt.Substring(2)), VarType.DInt, value);
                    default:
                        switch (txt.Substring(0, 1))
                        {
                            case "E":
                            case "I":
                                // Input
                                mDataType = DataType.Input;
                                break;
                            case "A":
                            case "O":
                                // Output
                                mDataType = DataType.Output;
                                break;
                            case "M":
                                // Memory
                                mDataType = DataType.Memory;
                                break;
                            case "T":
                                // Timer
                                return Write(DataType.Timer, 0, int.Parse(txt.Substring(1)), VarType.Timer, (double)value);
                            case "Z":
                            case "C":
                                // Counter
                                return Write(DataType.Counter, 0, int.Parse(txt.Substring(1)), VarType.Counter, (short)value);
                            default:
                                throw new Exception(string.Format("Unknown variable type {0}.", txt.Substring(0, 1)));
                        }

                        addressLocation = txt.Substring(1);
                        int decimalPointIndex = addressLocation.IndexOf(".");
                        if (decimalPointIndex == -1)
                        {
                            throw new Exception(string.Format("Cannot parse variable {0}. Input, Output, Memory Address, Timer, and Counter types require bit-level addressing (e.g. I0.1).", addressLocation));
                        }

                        mByte = int.Parse(addressLocation.Substring(0, decimalPointIndex));
                        mBit = int.Parse(addressLocation.Substring(decimalPointIndex + 1));
                        if (mBit > 7)
                        {
                            throw new Exception(string.Format("Addressing Error: You can only reference bitwise locations 0-7. Address {0} is invalid", mBit));
                        }

                        _byte = (byte)Read(mDataType, 0, mByte, VarType.Byte, 1);
                        if ((int)value == 1)
                            _byte = (byte)(_byte | (byte)Math.Pow(2, mBit));      // Set bit
                        else
                            _byte = (byte)(_byte & (_byte ^ (byte)Math.Pow(2, mBit))); // Reset bit

                        return Write(mDataType, 0, mByte, VarType.Bit, (byte)_byte);
                }
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WrongVarFormat;
                LastErrorString = "The variable'" + variable + "' could not be parsed. Please check the syntax and try again.\nException: " + exc.Message;
                return LastErrorCode;
            }
        }

        #endregion
    }

    /// <summary>
    /// 西门子S7通信协议二层操作封装
    /// </summary>
    public partial class ComS7
    {
        /// <summary>
        /// 通讯最大字节数
        /// </summary>
        private const int MaxReadByte = 200;
        /// <summary>
        /// 
        /// </summary>
        private string ErrorMsgLast = string.Empty;
        /// <summary>
        /// 分区列表
        /// </summary>
        private List<AddrRange> GrpRange = new List<AddrRange>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Drv"></param>
        public ComS7(DriverItem<NetworkCommParam> Drv)
        {
            if (!Config(Drv))
                return;
            if (ValidateCpuType(_DriverItem.DriverPtl) != ErrorCode.NoError)
                return;
            Rack = 0;
            if (CPU == CpuType.S7300 || CPU == CpuType.S7400)
                Slot = 2;
            Slot = 0;
        }

        /// <summary>
        /// 自动确定通讯分段地址范围
        /// </summary>
        protected virtual void AutoSetDataComRange()
        {
            if (_CommList == null)
                return;
            lock (_CommList)
            {
                List<AddrRange> TempGrpRange = new List<AddrRange>();
                Dictionary<string, int> DicVarStartByte = new Dictionary<string, int>();
                foreach (ModelComPLC node in _CommList)
                {
                    VarStruct sVarNode = new VarStruct()
                    {
                        DataType = node.AddrType.ToMyString(),
                        VarKey = node.DataAddr.ToMyVarStruct("VarKey").ToMyString(),
                        ByteStart = (int)node.DataAddr.ToMyVarStruct("ByteStart"),
                        BitOffset = (int)node.DataAddr.ToMyVarStruct("BitOffset")
                    };
                    int StartByte = 0;
                    TempGrpRange = S7Common.BuildDataAddrRange(TempGrpRange, sVarNode, MaxReadByte, out StartByte);
                    if (!DicVarStartByte.ContainsKey(node.DataAddr))
                        DicVarStartByte.Add(node.DataAddr, StartByte);
                }
                Dictionary<string, ModelComPLC> DicVarNode = _CommList.Distinct().ToDictionary(x => x.DataAddr);
                foreach (string strAddr in DicVarStartByte.Keys)
                {
                    if (DicVarNode.ContainsKey(strAddr))
                        DicVarNode[strAddr].StartByteOfRange = DicVarStartByte[strAddr].ToMyString();
                }
                if (TempGrpRange != null)
                    GrpRange = TempGrpRange;
            }
        }

        /// <summary>
        /// 通讯接收到数据
        /// </summary>
        protected virtual void ComDataReceived()
        {
            //仅为框架，由继承者实现
        }

        /// <summary>
        /// 向PLC写队列数据请求
        /// </summary>
        protected virtual void QueueDataWriteRequest()
        {
            //仅为框架，由继承者实现
        }

        /// <summary>
        /// 验证CPU信息
        /// </summary>
        /// <param name="cpuType"></param>
        /// <returns></returns>
        private ErrorCode ValidateCpuType(string cpuType)
        {
            try
            {
                CPU = (CpuType)Enum.Parse(typeof(CpuType), cpuType);
            }
            catch (Exception)
            {
                LastErrorCode = ErrorCode.WrongCPU_Type;
                LastErrorString = "CPU类型设定错误";
                return LastErrorCode;
            }
            return ErrorCode.NoError;
        }

        /// <summary>
        /// 将通讯反馈数据装载到变量列表中
        /// </summary>
        /// <param name="DataValDic"></param>
        /// <returns></returns>
        private bool UpdateComDataValueToVarlist(Dictionary<string, string> DataValDic)
        {
            if (DataValDic == null)
                return false;
            lock (_CommList)
            {
                Dictionary<string, ModelComPLC> VarNodesDic = _CommList.Distinct().ToDictionary(x => x.ID);
                foreach (string strAddr in DataValDic.Keys)
                {
                    if (VarNodesDic.ContainsKey(strAddr))
                        VarNodesDic[strAddr].DataValue = DataValDic[strAddr];
                }
            }
            return true;
        }

        /// <summary>
        /// 接收到的数据对应解析
        /// </summary>
        /// <param name="addrRange"></param>
        /// <param name="byBuffer"></param>
        /// <returns></returns>
        private Dictionary<string, string> ComDataArrayToDic(AddrRange addrRange, byte[] byBuffer, bool ReadFail = false)
        {
            Dictionary<string, string> ComAddrVal = new Dictionary<string, string>();
            lock (_CommList)
            {
                //x.V_ADDR == addrRange.addrKey
                List<ModelComPLC> ListVarNodes = _CommList.Distinct().Where(x => x.StartByteOfRange == addrRange.min.ToString() && x.DataAddr.Contains(addrRange.addrKey)).ToList();
                foreach (ModelComPLC node in ListVarNodes)
                {
                    string strVal = string.Empty;
                    string strVarNodeID = node.ID.ToMyString();
                    string strIsAlarm = node.DataUnitType.ToMyString().ToUpper().Contains("ALARM") ? "1" : "0";
                    string strComAddr = node.DataAddr.ToMyString();
                    if (LastErrorCode == ErrorCode.NoError)
                    {
                        VarStruct sVarNode = new VarStruct();
                        sVarNode.DataType = node.AddrType.ToMyString();
                        sVarNode.VarKey = strComAddr.ToMyVarStruct("VarKey").ToMyString();
                        sVarNode.ByteStart = (int)strComAddr.ToMyVarStruct("ByteStart");
                        sVarNode.BitOffset = (int)strComAddr.ToMyVarStruct("BitOffset");
                        byte[] byTemp;
                        int index = sVarNode.ByteStart - addrRange.min;
                        //根据变量类型按需取值
                        if (!ReadFail)
                        {
                            string strDataType = sVarNode.DataType.ToMyString().ToUpper();
                            switch (strDataType)
                            {
                                case "BOOL":
                                    byTemp = new byte[1];
                                    Array.Copy(byBuffer, index, byTemp, 0, 1);
                                    BitArray Bits = new BitArray(new byte[] { byTemp[0] });
                                    strVal = Bits.Get(sVarNode.BitOffset) ? "True" : "False";
                                    break;
                                case "BYTE":
                                    byTemp = new byte[1];
                                    Array.Copy(byBuffer, index, byTemp, 0, 1);
                                    strVal = byTemp[0].ToMyString();
                                    break;
                                case "INT":
                                    byTemp = new byte[2];
                                    Array.Copy(byBuffer, index, byTemp, 0, 2);
                                    strVal = PlcTypes.Int.FromByteArray(byTemp, SystemDefault.SiemensByteOrder16).ToMyString();
                                    break;
                                case "DINT":
                                    byTemp = new byte[4];
                                    Array.Copy(byBuffer, index, byTemp, 0, 4);
                                    strVal = PlcTypes.DInt.FromByteArray(byTemp, SystemDefault.SiemensByteOrder32).ToMyString();
                                    break;
                                case "WORD":
                                    byTemp = new byte[2];
                                    Array.Copy(byBuffer, index, byTemp, 0, 2);
                                    strVal = PlcTypes.Word.FromByteArray(byTemp, SystemDefault.SiemensByteOrder16).ToMyString();
                                    break;
                                case "DWORD":
                                    byTemp = new byte[4];
                                    Array.Copy(byBuffer, index, byTemp, 0, 4);
                                    strVal = PlcTypes.DWord.FromByteArray(byTemp, SystemDefault.SiemensByteOrder32).ToMyString();
                                    break;
                                case "REAL":
                                    byTemp = new byte[4];
                                    Array.Copy(byBuffer, index, byTemp, 0, 4);
                                    strVal = PlcTypes.Real.FromByteArray(byTemp, SystemDefault.SiemensByteOrder32).ToString("f3");
                                    break;
                                default:
                                    if (strDataType.Contains("STRING"))
                                    {
                                        int iLen = strDataType.MidString("STRING[", "]").ToMyInt();
                                        if (iLen > 0)
                                        {
                                            byTemp = new byte[iLen + 2];
                                            Array.Copy(byBuffer, index, byTemp, 0, iLen + 2);
                                            strVal = PlcTypes.S7String.FromByteArray(byTemp);
                                        }
                                    }
                                    break;
                            }
                        }
                        if (!ComAddrVal.ContainsKey(strComAddr))
                            ComAddrVal.Add(strComAddr, strVal);

                        //更新所有变量绑定的数据源
                        if (_CommList != null)
                        {
                            ModelComPLC DestNode = _CommList.Where(x => x.ID == strVarNodeID).ToList()[0];
                            DestNode.DataValue = strVal;
                        }
                    }
                }
            }
            return ComAddrVal;
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="ErrorMsg"></param>
        public void LogError(string ErrorMsg)
        {
            try
            {
                if (ErrorMsgLast != ErrorMsg)
                {
                    Logger.Error.Write(LOG_TYPE.ERROR, ErrorMsg);
                    ErrorMsgLast = ErrorMsg;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
