/*
 As constantes dos pré-processors estão nos arquivos ".csproj"
 desse projeto e da CG_Biblioteca.
*/
#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Dictionary<char, Objeto> grafoLista = [];
    private Objeto objetoSelecionado = null;
    // Declara variável
    private bool isPrimeiro = true;
    private int contador = 0;

    private readonly float[] _sruEixos =
        {
           -0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
            0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
            0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
        };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;

    private Shader _shaderMagenta;

    private bool mouseMovtoPrimeiro = true;
    private Ponto4D mouseMovtoUltimo;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      // Cor
            GL.ClearColor(0.50196f, 0.50196f, 0.50196f, 1.0f);

      #region Cores
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");

      #endregion

#if CG_Gizmo
      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion
#endif

      #region Objeto: retângulo  
      objetoSelecionado = new Retangulo(mundo, ref rotuloAtual, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5));
      objetoSelecionado.ShaderObjeto =  new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");

      #endregion
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);
            mundo.Desenhar(new Transformacao4D(), objetoSelecionado);


#if CG_Gizmo
      Gizmo_Sru3D();
#endif
      SwapBuffers();
    }

    private void AlternaTipoPrimitivo()
    {
      switch (contador)
      {
        case 0:
          if (isPrimeiro)
          {
            isPrimeiro = false;
            objetoSelecionado.PrimitivaTipo = PrimitiveType.Lines;
            contador += 2;
            break;
          }
          else
          {
            objetoSelecionado.PrimitivaTipo = PrimitiveType.Points;
            contador++;
            break;
          }
        case 1:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.Lines;
          contador++;
          break;
        case 2:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
          contador++;
          break;
        case 3:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
          contador++;
          break;
        case 4:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.Triangles;
          contador++;
          break;
        case 5:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.TriangleStrip;
          contador++;
          break;
        case 6:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.TriangleFan;
          contador = 0;
          break;
        default:
          break;
      }
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      #region Teclado
      var estadoTeclado = KeyboardState;
      #endregion

      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        AlternaTipoPrimitivo();
      }
    
      // if (objetoSelecionado != null)
      //     Console.WriteLine(objetoSelecionado);
      // else
      //     Console.WriteLine("objetoSelecionado: MUNDO \n__________________________________\n");
    
   
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

#if CG_Gizmo
      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);
#endif

      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);

      base.OnUnload();
    }

    private void Gizmo_Sru3D()
    {
#if CG_Gizmo
#if CG_OpenGL
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);

#endif
#endif
    }
  }
}