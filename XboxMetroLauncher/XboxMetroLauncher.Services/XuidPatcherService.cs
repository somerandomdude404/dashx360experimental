using System;
using System.IO;

namespace XboxMetroLauncher.Services
{
    public static class XuidPatcherService
    {
        // Patches the License ID (XUID) at offset 0x022C in the STFS header
        public static void PatchXuid(string itemPath, long profileXuid)
        {
            using var fs = new FileStream(itemPath, FileMode.Open, FileAccess.ReadWrite);
            byte[] xuidBytes = BitConverter.GetBytes(profileXuid);
            
            fs.Seek(0x022C, SeekOrigin.Begin);
            fs.Write(xuidBytes, 0, 8);
        }
    }
}
