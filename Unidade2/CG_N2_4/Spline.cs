#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System;

namespace gcgcg
{
  internal class Spline : Objeto
  {

    public List<Ponto4D> pontosAncora;
    private int qtdPontos;

    public Spline(Objeto _paiRef, ref char _rotulo, List<Ponto4D> pontosAncora, int qtdPontos) : base(_paiRef, ref _rotulo)
    {
      
      foreach (Ponto4D ponto in pontosAncora ) {
        base.ObjetoAdicionar(new Ponto(this, ref _rotulo, ponto));      
      }

      PrimitivaTipo = PrimitiveType.LineStrip;
      PrimitivaTamanho = 1;

      this.pontosAncora = pontosAncora;
      this.qtdPontos = qtdPontos;
      Atualizar();
    }

    private List<Ponto4D> CalcularPontosSpline(List<Ponto4D> pontos, int qtdPontos) {

    List<Ponto4D> pontosRetorno = new List<Ponto4D>();      

      double coeficiente = 1/qtdPontos;

      for (int t=0; t <= qtdPontos; t++) {

        Ponto4D p0p1 = PontoSpline(pontos[0], pontos[1], t);
        Ponto4D p1p2 = PontoSpline(pontos[1], pontos[2], t);
        Ponto4D p2p3 = PontoSpline(pontos[2], pontos[3], t);

        Ponto4D p0p1p2 = PontoSpline(p0p1, p1p2, t);
        Ponto4D p1p2p3 = PontoSpline(p1p2, p2p3, t);

        pontosRetorno.Add(PontoSpline(p0p1p2, p1p2p3, t));
      }

      return pontosRetorno;

    }

    private Ponto4D PontoSpline(Ponto4D p1, Ponto4D p2, double t) {
      
      double x = p1.X + (p2.X - p1.X) * t / qtdPontos;
      double y = p1.Y + (p2.Y - p1.Y) * t / qtdPontos;

      return new Ponto4D(x, y);
    }

    public override void Atualizar()
    {
      base.pontosLista = CalcularPontosSpline(this.pontosAncora, this.qtdPontos);
      base.Atualizar();
    }

    public override void PontosAlterar(Ponto4D pto, int posicao)
    {
      pontosAncora[0] = pto;
      pontosLista[posicao] = pto;
      Atualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Spline _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif

  }
}