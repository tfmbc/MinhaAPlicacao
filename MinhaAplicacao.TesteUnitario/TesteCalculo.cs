using FluentAssertions;
using MinhaAplicacao.Dominio.Entidades;
using MinhaAplicacao.Util;
using System;
using Xunit;

namespace MinhaAplicacao.TesteUnitario
{
    public class TesteCalculo
    {
        /// <summary>
        /// Realiza o teste do método de cálculo
        /// </summary>
        [Fact(DisplayName = "Teste dos cálculos")]
        [Trait("Util", "RealizarCalculo")]
        public async Task RealizarCalculoTesteAsync()
        {
            //Arrange (organizar)
            var testeListaImportacao = new List<Importacao>();
            testeListaImportacao.Add(new Importacao
            {
                NomeArquivo = "Teste Departamento de Administração-Abril-2022",
                Codigo = 1,
                Nome = "Thiago Teste",
                Data = new DateTime(2023, 03, 12),
                HoraEntrada = new DateTime(2023, 03, 12, 8, 0, 0),
                HoraAlmocoInicio = new DateTime(2023, 03, 12, 12, 0, 0),
                HoraAlmocoFim = new DateTime(2023, 03, 12, 13, 0, 0),
                HoraSaida = new DateTime(2023, 03, 12, 18, 0, 0),
                ValorHora = decimal.Parse("110,97")
            });
            var pgtoEsperado = new List<Pagamento>();
            var pgtoEsperadoFUncionario = new List<Funcionario>();
            pgtoEsperadoFUncionario.Add(new Funcionario {
               Codigo = 1,
               DiasExtras = 0,
               DiasFalta = 29,
               DiasTrabalhados = 1,
               HorasDebito = 0,
               HorasExtras = 1, 
               Nome = "Thiago Teste",
               TotalDesconto = double.Parse("3218,13"),
               TotalExtra = double.Parse("110,97"),
               TotalReceber = double.Parse("998,73")
            });
            pgtoEsperado.Add(new Pagamento
            {
                Departamento = "Teste Departamento de Administração",
                MesVigencia = "Abril",
                AnoVigencia = 2022,
                TotalPagar = double.Parse("998,73"),
                TotalDescontos = double.Parse("3218,13"),
                TotalExtras = double.Parse("110,97"),
                Funcionarios = pgtoEsperadoFUncionario
            });

            //Act (Agir)
            var pagamento = await new Util.Util().RealizarCalculo(testeListaImportacao);

            //Assert (Valida se o resultado esperado é igual a lista criada)
            pgtoEsperado.FirstOrDefault().Should().BeEquivalentTo(pagamento.FirstOrDefault());
        }
    }
}