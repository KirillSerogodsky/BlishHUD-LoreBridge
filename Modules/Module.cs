using LoreBridge.Models;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules;

public abstract class Module
{
    public abstract void Load(Settings settings);

    public abstract void Update(GameTime gameTime);

    public abstract void Unload();
}