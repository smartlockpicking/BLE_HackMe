using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BLE_Hackme.BLEServices
{
    class iBeacon
    {
        public Guid Uuid { get; private set; }
        public ushort Major { get; private set; }
        public ushort Minor { get; private set; }
        public byte RSSI { get; private set; }

        public iBeacon(Guid uuid, ushort major, ushort minor, byte rssi)
        {
            Uuid = uuid;
            Major = major;
            Minor = minor;
            RSSI = rssi;
        }

        public IBuffer getBuffer()
        {
            var writer = new DataWriter();

            writer.WriteByte(0x02); // type: 0x02 = iBeacon
            writer.WriteByte(0x15); // length 21 bytes

            writer.WriteBytes(Uuid.ToByteArray());
            writer.WriteUInt16(Major);
            writer.WriteUInt16(Minor);
            writer.WriteByte(RSSI);

            return writer.DetachBuffer();
        }

    }
}
