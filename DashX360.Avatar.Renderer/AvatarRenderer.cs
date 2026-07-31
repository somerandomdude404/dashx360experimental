using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DashX360.Avatar.Core;
using System.Collections.Generic;

namespace DashX360.Avatar.Renderer
{
    public class AvatarRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly ContentManager _content;
        
        private Model _bodyMesh;
        private Dictionary<string, Model> _clothingMeshes = new();
        private Matrix[] _boneTransforms;

        public AvatarDescription Description { get; private set; }

        public AvatarRenderer(GraphicsDevice graphicsDevice, ContentManager content)
        {
            _graphicsDevice = graphicsDevice;
            _content = content;
        }

        public void LoadAvatar(AvatarDescription desc)
        {
            Description = desc;
            _bodyMesh = _content.Load<Model>($"Avatars/Body_{desc.BodyType}");
            _boneTransforms = new Matrix[_bodyMesh.Bones.Count];
        }

        public void Draw(Matrix world, Matrix view, Matrix projection)
        {
            // Copy bone transforms
            _bodyMesh.CopyAbsoluteBoneTransformsTo(_boneTransforms);

            // Draw Body
            foreach (ModelMesh mesh in _bodyMesh.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = _boneTransforms[mesh.ParentBone.Index] * world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }

            // Draw Clothing (skinned to the same skeleton)
            foreach (var clothing in _clothingMeshes.Values)
            {
                foreach (ModelMesh mesh in clothing.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = _boneTransforms[mesh.ParentBone.Index] * world;
                        effect.View = view;
                        effect.Projection = projection;
                        effect.EnableDefaultLighting();
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
