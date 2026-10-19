using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Data.Sqlite;

namespace TCC2026LBP
{
	public class RegistroAnalise
	{
		public int Id { get; set; }
		public string NomeArquivo { get; set; }
		public DateTime DataAnalise { get; set; }
	}

	public class HistogramaJson
	{
		public double[] Y { get; set; }
		public double[] Cb { get; set; }
		public double[] Cr { get; set; }
	}

	public static class RepositorioAnalises
	{
		// O arquivo do banco fica em %LocalAppData%\TCC2026LBP\tcc_lbp.db
		private static readonly string CaminhoBanco = System.IO.Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"TCC2026LBP", "tcc_lbp.db");

		private static readonly string StringConexao = "Data Source=" + CaminhoBanco;

		// Abre a conexão e garante que a pasta, o arquivo e a tabela existam
		private static SqliteConnection AbrirConexao()
		{
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(CaminhoBanco));

			var conexao = new SqliteConnection(StringConexao);
			conexao.Open();

			using (var comando = conexao.CreateCommand())
			{
				comando.CommandText = @"
                    CREATE TABLE IF NOT EXISTS analises (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        nome_arquivo TEXT NOT NULL,
                        imagem BLOB NOT NULL,
                        percentual_autenticidade TEXT NOT NULL,
                        histograma TEXT NOT NULL,
                        data_analise TEXT NOT NULL DEFAULT (datetime('now','localtime'))
                    );";
				comando.ExecuteNonQuery();
			}

			return conexao;
		}

		public static void Salvar(string nomeArquivo, WriteableBitmap imagem, string percentualAutenticidade, double[] histograma)
		{
			try
			{
                byte[] imagemBytes = CodificarImagemComprimida(imagem);

                var histogramaObjeto = new HistogramaJson
                {
                    Y = histograma.Take(256).ToArray(),
                    Cb = histograma.Skip(256).Take(256).ToArray(),
                    Cr = histograma.Skip(512).Take(256).ToArray()
                };
                string histogramaTexto = JsonSerializer.Serialize(histogramaObjeto);

                using (var conexao = AbrirConexao())
                using (var comando = conexao.CreateCommand())
                {
                    comando.CommandText = @"INSERT INTO analises (nome_arquivo, imagem, percentual_autenticidade, histograma)
                                        VALUES ($nome, $imagem, $percentual, $histograma)";
                    comando.Parameters.AddWithValue("$nome", nomeArquivo);
                    comando.Parameters.AddWithValue("$imagem", imagemBytes);
                    comando.Parameters.AddWithValue("$percentual", percentualAutenticidade);
                    comando.Parameters.AddWithValue("$histograma", histogramaTexto);
                    comando.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar análise");
            }
        }

        public static void Excluir(int id)
        {
            using (var conexao = AbrirConexao())
            using (var comando = conexao.CreateCommand())
            {
                comando.CommandText = "DELETE FROM analises WHERE id = $id";
                comando.Parameters.AddWithValue("$id", id);
                comando.ExecuteNonQuery();
            }
        }

        public static List<RegistroAnalise> Listar()
		{
			var resultado = new List<RegistroAnalise>();

			using (var conexao = AbrirConexao())
			using (var comando = conexao.CreateCommand())
			{
				comando.CommandText = "SELECT id, nome_arquivo, data_analise FROM analises ORDER BY id DESC";

				using (var leitor = comando.ExecuteReader())
				{
					while (leitor.Read())
					{
						resultado.Add(new RegistroAnalise
						{
							Id = leitor.GetInt32(0),
							NomeArquivo = leitor.GetString(1),
							DataAnalise = DateTime.Parse(leitor.GetString(2), CultureInfo.InvariantCulture)
						});
					}
				}
			}

			return resultado;
		}

		public static AnaliseLBP.ResultadoAnalise Carregar(int id)
		{
			try
			{
                using (var conexao = AbrirConexao())
                using (var comando = conexao.CreateCommand())
                {
                    comando.CommandText = "SELECT imagem, percentual_autenticidade, histograma FROM analises WHERE id = $id";
                    comando.Parameters.AddWithValue("$id", id);

                    using (var leitor = comando.ExecuteReader())
                    {
                        if (!leitor.Read())
                            return null;

                        byte[] imagemBytes = leitor.GetFieldValue<byte[]>(0);
                        string percentual = leitor.GetString(1);
                        string histogramaTexto = leitor.GetString(2);

                        var histogramaObjeto = JsonSerializer.Deserialize<HistogramaJson>(histogramaTexto);
                        double[] histograma = histogramaObjeto.Y
                            .Concat(histogramaObjeto.Cb)
                            .Concat(histogramaObjeto.Cr)
                            .ToArray();

                        return new AnaliseLBP.ResultadoAnalise
                        {
                            bitmapOriginal = DecodificarImagem(imagemBytes),
                            PercentualAutenticidade = percentual,
                            HistogramaNormalizado = histograma
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar análise com ID {id}");
                return null;
            }
            
		}
        // ESTUDAR //
        private static byte[] CodificarImagemComprimida(WriteableBitmap imagem, int larguraMaxima = 1280)
		{
			try
			{
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imagem));

                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao codificar imagem");
                return null;
            }	

        }

		private static WriteableBitmap DecodificarImagem(byte[] bytes)
		{
			try
			{
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return new WriteableBitmap(bitmapImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao decodificar imagem");
                return null;
            }
        }
        // ESTUDAR //
    }
}