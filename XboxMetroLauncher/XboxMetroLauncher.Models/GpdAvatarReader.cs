namespace XboxMetroLauncher.Models
{
    // Reads FFFE07D1.gpd (Dashboard Game Profile Data) from an Xbox 360 profile
    public static class GpdAvatarReader
    {
        public const uint AvatarInfoSettingId = 0x63E80044;
        public const uint AvatarMetadataSettingId = 0x63E80068;

        public static AvatarDescription ReadFromGpd(string gpdPath)
        {
            // STUB: Parse XDBF format to find the setting entry and extract the 1021 bytes.
            // If not found, return a random default.
            return AvatarDescription.CreateRandom();
        }

        public static void WriteToGpd(string gpdPath, AvatarDescription desc)
        {
            // STUB: Write the 1021-byte buffer back to the 0x63E80044 setting entry.
        }
    }
}
