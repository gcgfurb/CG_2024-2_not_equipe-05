﻿#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
// #define CG_Privado      

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

//FIXME: padrão Singleton

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private static Objeto mundo = null;
        private char rotuloNovo = '?';
        private char rotuloNovoTeste = '!';

        private Objeto objetoSelecionado = null;

        private readonly float[] _sruEixos =
        {
          //-0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
          // 0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
          // 0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */

            // Posição (X, Y, Z)      // Coordenadas UV (U, V)
            -0.5f,  0.0f,  0.0f,    0.0f, 0.0f,   // Eixo X -
             0.5f,  0.0f,  0.0f,    1.0f, 0.0f,   // Eixo X +
             0.0f, -0.5f,  0.0f,    0.0f, 1.0f,   // Eixo Y -
             0.0f,  0.5f,  0.0f,    1.0f, 1.0f,   // Eixo Y +
             0.0f,  0.0f, -0.5f,    0.5f, 0.0f,   // Eixo Z -
             0.0f,  0.0f,  0.5f,    0.5f, 1.0f    // Eixo Z +
        };

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private Shader _shaderCuboMaior;
        private Shader _shaderCuboMenor;

        private Shader _shaderBranca;
        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Shader _shaderCiano;
        private Shader _shaderMagenta;
        private Shader _shaderAmarela;
        private Shader _basicLightingShader;

        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        private Cubo _cuboMaior;
        private Cubo _cuboMenor;

        private IluminacaoAtual _iluminacaoAtual;

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloNovo); //padrão Singleton
        }


        protected override void OnLoad()
        {
            base.OnLoad();

            Utilitario.Diretivas();
#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            GL.Enable(EnableCap.DepthTest);       // Ativar teste de profundidade
            // GL.Enable(EnableCap.CullFace);     // Desenha os dois lados da face
                                               // GL.FrontFace(FrontFaceDirection.Cw);
                                               // GL.CullFace(CullFaceMode.FrontAndBack);

            #region Shaders
            _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");

            _shaderCuboMenor = _shaderAmarela;//new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderCuboMaior = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");


            _basicLightingShader = new Shader("Shaders/basicLighting.vert", "Shaders/basicLighting.frag");
            #endregion

            #region Eixos: SRU  
            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            #endregion

            #region Objeto: ponto  
            objetoSelecionado = new Ponto(mundo, ref rotuloNovo, new Ponto4D(1.0f, 1.0f, -1.0f));
            objetoSelecionado.PrimitivaTipo = PrimitiveType.Points;
            objetoSelecionado.PrimitivaTamanho = 5;
            objetoSelecionado.shaderCor = _shaderAmarela;

            objetoSelecionado = new Ponto(mundo, ref rotuloNovo, new Ponto4D(-1.0f, 1.0f, -1.0f));
            objetoSelecionado.PrimitivaTipo = PrimitiveType.Points;
            objetoSelecionado.PrimitivaTamanho = 5;
            objetoSelecionado.shaderCor = _shaderVerde;

            objetoSelecionado = new Ponto(mundo, ref rotuloNovo, new Ponto4D(1.0f, 1.0f, 1.0f));
            objetoSelecionado.PrimitivaTipo = PrimitiveType.Points;
            objetoSelecionado.PrimitivaTamanho = 5;
            objetoSelecionado.shaderCor = _shaderVermelha;
            #endregion

            #region Objeto: Cubo principal
            _cuboMaior = new Cubo(mundo, ref rotuloNovo);
            _cuboMaior.shaderCor = _shaderBranca;
            _cuboMaior.Textura = Texture.LoadFromFile("Imagem/grupo.png");

            #endregion

            #region Objeto: Cubo menor
            _cuboMenor = new Cubo(_cuboMaior, ref rotuloNovoTeste);
            _cuboMenor.shaderCor = _shaderCuboMenor;

            _cuboMenor.MatrizTranslacaoXYZ(4, 0, 0);

            objetoSelecionado = _cuboMaior;

            #endregion
            _camera = new Camera(Vector3.UnitZ * 5, ClientSize.X / (float)ClientSize.Y);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _cuboMaior.Textura.Use(TextureUnit.Texture0);

            switch(_iluminacaoAtual)
            {
                case IluminacaoAtual.BasicLighting:

                    _basicLightingShader.Use();
                    _basicLightingShader.SetVector3("objectColor", new Vector3(1.0f, 1.0f, 1.0f)); // Cor do objeto
                    _basicLightingShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));  // Luz branca

                    Ponto4D pontoLuz = _cuboMenor.BBox.ObterCentro;

                    _basicLightingShader.SetVector3("lightPos", new Vector3((float)pontoLuz.X, (float)pontoLuz.Y, (float)pontoLuz.Z));    // Posição da luz

                    // Atualize as matrizes view e projection
                    _basicLightingShader.SetMatrix4("view", _camera.GetViewMatrix());
                    _basicLightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
                    _basicLightingShader.SetMatrix4("model", Matrix4.Identity);

                    // Atualize a posição da câmera
                    _basicLightingShader.SetVector3("viewPos", _camera.Position);

                    _cuboMaior.shaderCor = _basicLightingShader;
                    _cuboMaior.Textura.Use(TextureUnit.Texture0);

                    break;

                default:
                    _cuboMaior.shaderCor = _shaderCuboMaior;
                    break;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mundo.Desenhar(new Transformacao4D(), _camera);

#if CG_Gizmo
            Gizmo_Sru3D();
#endif
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _cuboMenor.MatrizRotacao(.005);
            

            // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
            #region Teclado
            var estadoTeclado = KeyboardState;

            if (estadoTeclado.IsKeyDown(Keys.Escape))
                Close();
            if (estadoTeclado.IsKeyPressed(Keys.Space))
            {
                if (objetoSelecionado == null)
                    objetoSelecionado = mundo;
                objetoSelecionado.shaderCor = _shaderBranca;
                objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
                objetoSelecionado.shaderCor = _shaderAmarela;
            }
            if (estadoTeclado.IsKeyPressed(Keys.G))
                mundo.GrafocenaImprimir("");
            if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
                Console.WriteLine(objetoSelecionado.ToString());
            if (estadoTeclado.IsKeyPressed(Keys.M) && objetoSelecionado != null)
                objetoSelecionado.MatrizImprimir();
            if (estadoTeclado.IsKeyPressed(Keys.I) && objetoSelecionado != null)
                objetoSelecionado.MatrizAtribuirIdentidade();
            if (estadoTeclado.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(-0.5, 0, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0.5, 0, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, 0.5, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, -0.5, 0);
            if (estadoTeclado.IsKeyPressed(Keys.O) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, 0, 0.5);
            if (estadoTeclado.IsKeyPressed(Keys.L) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, 0, -0.5);
            if (estadoTeclado.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
            if (estadoTeclado.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
            if (estadoTeclado.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
            if (estadoTeclado.IsKeyPressed(Keys.End) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
            //// if (estadoTeclado.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
            ////     objetoSelecionado.MatrizRotacao(10);
            //if (estadoTeclado.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
            //    objetoSelecionado.MatrizRotacao(-10);
            //if (estadoTeclado.IsKeyPressed(Keys.D3) && objetoSelecionado != null)
            //    objetoSelecionado.MatrizRotacaoZBBox(10);
            //if (estadoTeclado.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
            //    objetoSelecionado.MatrizRotacaoZBBox(-10);

            if (estadoTeclado.IsKeyDown(Keys.D1)) {
                _iluminacaoAtual = IluminacaoAtual.BasicLighting;
            }
            if (estadoTeclado.IsKeyDown(Keys.D0))
            {
                _iluminacaoAtual = IluminacaoAtual.Nenhuma;
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (estadoTeclado.IsKeyDown(Keys.Z))
                _camera.Position = Vector3.UnitZ * 5;
            if (estadoTeclado.IsKeyDown(Keys.W))
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            if (estadoTeclado.IsKeyDown(Keys.S))
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            if (estadoTeclado.IsKeyDown(Keys.A))
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            if (estadoTeclado.IsKeyDown(Keys.D))
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            if (estadoTeclado.IsKeyDown(Keys.RightShift))
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            if (estadoTeclado.IsKeyDown(Keys.LeftShift))
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
                                                                              // if (estadoTeclado.IsKeyDown(Keys.D9))
                                                                              //   _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
                                                                              // if (estadoTeclado.IsKeyDown(Keys.D0))
                                                                              //   _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down

            #endregion

            #region  Mouse

            if (MouseState.IsButtonPressed(MouseButton.Left))
            {
                Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
                Console.WriteLine("__ Valores do Espaço de Tela");
                Console.WriteLine("Vector2 mousePosition: " + MousePosition);
                Console.WriteLine("Vector2i windowSize: " + ClientSize);
            }

            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;

                //var deltaX = mouse.X - _lastPos.X;
                //_cuboMaior.TrocaEixoRotacao('y');
                //_cuboMaior.MatrizRotacaoZBBox(deltaX);

                //var deltaY = mouse.Y - _lastPos.Y;
                //_cuboMaior.TrocaEixoRotacao('x');
                //_cuboMaior.MatrizRotacaoZBBox(deltaY);

                //_lastPos = new Vector2(mouse.X, mouse.Y);
            }

            #endregion

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteProgram(_shaderBranca.Handle);
            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);
            GL.DeleteProgram(_shaderCiano.Handle);
            GL.DeleteProgram(_shaderMagenta.Handle);
            GL.DeleteProgram(_shaderAmarela.Handle);
            GL.DeleteProgram(_basicLightingShader.Handle);


            base.OnUnload();
        }

#if CG_Gizmo
        private void Gizmo_Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
            var model = Matrix4.Identity;
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.SetMatrix4("model", model);
            _shaderVermelha.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVermelha.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.SetMatrix4("model", model);
            _shaderVerde.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVerde.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.SetMatrix4("model", model);
            _shaderAzul.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderAzul.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }
#endif

    }
    public enum IluminacaoAtual
    {
        Nenhuma,
        BasicLighting
    }
}
