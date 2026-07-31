public sealed class AvatarRenderer : IDisposable {
    private readonly Model _body;
    private readonly Dictionary<AvatarSlot, Model> _worn = new();
    private SkinningData _skin;

    public AvatarRenderer(AvatarDescription desc, ContentManager content) {
        _body = content.Load<Model>($"Avatar/Body/{desc.BodyType}");
        _skin = _body.Tag as SkinningData
            ?? throw new InvalidOperationException("Missing SkinningData");
        foreach (var (slot, guid) in desc.EnumerateWornItems())
            _worn[slot] = content.Load<Model>($"Avatar/Items/{guid}");
    }

    public void Draw(Matrix world, Matrix view, Matrix projection,
                     AvatarAnimation anim) {
        var bones = anim.GetBoneTransforms(_skin);      // Matrix[]
        foreach (var mesh in _body.Meshes)
            DrawSkinned(mesh, bones, world, view, projection);
        foreach (var item in _worn.Values)
            foreach (var mesh in item.Meshes)
                DrawSkinned(mesh, bones, world, view, projection);
    }

    private void DrawSkinned(ModelMesh m, Matrix[] bones,
                             Matrix w, Matrix v, Matrix p) {
        foreach (var part in m.MeshParts) {
            part.Effect.Parameters["Bones"].SetValue(bones);
            part.Effect.Parameters["WorldViewProj"].SetValue(w * v * p);
            // eye/mouth texture swap driven by AvatarExpression:
            // part.Effect.Parameters["FaceTex"].SetValue(_faceTextures[expr]);
        }
        m.Draw();
    }
}
