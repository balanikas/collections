using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Collections
{
    internal class ThreadSafeRandom
    {
        private static readonly Random Global = new Random();

        [ThreadStatic] private static Random _local;

        public ThreadSafeRandom()
        {
            if (_local == null)
            {
                int seed;
                lock (Global)
                {
                    seed = Global.Next();
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
                    return NextSByte();
                }
                case "Byte":
                {
                    return NextByte();
                }
                case "Int16":
                {
                    return NextInt16();
                }
                case "UInt16":
                {
                    return NextUInt16();
                }
                case "Int32":
                {
                    return NextInt32();
                }
                case "UInt32":
                {
                    return NextUInt32();
                }
                case "Int64":
                {
                    return NextInt64();
                }
                case "UInt64":
                {
                    return NextUInt64();
                }
                case "Single":
                {
                    return NextSingle();
                }
                case "Double":
                {
                    return NextDouble();
                }
                case "Decimal":
                {
                    return NextDecimal();
                }
                case "Boolean":
                {
                    return NextBoolean();
                }
                case "Char":
                {
                    return NextChar();
                }
                case "Object":
                {
                    return new object();
                }
                case "Char*":
                case "String":
                {
                    return NextString();
                }


                case "SByte[]":
                {
                    return NextArray(_local.Next(1, 3), NextSByte);
                }
                case "Byte[]":
                {
                    return NextArray(_local.Next(1, 3), NextByte);
                }
                case "Int16[]":
                {
                    return NextArray(_local.Next(1, 3), NextInt16);
                }
                case "UInt16[]":
                {
                    return NextArray(_local.Next(1, 3), NextUInt16);
                }
                case "Int32[]":
                {
                    return NextArray(_local.Next(1, 3), NextInt32);
                }
                case "UInt32[]":
                {
                    return NextArray(_local.Next(1, 3), NextUInt32);
                }
                case "Int64[]":
                {
                    return NextArray(_local.Next(1, 3), NextInt64);
                }
                case "UInt64[]":
                {
                    return NextArray(_local.Next(1, 3), NextUInt64);
                }
                case "Single[]":
                {
                    return NextArray(_local.Next(1, 3), NextSingle);
                }
                case "Double[]":
                {
                    return NextArray(_local.Next(1, 3), NextDouble);
                }
                case "Decimal[]":
                {
                    return NextArray(_local.Next(1, 3), NextDecimal);
                }
                case "Boolean[]":
                {
                    return NextArray(_local.Next(1, 3), NextBoolean);
                }
                case "Char[]":
                {
                    return NextArray(_local.Next(1, 3), NextChar);
                }
                case "Object[]":
                {
                    return NextArray(_local.Next(1, 3), NextString);
                }
                case "Char*[]":
                case "String[]":
                {
                    return NextArray(_local.Next(1, 3), NextString);
                }
                default:
                    throw new ArgumentException(string.Format("unsupported param type '{0}'",typeName));
            }
        }

        private T[] NextArray<T>(int sizeOfArray, Func<T> func)
        {
            var items = new T[sizeOfArray];
            for (int i = 0; i < sizeOfArray; i++)
            {
                items[i] = func();
            }
            return items;
        }

        private int NextInt32Unchecked()
        {
            unchecked
            {
                int firstBits = _local.Next(0, 1 << 4) << 28;
                int lastBits = _local.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }

        private string NextString()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _local.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        private bool NextBoolean()
        {
            var bytes = new byte[sizeof(bool)];
            _local.NextBytes(bytes);
            return BitConverter.ToBoolean(bytes, 0);
        }

        private byte NextByte()
        {
            var bytes = new byte[sizeof(byte)];
            _local.NextBytes(bytes);
            return bytes[0];
        }

        private sbyte NextSByte()
        {
            var bytes = new byte[sizeof(sbyte)];
            _local.NextBytes(bytes);
            return (sbyte)bytes[0];
        }

        private short NextInt16()
        {
            var bytes = new byte[sizeof(Int16)];
            _local.NextBytes(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }
        private ushort NextUInt16()
        {
            var bytes = new byte[sizeof(UInt16)];
            _local.NextBytes(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        private int NextInt32()
        {
            var bytes = new byte[sizeof(Int32)];
            _local.NextBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        private uint NextUInt32()
        {
            var bytes = new byte[sizeof(UInt32)];
            _local.NextBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private long NextInt64()
        {
            var bytes = new byte[sizeof(Int64)];
            _local.NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
        private ulong NextUInt64()
        {
            var bytes = new byte[sizeof(UInt64)];
            _local.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        private float NextSingle()
        {
            var bytes = new byte[sizeof(float)];
            _local.NextBytes(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        private double NextDouble()
        {
            var bytes = new byte[sizeof(double)];
            _local.NextBytes(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

        private decimal NextDecimal()
        {
            var scale = (byte)_local.Next(29);
            bool sign = _local.Next(2) == 1;
            return new decimal(NextInt32Unchecked(),
                NextInt32Unchecked(),
                NextInt32Unchecked(),
                sign,
                scale);
        }

        private char NextChar()
        {
            var bytes = new byte[sizeof(char)];
            _local.NextBytes(bytes);
            return BitConverter.ToChar(bytes, 0);
        }
    }

     
}