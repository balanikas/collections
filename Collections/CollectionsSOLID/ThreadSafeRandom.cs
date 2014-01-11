using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace CollectionsSOLID
{
    internal class ThreadSafeRandom
    {
        private static readonly Random _global = new Random();

        [ThreadStatic]
        private static Random _local;

        public ThreadSafeRandom()
        {
           
            if(_local == null)
            {
                int seed;
                lock(_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }
        }


        public object RandomizeParamValue(string typeName)
        {
           
            switch (typeName)
            {
                case "SByte":
                    {
                        var bytes = new byte[sizeof(sbyte)];
                        _local.NextBytes(bytes);
                        return (sbyte)bytes[0];
                    }
                case "Byte":
                    {
                        var bytes = new byte[sizeof(byte)];
                        _local.NextBytes(bytes);
                        return bytes[0];
                    }
                case "Int16":
                    {

                        var bytes = new byte[sizeof(Int16)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToInt16(bytes, 0);
                    }
                case "UInt16":
                    {
                        var bytes = new byte[sizeof(UInt16)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToUInt16(bytes, 0);
                    }
                case "Int32":
                    {
                        var bytes = new byte[sizeof(Int32)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToInt32(bytes, 0);
                    }
                case "UInt32":
                    {
                        var bytes = new byte[sizeof(UInt32)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToUInt32(bytes, 0);
                    }
                case "Int64":
                    {
                        var bytes = new byte[sizeof(Int64)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToInt64(bytes, 0);
                    }
                case "UInt64":
                    {
                        var bytes = new byte[sizeof(UInt64)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToUInt64(bytes, 0);
                    }
                case "Single":
                    {
                        var bytes = new byte[sizeof(float)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToSingle(bytes, 0);
                    }
                case "Double":
                    {
                        var bytes = new byte[sizeof(double)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToDouble(bytes, 0);
                    }
                case "Decimal":
                    {
                        byte scale = (byte)_local.Next(29);
                        bool sign = _local.Next(2) == 1;
                        return new decimal(NextInt32(),
                                           NextInt32(),
                                           NextInt32(),
                                           sign,
                                           scale);
                    }
                case "Boolean":
                    {
                        var bytes = new byte[sizeof(bool)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToBoolean(bytes, 0);
                    }
                case "Char":
                    {
                        var bytes = new byte[sizeof(char)];
                        _local.NextBytes(bytes);
                        return BitConverter.ToChar(bytes, 0);
                    }
                case "Object":
                    {
                        return new object();
                    }
                case "Char*":
                case "String":
                    {
                        var size = 10;
                        StringBuilder builder = new StringBuilder();
                        char ch;
                        for (int i = 0; i < size; i++)
                        {
                            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _local.NextDouble() + 65)));
                            builder.Append(ch);
                        }

                        return builder.ToString();
                    }
                default:
                    return null;
            }

        }

        private int NextInt32()
        {
            unchecked
            {
                int firstBits = _local.Next(0, 1 << 4) << 28;
                int lastBits = _local.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }
    }
}
