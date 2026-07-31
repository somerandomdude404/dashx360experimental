using System.Windows;
using System.Windows.Controls;
using DashX360.Avatar.Core;

namespace XboxMetroLauncher.Views
{
    public partial class AvatarEditorView : Window
    {
        private AvatarDescription _currentAvatar;

        public AvatarEditorView()
        {
            InitializeComponent();
            _currentAvatar = GpdAvatarReader.ReadFromGpd("profile/FFFE07D1.gpd");
            RefreshRenderer();
        }

        private void OnSettingsChanged(object sender, RoutedEventArgs e)
        {
            // Apply UI changes to the AvatarDescription buffer
            _currentAvatar = AvatarDescription.CreateRandom(); // STUB: Apply actual UI values
            RefreshRenderer();
        }

        private void RefreshRenderer()
        {
            // Push the updated 1021-byte buffer to the MonoGame renderer
            AvatarRenderHost.UpdateAvatar(_currentAvatar.Description);
        }

        private void OpenItemPicker(object sender, RoutedEventArgs e)
        {
            // Open a dialog listing items from the extracted archive.org collection
            // User selects a GUID, patch it into the AvatarDescription clothing slot.
        }

        private void SaveAvatar(object sender, RoutedEventArgs e)
        {
            GpdAvatarReader.WriteToGpd("profile/FFFE07D1.gpd", _currentAvatar);
            MessageBox.Show("Avatar saved to local profile!", "DashX360", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
