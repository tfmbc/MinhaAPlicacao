using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhaAplicacao.Dominio.Entidades
{
    public class Importacao
    {
        public string NomeArquivo { get; set; }
        public long Codigo { get; set; }
        public string Nome { get; set; }
        public  decimal ValorHora { get; set; }
        public DateTime Data { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime HoraSaida { get; set; }
        public DateTime HoraAlmocoInicio { get; set; }
        public DateTime HoraAlmocoFim { get; set; }
    }
}
