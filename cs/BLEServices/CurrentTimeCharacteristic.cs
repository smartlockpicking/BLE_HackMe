
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Power;
using Windows.Security.Authentication.Identity.Provider;
using Windows.Storage.Streams;
using Windows.System.Threading;

namespace BLE_Hackme.BLEServices
{
    public class CurrentTimeCharacteristic : GenericGattCharacteristic
    {
        private ThreadPoolTimer m_timeUpdate = null;

        public CurrentTimeCharacteristic(GattLocalCharacteristic characteristic, GenericGattService service) : base(characteristic, service)
        {
            UpdateCurrentTimeValue();
        }

        public override void NotifyValue()
        {
            UpdateCurrentTimeValue();
            base.NotifyValue();
        }

        protected override bool ReadRequested(GattSession session, GattReadRequest request)
        {
            UpdateCurrentTimeValue();
            request.RespondWithValue(Value);
            return true;
        }


        protected override void Characteristic_SubscribedClientsChanged(GattLocalCharacteristic sender, object args)
        {
            lock (this)
            {
                if (sender.SubscribedClients.Count == 0)
                {
                    if (m_timeUpdate != null)
                    {
                        m_timeUpdate.Cancel();
                        m_timeUpdate = null;
                    }
                }
                else if (m_timeUpdate == null)
                {
                    m_timeUpdate = ThreadPoolTimer.CreatePeriodicTimer(
                        (source) =>
                        {
                            UpdateCurrentTimeValue();
                            NotifyValue();
                        },
                        // update time via notification every minute
                        TimeSpan.FromMinutes(1));
                }
            }

            base.Characteristic_SubscribedClientsChanged(sender, args);
        }

        private void UpdateCurrentTimeValue()
        {
            // Quicklock stores data as seconds counted from 2000-01-01 00:00:00
            DateTime currentDateTime = DateTime.Now;
            DateTime quickLockEpochStart = new DateTime(2000,1,1);

            var currentSeconds = (int)(currentDateTime - quickLockEpochStart).TotalSeconds;

            Debug.WriteLine($"Current time for quicklock (seconds counted from 2000-01-01): {currentSeconds}");

            Value = Helpers.ToIBuffer(currentSeconds);
        }

    }
}
