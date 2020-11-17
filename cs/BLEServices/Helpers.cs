using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;
using Windows.Security.Cryptography;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace BLE_Hackme.BLEServices
{
    /// <summary>
    /// Extension class for byte
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Converts byte array to string
        /// </summary>
        /// <param name="array">Byte array to covert</param>
        /// <returns>string equivalent of the byte array</returns>
        public static string BytesToString(this byte[] array)
        {
            var result = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                result.Append($"{array[i]:X2}");
                if (i < array.Length - 1)
                {
                    result.Append(" ");
                }
            }

            return result.ToString();
        }

        public static IBuffer HexStringToIBuffer(string data)
        {
            data = data.Replace("-", "");

            int NumberChars = data.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);
            }

            DataWriter writer = new DataWriter();
            writer.WriteBytes(bytes);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(bool data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteBoolean(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(byte data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteByte(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(byte[] data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteBytes(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(double data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteDouble(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(Int16 data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteInt16(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(Int32 data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteInt32(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(Int64 data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteInt64(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(Single data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteSingle(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(UInt16 data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteUInt16(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(UInt32 data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteUInt32(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(UInt64 data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteUInt64(data);
            return writer.DetachBuffer();
        }

        public static IBuffer ToIBuffer(string data)
        {
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.LittleEndian;
            writer.WriteString(data);
            return writer.DetachBuffer();
        }

        public static string ToHexString(IBuffer buffer)
        {
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);
            return BitConverter.ToString(data);
        }




        /// <summary>
        /// Get Characteristics from the Characteristics Result
        /// </summary>
        /// <param name="result">Gatt Characteristics Result</param>
        /// <param name="characteristics">Gatt characteristics</param>
        public static void GetCharacteristicsFromResult(GattLocalCharacteristicResult result, ref GattLocalCharacteristic characteristics)
        {
            if (result.Error == BluetoothError.Success)
            {
                characteristics = result.Characteristic;
            }
            else
            {
                Debug.WriteLine(result.Error.ToString());
            }
        }

    }


   // used by GenericGattCharacteristic
    public static class DispatcherTaskExtensions
    {
        public static async Task<T> RunTaskAsync<T>(
            this CoreDispatcher dispatcher,
            Func<Task<T>> func,
            CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            await dispatcher.RunAsync(priority, async () =>
            {
                try
                {
                    taskCompletionSource.SetResult(await func());
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            return await taskCompletionSource.Task;
        }

        // There is no TaskCompletionSource<void> so we use a bool that we throw away.
        public static async Task RunTaskAsync(this CoreDispatcher dispatcher,
            Func<Task> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal) =>
            await RunTaskAsync(dispatcher, async () => { await func(); return false; }, priority);
    }

}
