using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace BattleVisualization
{
    class Unit
    {
        protected Color4 _unitColor = Color4.White;
        protected Texture _tex;
        protected float _radius = 7;
        protected int _x, _y;

        public Unit(int x, int y)
        {
            _tex = TextureStorage.GetTexture("hero");
            _x = 0;
            _y = 0;
        }

        public void Draw()
        {
            RenderSolid(_x, _y);
        }

        private void RenderSolid(float x, float y)
        {
            _tex.Bind();
            GL.Color4(Color4.White);
            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 0);
            GL.Vertex2(x - _radius/2f, y - _radius/2f);

            GL.TexCoord2(1, 0);
            GL.Vertex2(x + _radius/2, y - _radius/2f);

            GL.TexCoord2(1, 1);
            GL.Vertex2(x + _radius/2, y + _radius/2);

            GL.TexCoord2(0, 1);
            GL.Vertex2(x - _radius/2f, y + _radius/2);

            GL.End();
        }
    }
}
