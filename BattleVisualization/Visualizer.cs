using System;
using System.Diagnostics;
using System.IO;
using HPG.Battle;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SharedLogic;
using Tanat.SharedLogic.Defs;

namespace BattleVisualization
{
    internal class Visualizer : GameWindow, IEventHandler
    {
        private static BattleField _field;
        private static Context SharedLogicContext { get; set; }

        [STAThread]
        private static void Main()
        {
            using (var visualizer = new Visualizer())
            {
                _field = new BattleField();
                visualizer.Run(30);
            }
        }

        public static int _screensize = 700;

        public Visualizer()
            : base(_screensize, _screensize, GraphicsMode.Default, "battle visualization")
        {
            VSync = VSyncMode.On;
        }

        private float _zoom = 0.5f;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            var Projection = Matrix4.CreateOrthographic(-_screensize * _zoom, -_screensize * _zoom, -1, 1);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref Projection);
            //GL.Translate(700 / 2, -700 / 2, 0);

            var Modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref Modelview);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Mouse.ButtonUp += MouseOnButtonUp;

            SharedLogicContext = new Context();
            string defs = ReadFile("InitData//Defs.json");
            string state = ReadFile("InitData//Init.json");
            Debug.WriteLine("defs: " + defs);
            Debug.WriteLine("state: " + state);
            SharedLogicContext.InitDefs(defs);
            SharedLogicContext.InitState(state);
            State.Init(SharedLogicContext.Defs, SharedLogicContext.State, this);
        }

        private static string ReadFile(string fileName)
        {
            string line = string.Empty;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(fileName);
                line = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                if(sr != null)
                    sr.Close();
            }
            return line;
        }

        private void MouseOnButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.Button == MouseButton.Left)
            {
                Debug.WriteLine(mouseButtonEventArgs.X + ":" + mouseButtonEventArgs.Y);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _field.Draw();

            SwapBuffers();
        }
    }
}