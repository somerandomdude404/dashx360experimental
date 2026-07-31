using System.Windows.Forms.Integration;
using Microsoft.Xna.Framework;
using XboxMetroLauncher.Models;

namespace XboxMetroLauncher.Controls
{
    public class MonoGameHost : WindowsFormsHost
    {
        private readonly AvatarGameBridge _game;

        public MonoGameHost()
        {
            _game = new AvatarGameBridge();
            this.Child = _game.Window.Form;
        }

        public void UpdateAvatar(byte[] descriptionBuffer)
        {
            var desc = AvatarDescription.CreateFromBuffer(descriptionBuffer);
            _game.UpdateAvatar(desc);
        }
    }

    internal class AvatarGameBridge : Game
    {
        private GraphicsDeviceManager _graphics;
        private AvatarRenderer _renderer;

        public AvatarGameBridge()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            Window.Form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Window.Form.TopLevel = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _renderer = new AvatarRenderer(GraphicsDevice, Content);
            _renderer.LoadAvatar(AvatarDescription.CreateRandom()); // Default load
        }

        public void UpdateAvatar(AvatarDescription desc)
        {
            _renderer.LoadAvatar(desc);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            Matrix view = Matrix.CreateLookAt(new Vector3(0, 2, 4), new Vector3(0, 1, 0), Vector3.Up);
            Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 600f/800f, 0.1f, 100f);
            Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            _renderer?.Draw(world, view, proj);
            base.Draw(gameTime);
        }
    }
}
