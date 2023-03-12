using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhaAplicacao.Dominio.Entidades
{
    public class Pagamento
    {
        public string Departamento { get; set; }
        public string MesVigencia { get; set; }
        public long AnoVigencia { get; set; }
        public double TotalPagar { get; set; }
        public double TotalDescontos { get; set; }
        public double TotalExtras { get; set; }
        public List<Funcionario> Funcionarios { get; set; }
    }
}
