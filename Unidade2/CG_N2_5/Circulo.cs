using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public double raio;

        public Circulo(Objeto paiRef, ref char rotulo, double _raio) : base(paiRef, ref rotulo)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            raio = _raio;
            for (int i = 0; i < 360; i += 1) {
                Ponto4D ponto = Matematica.GerarPtosCirculo(i, raio);
                base.PontosAdicionar(ponto);
            }
            base.ObjetoAtualizar();        
        }

        public Circulo(Objeto paiRef,  ref char rotulo, double _raio, Ponto4D ptoDeslocamento) : base(paiRef, ref rotulo)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            raio = _raio;
            for (int i = 0; i < 360; i += 1) {
                Ponto4D ponto = Matematica.GerarPtosCirculo(i, raio);
                ponto.X += ptoDeslocamento.X;
                ponto.Y += ptoDeslocamento.Y;
                base.PontosAdicionar(ponto);
            }
            base.ObjetoAtualizar(); 
        }

        public void Atualizar(Ponto4D ptoDeslocamento) {
            for (int i = 0; i < 360; i += 1) {
                Ponto4D ponto = Matematica.GerarPtosCirculo(i, raio);
                ponto.X += ptoDeslocamento.X;
                ponto.Y += ptoDeslocamento.Y;
                base.PontosAlterar(ponto, i);
            }
            base.ObjetoAtualizar(); 
        }

        #if CG_Debug
        public override string ToString()
        {
        string retorno;
        retorno = "__ Objeto Círculo _ Tipo: " + PrimitivaTipo + "\n";
        retorno += base.ToString();
        return (retorno);

        }
        #endif
    }
}