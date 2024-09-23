#define CG_Debug

using System;
using System.Runtime;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class SrPalito : Objeto
  {
    public Ponto4D inicio { get; set; }
    public Ponto4D fim { get; set; }

    private double angulo;
    private double raio;
    private double moverX;

    public SrPalito(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Lines;
      PrimitivaTamanho = 1;
      inicio = new Ponto4D(0, 0);
      fim = new Ponto4D(0.35, 0.35);
      moverX = 0;
      angulo = 45;
      raio = Matematica.Distancia(inicio, fim);

      base.PontosAdicionar(inicio);
      base.PontosAdicionar(fim);
      base.ObjetoAtualizar();
    }

    private void Atualizar()
    {   
        fim = Matematica.GerarPtosCirculo(angulo, raio);
        inicio.X = moverX;
        fim.X += moverX;
        base.PontosAlterar(inicio, 0);
        base.PontosAlterar(fim, 1);
        base.ObjetoAtualizar();
    }

    public void AtualizarPe(double peInc) {
        moverX += peInc;
        Atualizar();    
    }

    public void AtualizarRaio(double raioInc) {
        raio += raioInc;
        Atualizar();
    }

    public void AtualizarAngulo(double anguloInc) {
        angulo += anguloInc;
        Atualizar();
    }


#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto SrPalito _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += ImprimeToString();
      return (retorno);
    }
#endif

  }
}
