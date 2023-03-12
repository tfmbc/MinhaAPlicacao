using Microsoft.AspNetCore.Http;
using MinhaAplicacao.Dominio.Entidades;
using System.Globalization;
using System.Text;

namespace MinhaAplicacao.Negocio
{
    public class ImportacaoNegocio
    {
        /// <summary>
        /// Retorna o Json final de acordo com os arquivos importados
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<List<Pagamento>> ProcessarImportacao(IFormFile[] files)
        {
            List<Importacao> listaFInal = new List<Importacao>();
            //processando a listagem da importação
            Parallel.For(0, files.Count(), async i =>
            {
                List<Importacao> listImportacao = new List<Importacao>();
                listImportacao = await ProcessarListaImportacao(files[i]);
                listaFInal.AddRange(listImportacao);
            });

            //Processando os calculos
            return await new Util.Util().RealizarCalculo(listaFInal);
        }

        /// <summary>
        /// Método para Processar o arquivo e retornar o entidade Importacao
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<List<Importacao>> ProcessarListaImportacao(IFormFile file)
        {
            try
            {
                List<Importacao> listImportacao = new List<Importacao>();
                var registros = new List<string>();
                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream, Encoding.Latin1, true))
                {
                    //salva a primeira linha com a header do arquivo
                    var header = await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line) && !line.Equals(header))
                        {
                            registros.Add(line);
                        }
                    }
                    if (registros.Count() > 0)
                    {
                        foreach (var registro in registros)
                        {
                            var recordSplit = registro.Split(';');
                            Importacao imp = new Importacao();
                            imp.NomeArquivo = Path.GetFileNameWithoutExtension(file.FileName);
                            imp.Codigo = Convert.ToInt32(recordSplit[0]);
                            imp.Nome = recordSplit[1];
                            imp.ValorHora = Convert.ToDecimal(recordSplit[2].Replace("R$ ", "").Replace(", ", ","));
                            imp.Data = Convert.ToDateTime(recordSplit[3]);
                            imp.HoraEntrada = DateTime.ParseExact(recordSplit[4], "HH:mm:ss", CultureInfo.InvariantCulture); ;
                            imp.HoraSaida = DateTime.ParseExact(recordSplit[5], "HH:mm:ss", CultureInfo.InvariantCulture); ;
                            imp.HoraAlmocoInicio = DateTime.ParseExact(recordSplit[6].Split('-')[0].Trim(), "HH:mm", CultureInfo.InvariantCulture); ;
                            imp.HoraAlmocoFim = DateTime.ParseExact(recordSplit[6].Split('-')[1].Trim(), "HH:mm", CultureInfo.InvariantCulture); ;
                            listImportacao.Add(imp);
                        }
                    }
                }
                return listImportacao;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao processar o arquivo: " + file.Name + " - Detalhes: " + ex.Message);
            }

        }
    }
}

