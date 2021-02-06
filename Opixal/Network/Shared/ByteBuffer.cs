using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opixal.Network.Shared
{
    public class ByteBuffer : IDisposable
    {
        private readonly List<byte> Buffer;
        private byte[] ReadBuffer;
        private int ReadPosition;
        private bool BufferUpdated = false;

        public ByteBuffer()
        {
            Buffer = new List<byte>();
            ReadPosition = 0;
        }

        public int GetReadPosition()
        {
            return ReadPosition;
        }

        public byte[] ToArray()
        {
            return Buffer.ToArray();
        }

        public int Count()
        {
            return Buffer.Count();
        }

        public int Length()
        {
            return Count() - ReadPosition;
        }

        public void Clear()
        {
            Buffer.Clear();
            ReadPosition = 0;
        }

        public void WriteByte(byte input)
        {
            Buffer.Add(input);
            BufferUpdated = true;
        }

        public void WriteBytes(byte[] input)
        {
            Buffer.AddRange(input);
            BufferUpdated = true;
        }

        public void WriteShort(short input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }

        public void WriteInteger(int input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }

        public void WriteLong(long input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }

        public void WriteFloat(float input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }

        public void WriteBool(bool input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }

        public void WriteString(string input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input.Length));
            Buffer.AddRange(Encoding.ASCII.GetBytes(input));
            BufferUpdated = true;
        }

        public byte ReadByte(bool Peek = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                byte value = ReadBuffer[ReadPosition];
                if (Peek && Buffer.Count > ReadPosition)
                {
                    ReadPosition += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("The 'byte' value was not contained in ByteBuffer");
            }
        }

        public byte[] ReadBytes(int Length, bool Peek = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                byte[] value = Buffer.GetRange(ReadPosition, Length).ToArray();
                if (Peek)
                {
                    ReadPosition += Length;
                }

                return value;
            }
            else
            {
                throw new Exception("The 'byte[]' value was not contained in ByteBuffer");
            }
        }

        public short ReadShort(bool Peek = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                short value = BitConverter.ToInt16(ReadBuffer, ReadPosition);
                if (Peek && Buffer.Count > ReadPosition)
                {
                    ReadPosition += 2;
                }

                return value;
            }
            else
            {
                throw new Exception("The 'short' value was not contained in ByteBuffer");
            }
        }

        public int ReadInteger(bool Peek = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                int value = BitConverter.ToInt32(ReadBuffer, ReadPosition);
                if (Peek && Buffer.Count > ReadPosition)
                {
                    ReadPosition += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("The 'int' value was not contained in ByteBuffer");
            }
        }

        public long ReadLong(bool Peek = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                long value = BitConverter.ToInt64(ReadBuffer, ReadPosition);
                if (Peek && Buffer.Count > ReadPosition)
                {
                    ReadPosition += 8;
                }

                return value;
            }
            else
            {
                throw new Exception("The 'long' value was not contained in ByteBuffer");
            }
        }

        public float ReadFloat(bool Peek = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                float value = BitConverter.ToSingle(ReadBuffer, ReadPosition);
                if (Peek && Buffer.Count > ReadPosition)
                {
                    ReadPosition += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("The 'float' value was not contained in ByteBuffer");
            }
        }

        public bool ReadBool(bool Peek = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                bool value = BitConverter.ToBoolean(ReadBuffer, ReadPosition);
                if (Peek && Buffer.Count > ReadPosition)
                {
                    ReadPosition += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("The 'bool' value was not contained in ByteBuffer");
            }
        }

        public string ReadString(bool Peek = true)
        {
            try
            {
                int lenght = ReadInteger(true);

                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                string value = Encoding.ASCII.GetString(ReadBuffer, ReadPosition, lenght);
                if (Peek && Buffer.Count > ReadPosition)
                {
                    if (value.Length > 0)
                    {
                        ReadPosition += lenght;
                    }
                }
                return value;
            }
            catch (Exception)
            {
                throw new Exception("The 'string' value was not contained in ByteBuffer");
            }
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Buffer.Clear();
                    ReadPosition = 0;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}