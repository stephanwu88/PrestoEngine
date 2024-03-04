using Engine.Common;
using System.Collections.Generic;

namespace Engine.Access
{
    public class USBKey
    {
        public void GetUSB()
        {
            List<DiskDriveItem> items = WmiService.Default.AccessiableUSBDiskDriveItems;
        }
    }
}
