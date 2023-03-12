using Microsoft.AspNetCore.Http;
using MinhaAplicacao.Dominio.Entidades;
using System.Globalization;
using System.Text;

namespace MinhaAplicacao.Util
{
    public class Util
    {
        /// <summary>
        /// Realiza os Calculos por Departamento e Funcionarios
        /// </summary>
        /// <param name="listImportacao"></param>
        /// <returns></returns>
        public async Task<List<Pagamento>> RealizarCalculo(List<Importacao> listImportacao)
        {
            List<Pagamento> ListaDepartamentos = new List<Pagamento>();
            foreach (var itemFinal in listImportacao.GroupBy(g => g.NomeArquivo).Select(s => s.Key))
            {
                var novoDepartamento = new Pagamento();
                novoDepartamento.Departamento = itemFinal.Split("-")[0];
                novoDepartamento.MesVigencia = itemFinal.Split("-")[1];
                novoDepartamento.AnoVigencia = Convert.ToInt32(itemFinal.Split("-")[2]);
                novoDepartamento.Funcionarios = new List<Funcionario>();
                //loop dos funcionários
                foreach (string nomeFunc in listImportacao.Where(x => x.NomeArquivo.ToUpper().Trim() == itemFinal.ToUpper().Trim()).Select(x => x.Nome).Distinct())
                {
                    TimeSpan totalHoraEntrada = new TimeSpan(0, 0, 0);
                    TimeSpan totalHorAlmocoInicio = new TimeSpan(0, 0, 0);
                    TimeSpan totalHorAlmocoFim = new TimeSpan(0, 0, 0);
                    TimeSpan totalHorAlmocoSaida = new TimeSpan(0, 0, 0);
                    TimeSpan totalHorasMes = new TimeSpan(240, 0, 0);// Levando em consideração 8h * 30d
                    int totalDiasTrabalhado = 0;
                    int totalDiasMes = 30;
                    long codigoFunc = 0;
                    double recebimentoTotal = 0;
                    double totalHoraExtra = 0;
                    double valorHora = 0;
                    //loop para os totalizadores
                    foreach (var itemDia in listImportacao.Where(x => x.NomeArquivo.ToUpper().Trim() == itemFinal.ToUpper().Trim() && x.Nome.ToUpper().Trim() == nomeFunc.ToUpper().Trim()))
                    {
                        totalHoraEntrada += new TimeSpan(itemDia.HoraEntrada.Hour, itemDia.HoraEntrada.Minute, itemDia.HoraEntrada.Second);
                        totalHorAlmocoInicio += new TimeSpan(itemDia.HoraAlmocoInicio.Hour, itemDia.HoraAlmocoInicio.Minute, itemDia.HoraAlmocoInicio.Second);
                        totalHorAlmocoFim += new TimeSpan(itemDia.HoraAlmocoFim.Hour, itemDia.HoraAlmocoFim.Minute, itemDia.HoraAlmocoFim.Second);
                        totalHorAlmocoSaida += new TimeSpan(itemDia.HoraSaida.Hour, itemDia.HoraSaida.Minute, itemDia.HoraSaida.Second);
                        totalDiasTrabalhado++;
                        codigoFunc = itemDia.Codigo;
                        recebimentoTotal += Decimal.ToDouble(itemDia.ValorHora) * ((new TimeSpan(itemDia.HoraAlmocoInicio.Hour, itemDia.HoraAlmocoInicio.Minute, itemDia.HoraAlmocoInicio.Second) - 
                                                                                    new TimeSpan(itemDia.HoraEntrada.Hour, itemDia.HoraEntrada.Minute, itemDia.HoraEntrada.Second)).TotalHours + 
                                                                                    (new TimeSpan(itemDia.HoraSaida.Hour, itemDia.HoraSaida.Minute, itemDia.HoraSaida.Second) -
                                                                                    new TimeSpan(itemDia.HoraAlmocoFim.Hour, itemDia.HoraAlmocoFim.Minute, itemDia.HoraAlmocoFim.Second)).TotalHours);
                        totalHoraExtra += 8 - ((new TimeSpan(itemDia.HoraAlmocoInicio.Hour, itemDia.HoraAlmocoInicio.Minute, itemDia.HoraAlmocoInicio.Second) -
                                    new TimeSpan(itemDia.HoraEntrada.Hour, itemDia.HoraEntrada.Minute, itemDia.HoraEntrada.Second)).TotalHours +
                                    (new TimeSpan(itemDia.HoraSaida.Hour, itemDia.HoraSaida.Minute, itemDia.HoraSaida.Second) -
                                    new TimeSpan(itemDia.HoraAlmocoFim.Hour, itemDia.HoraAlmocoFim.Minute, itemDia.HoraAlmocoFim.Second)).TotalHours);

                        valorHora = Decimal.ToDouble(itemDia.ValorHora);
                    }
                    ////Calculando Hora Extra
                    TimeSpan horasTrabalhadasManha = totalHorAlmocoInicio - totalHoraEntrada;
                    TimeSpan horasTrabalhadasTarde = totalHorAlmocoSaida - totalHorAlmocoFim;
                    totalHoraExtra = totalHoraExtra < 0 ? totalHoraExtra * -1 : totalHoraExtra;

                    //populando funcionario
                    Funcionario novoFuncionario = new Funcionario();
                    novoFuncionario.Codigo = codigoFunc;
                    novoFuncionario.Nome = nomeFunc;
                    novoFuncionario.DiasFalta = totalDiasMes - totalDiasTrabalhado;
                    novoFuncionario.DiasExtras = totalDiasTrabalhado > totalDiasMes ? totalDiasTrabalhado - totalDiasMes : 0;
                    novoFuncionario.DiasTrabalhados = totalDiasTrabalhado;
                    novoFuncionario.HorasExtras = totalHoraExtra;
                    novoFuncionario.HorasDebito = totalHoraExtra < 0 ? (totalHorasMes - (horasTrabalhadasManha + horasTrabalhadasTarde)).TotalHours : 0;
                    novoFuncionario.TotalReceber = recebimentoTotal;
                    novoFuncionario.TotalExtra = novoFuncionario.HorasExtras * valorHora;
                    novoFuncionario.TotalDesconto = novoFuncionario.DiasFalta * valorHora;
                    //adicionando o funcionario ao departamento
                    novoDepartamento.Funcionarios.Add(novoFuncionario);
                }
                //contabilizando totais do departamento
                novoDepartamento.TotalPagar = novoDepartamento.Funcionarios.Sum(x => x.TotalReceber);
                novoDepartamento.TotalExtras = novoDepartamento.Funcionarios.Sum(x => x.TotalExtra);
                novoDepartamento.TotalDescontos = novoDepartamento.Funcionarios.Sum(x => x.TotalDesconto);
                ListaDepartamentos.Add(novoDepartamento);
            }

            return await Task.FromResult(ListaDepartamentos);
        }
    }
}