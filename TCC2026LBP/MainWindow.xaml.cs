using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.Text.Json;
namespace TCC2026LBP
{
    public partial class MainWindow : Window
    {
        private bool _menuAberto = true;
        private string _ultimoNomeArquivo;
        private AnaliseLBP.ResultadoAnalise _ultimaAnalise;
        public MainWindow()
        {
            InitializeComponent();
        }
        // ==========================================================
        // MENU LATERAL
        // ==========================================================
        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            _menuAberto = !_menuAberto;
            ColunaMenu.Width = _menuAberto ? new GridLength(220) : new GridLength(0);
        }
        // ==========================================================
        // AÇÕES DO MENU
        // ==========================================================
        private void ButtonAbrirImagem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Selecione uma imagem"
            };
            if (dialog.ShowDialog() == true)
            {
                if (!File.Exists(dialog.FileName))
                {
                    MessageBox.Show("O arquivo selecionado não foi encontrado. Ele pode ter sido movido, renomeado ou excluído.",
                        "Arquivo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    if (new FileInfo(dialog.FileName).Length == 0)
                    {
                        MessageBox.Show("O arquivo selecionado está vazio e não pode ser analisado.",
                            "Arquivo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var resultado = AnaliseLBP.Analisar(dialog.FileName);
                    MostrarResultado(resultado.bitmapOriginal, resultado.PercentualAutenticidade, resultado.HistogramaNormalizado);
                    _ultimoNomeArquivo = System.IO.Path.GetFileName(dialog.FileName);
                    _ultimaAnalise = resultado;
                }
                catch (IOException)
                {
                    MessageBox.Show("Não foi possível ler o arquivo. Ele pode estar em uso por outro programa ou sem permissão de acesso.",
                        "Erro ao abrir imagem", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("O arquivo selecionado não é uma imagem válida ou está corrompido.\n\nDetalhes técnicos: " + ex.Message,
                        "Erro ao abrir imagem", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ButtonCadastrarBanco_Click(object sender, RoutedEventArgs e)
        {
            if (_ultimaAnalise == null || _ultimaAnalise.bitmapOriginal == null)
            {
                MessageBox.Show("Abra uma imagem primeiro — análises carregadas só pelo histograma JSON não têm imagem para cadastrar.",
                    "Cadastrar no Banco", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                RepositorioAnalises.Salvar(_ultimoNomeArquivo, _ultimaAnalise.bitmapOriginal,
                    _ultimaAnalise.PercentualAutenticidade, _ultimaAnalise.HistogramaNormalizado);
                MessageBox.Show($"'{_ultimoNomeArquivo}' cadastrado no banco com sucesso.",
                    "Cadastrar no Banco", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Não foi possível cadastrar: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ButtonCarregarBanco_Click(object sender, RoutedEventArgs e)
        {
            var janela = new SelecionarAnaliseWindow { Owner = this };
            if (janela.ShowDialog() == true && janela.IdSelecionado.HasValue)
            {
                try
                {
                    var resultado = RepositorioAnalises.Carregar(janela.IdSelecionado.Value);
                    if (resultado != null)
                    {
                        MostrarResultado(resultado.bitmapOriginal, resultado.PercentualAutenticidade, resultado.HistogramaNormalizado);
                        _ultimaAnalise = resultado;
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível carregar essa análise.", "Erro",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Não foi possível carregar: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ButtonSalvarImagem_Click(object sender, RoutedEventArgs e)
        {
            if (_ultimaAnalise == null || _ultimaAnalise.bitmapOriginal == null)
            {
                MessageBox.Show("Nenhuma imagem disponível para salvar (análises carregadas por histograma JSON não têm imagem).",
                    "Salvar", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Imagem PNG (*.png)|*.png",
                FileName = "imagem_analisada.png",
                Title = "Salvar imagem analisada"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(_ultimaAnalise.bitmapOriginal));
                    using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Não foi possível salvar a imagem. Verifique se o local permite gravação e se o arquivo não está aberto em outro programa.\n\nDetalhes técnicos: " + ex.Message,
                        "Erro ao salvar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ButtonSalvarHistograma_Click(object sender, RoutedEventArgs e)
        {
            if (_ultimaAnalise == null)
            {
                MessageBox.Show("Nenhuma análise disponível para salvar o histograma.", "Salvar Histograma",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Arquivo JSON (*.json)|*.json",
                FileName = "histograma.json",
                Title = "Salvar histograma em JSON"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var histograma = _ultimaAnalise.HistogramaNormalizado;
                    if (histograma == null || histograma.Length < 768)
                    {
                        MessageBox.Show("O histograma atual não contém os dados esperados (Y, Cb e Cr) e não pode ser salvo.",
                            "Salvar Histograma", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var histogramaObjeto = new HistogramaJson
                    {
                        Y = histograma.Take(256).ToArray(),
                        Cb = histograma.Skip(256).Take(256).ToArray(),
                        Cr = histograma.Skip(512).Take(256).ToArray()
                    };
                    string json = JsonSerializer.Serialize(histogramaObjeto, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(dialog.FileName, json);
                    MessageBox.Show("Histograma salvo com sucesso.", "Salvar Histograma",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Não foi possível salvar o histograma. Verifique se o local permite gravação e se o arquivo não está aberto em outro programa.\n\nDetalhes técnicos: " + ex.Message,
                        "Erro ao salvar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ButtonCarregarHistograma_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Arquivo JSON (*.json)|*.json",
                Title = "Carregar histograma de um arquivo JSON"
            };
            if (dialog.ShowDialog() == true)
            {
                if (!File.Exists(dialog.FileName))
                {
                    MessageBox.Show("O arquivo selecionado não foi encontrado. Ele pode ter sido movido, renomeado ou excluído.",
                        "Arquivo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    string json = File.ReadAllText(dialog.FileName);
                    var histogramaObjeto = JsonSerializer.Deserialize<HistogramaJson>(json);
                    bool formatoValido = histogramaObjeto?.Y != null && histogramaObjeto.Cb != null && histogramaObjeto.Cr != null
                        && histogramaObjeto.Y.Length == 256 && histogramaObjeto.Cb.Length == 256 && histogramaObjeto.Cr.Length == 256;
                    if (!formatoValido)
                    {
                        MessageBox.Show("O arquivo não tem o formato esperado de histograma (Y, Cb e Cr com 256 valores cada).",
                            "Carregar Histograma", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    double[] histograma = histogramaObjeto.Y
                        .Concat(histogramaObjeto.Cb)
                        .Concat(histogramaObjeto.Cr)
                        .ToArray();
                    string percentual = AnaliseLBP.CalcularPercentualDoHistograma(histograma);
                    MostrarResultado(null, percentual, histograma);
                    _ultimaAnalise = new AnaliseLBP.ResultadoAnalise
                    {
                        bitmapOriginal = null,
                        PercentualAutenticidade = percentual,
                        HistogramaNormalizado = histograma
                    };
                    _ultimoNomeArquivo = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                }
                catch (JsonException)
                {
                    MessageBox.Show("Não foi possível interpretar o arquivo como JSON de histograma.", "Carregar Histograma",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (IOException)
                {
                    MessageBox.Show("Não foi possível ler o arquivo. Ele pode estar em uso por outro programa ou sem permissão de acesso.",
                        "Carregar Histograma", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ocorreu um erro inesperado ao carregar o histograma.\n\nDetalhes técnicos: " + ex.Message,
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ButtonSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        // ==========================================================
        // EXIBIÇÃO DO RESULTADO
        // ==========================================================
        public void MostrarImagem(WriteableBitmap bitmapOriginal, string percentualAutenticidade, double[] histogramaNormalizado)
        {
            MostrarResultado(bitmapOriginal, percentualAutenticidade, histogramaNormalizado);
        }
        private void MostrarResultado(WriteableBitmap bitmapOriginal, string percentualAutenticidade, double[] histogramaNormalizado)
        {
            Grid gridPrincipal = new Grid();
            gridPrincipal.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridPrincipal.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            TextBlock texto = new TextBlock
            {
                Text = $"Porcentagem de ser real: {percentualAutenticidade}",
                FontSize = 50,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5)
            };
            Grid.SetRow(texto, 0);
            var graficoHistograma = CriarGraficoHistograma(histogramaNormalizado);
            FrameworkElement conteudo;
            if (bitmapOriginal != null)
            {
                Grid gridConteudo = new Grid();
                gridConteudo.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                gridConteudo.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                Image img = new Image
                {
                    Source = bitmapOriginal,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(5)
                };
                Grid.SetColumn(img, 0);
                Grid.SetColumn(graficoHistograma, 1);
                gridConteudo.Children.Add(img);
                gridConteudo.Children.Add(graficoHistograma);
                conteudo = gridConteudo;
            }
            else
            {
                conteudo = graficoHistograma;
            }
            Grid.SetRow(conteudo, 1);
            gridPrincipal.Children.Add(texto);
            gridPrincipal.Children.Add(conteudo);
            AreaConteudo.Content = gridPrincipal;
        }


        double MediaDoIntervalo(double[] valores, int inicio, int quantidade)
        {
            double soma = 0;
            for (int i = inicio; i < inicio + quantidade; i++)
                soma += valores[i];
            return soma / quantidade;
        }

        // ESTUDAR //
        private FrameworkElement CriarGraficoHistograma(double[] histogramaNormalizado)
        {
            const int valuesPerChannel = 256;
            const int numGrupos = 4;
            const int valoresPorGrupo = valuesPerChannel / numGrupos; // 64

            const double escalaMaxima = 0.05;

            Grid gridGrafico = new Grid { Margin = new Thickness(3) };
            gridGrafico.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridGrafico.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            gridGrafico.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            TextBlock tituloGrafico = new TextBlock
            {
                Text = "Médias do Padrão LBP por Faixa (64 em 64)",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 4)
            };
            Grid.SetRow(tituloGrafico, 0);

            Grid gridComEixoY = new Grid();
            gridComEixoY.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            gridComEixoY.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(gridComEixoY, 1);

            TextBlock labelEixoY = new TextBlock
            {
                Text = "Média (%)",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                LayoutTransform = new RotateTransform(-90),
                Margin = new Thickness(0, 0, 5, 0)
            };
            Grid.SetColumn(labelEixoY, 0);

            double[] yValues = new double[valuesPerChannel];
            double[] cbValues = new double[valuesPerChannel];
            double[] crValues = new double[valuesPerChannel];

            if (histogramaNormalizado != null && histogramaNormalizado.Length >= valuesPerChannel * 3)
            {
                Array.Copy(histogramaNormalizado, 0, yValues, 0, valuesPerChannel);
                Array.Copy(histogramaNormalizado, valuesPerChannel, cbValues, 0, valuesPerChannel);
                Array.Copy(histogramaNormalizado, valuesPerChannel * 2, crValues, 0, valuesPerChannel);
            }

            double MediaDoIntervalo(double[] valores, int inicio, int quantidade)
            {
                double soma = 0;
                for (int i = inicio; i < inicio + quantidade; i++)
                    soma += valores[i];
                return soma / quantidade;
            }

            double[] mediasY = new double[numGrupos];
            double[] mediasCb = new double[numGrupos];
            double[] mediasCr = new double[numGrupos];

            for (int g = 0; g < numGrupos; g++)
            {
                int inicio = g * valoresPorGrupo;
                mediasY[g] = MediaDoIntervalo(yValues, inicio, valoresPorGrupo);
                mediasCb[g] = MediaDoIntervalo(cbValues, inicio, valoresPorGrupo);
                mediasCr[g] = MediaDoIntervalo(crValues, inicio, valoresPorGrupo);
            }

            double alturaBase = 320;
            double barWidth = 36;
            double gapEntreBarras = 8;
            double gapEntreGrupos = 60;
            double margemLateral = 50;

            double larguraGrupo = 3 * barWidth + 2 * gapEntreBarras;
            double larguraBase = numGrupos * larguraGrupo + (numGrupos - 1) * gapEntreGrupos + 2 * margemLateral;

            Canvas canvas = new Canvas
            {
                Width = larguraBase,
                Height = alturaBase + 80,
                Background = Brushes.White
            };

            var cores = new (string nome, Brush cor)[]
            {
                ("Y", Brushes.Black),
                ("Cb", Brushes.DodgerBlue),
                ("Cr", Brushes.IndianRed)
            };

            for (int g = 0; g < numGrupos; g++)
            {
                double[] valoresGrupo = { mediasY[g], mediasCb[g], mediasCr[g] };
                double leftGrupo = margemLateral + g * (larguraGrupo + gapEntreGrupos);

                for (int c = 0; c < 3; c++)
                {
                    double left = leftGrupo + c * (barWidth + gapEntreBarras);

                    double proporcao = Math.Min(valoresGrupo[c] / escalaMaxima, 1.0);
                    double altura = proporcao * alturaBase;

                    Rectangle barra = new Rectangle { Width = barWidth, Height = altura, Fill = cores[c].cor };
                    Canvas.SetLeft(barra, left);
                    Canvas.SetTop(barra, alturaBase - altura);
                    canvas.Children.Add(barra);

                    TextBlock valorTexto = new TextBlock
                    {
                        Text = (valoresGrupo[c] * 1000).ToString("F2") + "%",
                        FontSize = 12,
                        FontWeight = FontWeights.Bold,
                        Width = barWidth + 20,
                        TextAlignment = TextAlignment.Center
                    };
                    Canvas.SetLeft(valorTexto, left - 10);
                    Canvas.SetTop(valorTexto, alturaBase - altura - 20);
                    canvas.Children.Add(valorTexto);
                }

                int inicioFaixa = g * valoresPorGrupo;
                int fimFaixa = inicioFaixa + valoresPorGrupo - 1;
                TextBlock nomeGrupo = new TextBlock
                {
                    Text = $"{inicioFaixa}-{fimFaixa}",
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    Width = larguraGrupo,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(nomeGrupo, leftGrupo);
                Canvas.SetTop(nomeGrupo, alturaBase + 14);
                canvas.Children.Add(nomeGrupo);
            }

            Line linhaTopo = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = larguraBase,
                Y2 = 0,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 4, 2 }
            };
            canvas.Children.Add(linhaTopo);
            TextBlock labelTopo = new TextBlock { Text = (escalaMaxima * 100).ToString("F1") + "%", FontSize = 13, Foreground = Brushes.Gray };
            Canvas.SetLeft(labelTopo, larguraBase + 6);
            Canvas.SetTop(labelTopo, -8);
            canvas.Children.Add(labelTopo);

            Line eixoX = new Line { X1 = 0, Y1 = alturaBase, X2 = larguraBase, Y2 = alturaBase, Stroke = Brushes.Black, StrokeThickness = 1 };
            Line eixoY = new Line { X1 = 0, Y1 = 0, X2 = 0, Y2 = alturaBase, Stroke = Brushes.Black, StrokeThickness = 1 };
            canvas.Children.Add(eixoX);
            canvas.Children.Add(eixoY);

            StackPanel legenda = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 8, 0, 0)
            };
            foreach (var (nome, cor) in cores)
            {
                Rectangle quadrado = new Rectangle { Width = 14, Height = 14, Fill = cor, Margin = new Thickness(10, 0, 4, 0) };
                TextBlock label = new TextBlock { Text = nome, FontSize = 14, VerticalAlignment = VerticalAlignment.Center };
                legenda.Children.Add(quadrado);
                legenda.Children.Add(label);
            }

            Viewbox viewbox = new Viewbox
            {
                Stretch = Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Child = canvas
            };
            Grid.SetColumn(viewbox, 1);

            gridComEixoY.Children.Add(labelEixoY);
            gridComEixoY.Children.Add(viewbox);

            TextBlock labelEixoX = new TextBlock
            {
                Text = "Faixa de padrões LBP (0-63, 64-127, 128-191, 192-255)",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 4, 0, 0)
            };

            StackPanel rodape = new StackPanel();
            rodape.Children.Add(labelEixoX);
            rodape.Children.Add(legenda);
            Grid.SetRow(rodape, 2);

            gridGrafico.Children.Add(tituloGrafico);
            gridGrafico.Children.Add(gridComEixoY);
            gridGrafico.Children.Add(rodape);

            return gridGrafico;
        }
        // ESTUDAR //
    }
}