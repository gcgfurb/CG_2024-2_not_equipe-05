using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System;
using System.Drawing;


namespace gcgcg
{
    // Exe https://github.com/dalton-reis/disciplina_CG_2024_2/blob/main/Unidade2/Atividade2/README.md
    public class Circulo : Objeto
    {
        public Circulo(Objeto _paiRef, ref char _rotulo, Objeto objetoFilho = null) : base(_paiRef, ref _rotulo, objetoFilho)
        {
        }

        public Circulo(Objeto _paiRef, ref char _rotulo, Objeto objetoFilho = null, Ponto4D pontoDeslocamento = null) : base(_paiRef, ref _rotulo, objetoFilho)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 5;
            

            for (int i = 0; i <= 360; i += 360/72)
            {
                var pto = Matematica.GerarPtosCirculo(i, 0.5);

                PontosAdicionar(pto);
            }
            
            ShaderObjeto = new("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            //ObjetoAdicionar(null);
            //Atualizar();


        }

        public override void Atualizar()
        {
            base.Atualizar();
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
