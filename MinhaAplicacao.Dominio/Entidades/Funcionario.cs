using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhaAplicacao.Dominio.Entidades
{
    public class Funcionario
    {
        public string Nome { get; set; }
        public long Codigo { get; set; }
        public double TotalReceber { get; set; }
        public double TotalExtra { get; set; }
        public double TotalDesconto { get; set; }
        public double HorasExtras { get; set; }
        public double HorasDebito { get; set; }
        public long DiasFalta { get; set; }
        public long DiasExtras { get; set; }
        public long DiasTrabalhados { get; set; }
    }
}
