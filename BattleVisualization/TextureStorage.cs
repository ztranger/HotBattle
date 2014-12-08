using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BattleVisualization
{
    public static class TextureStorage
    {
        static Dictionary<string, Texture> _storage = new Dictionary<string, Texture>();

        static TextureStorage()
        {
            _storage.Add("hero", new Texture(new Bitmap("Images/hero.png")));
        }

        public static Texture GetTexture(string name)
        {
            if (_storage.ContainsKey(name))
                return _storage[name];
            return null;
        }
    }
}
