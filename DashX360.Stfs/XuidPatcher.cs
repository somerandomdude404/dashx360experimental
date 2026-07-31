using System;
using System.IO;

namespace DashX360.Stfs
{
    public static class XuidPatcher
    {
        // Patches the License ID (XUID) at offset 0x022C in the STFS header
        public static void PatchXuid(string itemPath, long profileXuid)
        {
            using var fs = new FileStream(itemPath, FileMode.Open, FileAccess.ReadWrite);
            byte[] xuidBytes = BitConverter.GetBytes(profileXuid);
            
            fs.Seek(0x022C, SeekOrigin.Begin);
            fs.Write(xuidBytes, 0, 8);
            
            // Note: For CON packages, the RSA signature at 0x01AC must be re-signed 
            // with the console's private key for it to work on a real Xbox 360.
            // For local PC rendering, this is ignored.
        }
    }
}
