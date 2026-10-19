using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TCC2026LBP
{
    public static class AnaliseLBP
    {
        private static readonly double[] HistogramaGabaritoReal = new double[768] {
            0.012220, 0.001103, 0.006088, 0.003138, 0.001117, 0.000106, 0.003220, 0.003756,
            0.006188, 0.000639, 0.000612, 0.000756, 0.003198, 0.000171, 0.011438, 0.016608,
            0.001013, 0.000174, 0.000593, 0.000448, 0.000094, 0.000014, 0.000163, 0.000220,
            0.003028, 0.000461, 0.000689, 0.000913, 0.003460, 0.000203, 0.015338, 0.012723,
            0.005808, 0.000612, 0.003451, 0.000700, 0.000607, 0.000089, 0.000694, 0.000426,
            0.000618, 0.000074, 0.000074, 0.000051, 0.000721, 0.000077, 0.000876, 0.000407,
            0.002838, 0.000442, 0.000653, 0.000400, 0.000152, 0.000028, 0.000266, 0.000309,
            0.010370, 0.001613, 0.000802, 0.000955, 0.015097, 0.000811, 0.032482, 0.010795,
            0.001063, 0.000096, 0.000612, 0.000162, 0.000193, 0.000014, 0.000462, 0.000212,
            0.000614, 0.000084, 0.000065, 0.000075, 0.000458, 0.000029, 0.001634, 0.000868,
            0.000093, 0.000013, 0.000079, 0.000027, 0.000012, 0.000004, 0.000029, 0.000033,
            0.000148, 0.000026, 0.000066, 0.000041, 0.000193, 0.000026, 0.000779, 0.000490,
            0.002917, 0.000153, 0.000662, 0.000264, 0.000447, 0.000031, 0.000401, 0.000308,
            0.000723, 0.000073, 0.000043, 0.000078, 0.000913, 0.000048, 0.000954, 0.000730,
            0.003317, 0.000195, 0.000390, 0.000293, 0.000197, 0.000032, 0.000289, 0.000515,
            0.014677, 0.000826, 0.000345, 0.000693, 0.011077, 0.000481, 0.010052, 0.009057,
            0.006185, 0.003161, 0.000628, 0.011381, 0.000645, 0.000167, 0.000775, 0.016280,
            0.003321, 0.000705, 0.000077, 0.000879, 0.000712, 0.000278, 0.000879, 0.034141,
            0.000602, 0.000447, 0.000066, 0.001658, 0.000081, 0.000029, 0.000075, 0.000892,
            0.000672, 0.000400, 0.000045, 0.000978, 0.000414, 0.000295, 0.000388, 0.011103,
            0.000591, 0.000701, 0.000074, 0.000892, 0.000068, 0.000072, 0.000047, 0.000407,
            0.000080, 0.000049, 0.000007, 0.000041, 0.000046, 0.000080, 0.000040, 0.000576,
            0.000705, 0.000886, 0.000044, 0.000968, 0.000071, 0.000043, 0.000076, 0.000742,
            0.000841, 0.000957, 0.000038, 0.000928, 0.000379, 0.000682, 0.000479, 0.010922,
            0.003154, 0.003556, 0.000719, 0.015957, 0.000475, 0.000208, 0.000935, 0.012644,
            0.000670, 0.000421, 0.000044, 0.000382, 0.000406, 0.000299, 0.000970, 0.010959,
            0.000153, 0.000193, 0.000072, 0.000814, 0.000026, 0.000028, 0.000047, 0.000509,
            0.000258, 0.000289, 0.000069, 0.000675, 0.000276, 0.000463, 0.000659, 0.009084,
            0.010385, 0.014899, 0.000811, 0.032020, 0.001603, 0.000792, 0.000946, 0.010710,
            0.000837, 0.000378, 0.000038, 0.000480, 0.000949, 0.000686, 0.000894, 0.010959,
            0.014496, 0.011190, 0.000356, 0.010102, 0.000802, 0.000464, 0.000690, 0.009049,
            0.029294, 0.009780, 0.000494, 0.009746, 0.009751, 0.008492, 0.009762, 0.328772,
            0.001797, 0.000384, 0.000845, 0.000438, 0.000381, 0.000156, 0.000420, 0.001698,
            0.000790, 0.000198, 0.000226, 0.000159, 0.000362, 0.000123, 0.002571, 0.003045,
            0.000390, 0.000124, 0.000196, 0.000093, 0.000136, 0.000060, 0.000122, 0.000114,
            0.000362, 0.000098, 0.000156, 0.000127, 0.000502, 0.000111, 0.002606, 0.002128,
            0.000867, 0.000202, 0.001047, 0.000238, 0.000197, 0.000084, 0.000242, 0.000192,
            0.000228, 0.000074, 0.000106, 0.000062, 0.000158, 0.000063, 0.000361, 0.000227,
            0.000444, 0.000100, 0.000244, 0.000097, 0.000122, 0.000056, 0.000171, 0.000127,
            0.002626, 0.000259, 0.000365, 0.000196, 0.002869, 0.000208, 0.021488, 0.003780,
            0.000402, 0.000146, 0.000201, 0.000122, 0.000122, 0.000065, 0.000104, 0.000117,
            0.000201, 0.000085, 0.000071, 0.000065, 0.000094, 0.000058, 0.000257, 0.000219,
            0.000161, 0.000063, 0.000087, 0.000057, 0.000066, 0.000050, 0.000056, 0.000092,
            0.000127, 0.000059, 0.000066, 0.000079, 0.000117, 0.000084, 0.000212, 0.000347,
            0.000450, 0.000126, 0.000238, 0.000173, 0.000097, 0.000058, 0.000100, 0.000128,
            0.000162, 0.000066, 0.000065, 0.000082, 0.000127, 0.000082, 0.000194, 0.000277,
            0.001703, 0.000122, 0.000205, 0.000135, 0.000120, 0.000096, 0.000129, 0.000260,
            0.002915, 0.000226, 0.000236, 0.000279, 0.002210, 0.000346, 0.003873, 0.002216,
            0.000784, 0.000388, 0.000225, 0.002367, 0.000195, 0.000126, 0.000162, 0.002997,
            0.000882, 0.000228, 0.000132, 0.000334, 0.000230, 0.000195, 0.000329, 0.019093,
            0.000202, 0.000101, 0.000072, 0.000254, 0.000081, 0.000057, 0.000064, 0.000224,
            0.000234, 0.000096, 0.000063, 0.000197, 0.000177, 0.000129, 0.000199, 0.004122,
            0.000230, 0.000172, 0.000101, 0.000364, 0.000071, 0.000064, 0.000059, 0.000228,
            0.000132, 0.000070, 0.000052, 0.000100, 0.000064, 0.000082, 0.000099, 0.001036,
            0.000166, 0.000136, 0.000062, 0.000199, 0.000061, 0.000078, 0.000078, 0.000273,
            0.000331, 0.000195, 0.000103, 0.000390, 0.000199, 0.000272, 0.000545, 0.005827,
            0.000401, 0.000543, 0.000171, 0.003061, 0.000098, 0.000115, 0.000138, 0.002451,
            0.000227, 0.000174, 0.000069, 0.000214, 0.000101, 0.000132, 0.000199, 0.004031,
            0.000127, 0.000116, 0.000067, 0.000230, 0.000059, 0.000086, 0.000079, 0.000348,
            0.000198, 0.000133, 0.000082, 0.000294, 0.000134, 0.000232, 0.000271, 0.003409,
            0.002435, 0.003403, 0.000367, 0.021996, 0.000262, 0.000236, 0.000196, 0.004227,
            0.000342, 0.000221, 0.000098, 0.000580, 0.000202, 0.000292, 0.000399, 0.006808,
            0.003135, 0.002509, 0.000236, 0.004334, 0.000225, 0.000351, 0.000276, 0.002284,
            0.019551, 0.004132, 0.001065, 0.006932, 0.004233, 0.003420, 0.005929, 0.754593,
            0.001686, 0.000325, 0.000778, 0.000381, 0.000325, 0.000118, 0.000386, 0.001620,
            0.000656, 0.000142, 0.000180, 0.000121, 0.000313, 0.000094, 0.002603, 0.002635,
            0.000317, 0.000086, 0.000150, 0.000067, 0.000101, 0.000042, 0.000092, 0.000092,
            0.000298, 0.000068, 0.000127, 0.000100, 0.000469, 0.000093, 0.003042, 0.002074,
            0.000772, 0.000156, 0.000969, 0.000204, 0.000143, 0.000056, 0.000200, 0.000167,
            0.000179, 0.000050, 0.000072, 0.000046, 0.000124, 0.000049, 0.000320, 0.000208,
            0.000384, 0.000074, 0.000193, 0.000077, 0.000088, 0.000039, 0.000142, 0.000110,
            0.002595, 0.000224, 0.000312, 0.000166, 0.002703, 0.000189, 0.021511, 0.003817,
            0.000322, 0.000104, 0.000151, 0.000096, 0.000092, 0.000041, 0.000075, 0.000096,
            0.000145, 0.000054, 0.000046, 0.000047, 0.000067, 0.000039, 0.000222, 0.000189,
            0.000117, 0.000042, 0.000056, 0.000043, 0.000041, 0.000037, 0.000043, 0.000077,
            0.000097, 0.000045, 0.000045, 0.000066, 0.000093, 0.000070, 0.000188, 0.000329,
            0.000383, 0.000095, 0.000200, 0.000139, 0.000068, 0.000044, 0.000073, 0.000109,
            0.000117, 0.000045, 0.000045, 0.000066, 0.000100, 0.000065, 0.000162, 0.000262,
            0.001720, 0.000094, 0.000171, 0.000108, 0.000095, 0.000075, 0.000113, 0.000257,
            0.002837, 0.000187, 0.000206, 0.000265, 0.002131, 0.000321, 0.003729, 0.002217,
            0.000634, 0.000338, 0.000168, 0.002276, 0.000144, 0.000096, 0.000117, 0.002634,
            0.000690, 0.000173, 0.000109, 0.000277, 0.000178, 0.000169, 0.000278, 0.017924,
            0.000145, 0.000071, 0.000046, 0.000220, 0.000052, 0.000041, 0.000045, 0.000191,
            0.000177, 0.000070, 0.000045, 0.000170, 0.000146, 0.000113, 0.000176, 0.004110,
            0.000172, 0.000136, 0.000076, 0.000329, 0.000048, 0.000047, 0.000042, 0.000195,
            0.000107, 0.000050, 0.000035, 0.000079, 0.000043, 0.000068, 0.000083, 0.000989,
            0.000124, 0.000106, 0.000044, 0.000164, 0.000044, 0.000069, 0.000065, 0.000259,
            0.000271, 0.000165, 0.000083, 0.000365, 0.000170, 0.000259, 0.000491, 0.005911,
            0.000331, 0.000497, 0.000138, 0.003383, 0.000075, 0.000092, 0.000109, 0.002293,
            0.000168, 0.000138, 0.000051, 0.000188, 0.000071, 0.000109, 0.000166, 0.003802,
            0.000094, 0.000092, 0.000046, 0.000205, 0.000040, 0.000067, 0.000065, 0.000327,
            0.000168, 0.000110, 0.000064, 0.000282, 0.000112, 0.000221, 0.000258, 0.003420,
            0.002202, 0.003159, 0.000322, 0.021592, 0.000219, 0.000204, 0.000162, 0.004091,
            0.000276, 0.000183, 0.000079, 0.000526, 0.000169, 0.000276, 0.000364, 0.006588,
            0.002886, 0.002265, 0.000199, 0.003916, 0.000189, 0.000330, 0.000254, 0.002253,
            0.018391, 0.003737, 0.001038, 0.006451, 0.004132, 0.003517, 0.005879, 0.768259,
        };

        public class ResultadoAnalise
        {
            public WriteableBitmap bitmapOriginal { get; set; }
            public string PercentualAutenticidade { get; set; }
            public double[] HistogramaNormalizado { get; set; }
        }

        public static ResultadoAnalise Analisar(string caminhoArquivo)
        {
            try
            {
                // ESTUDAR //
                BitmapImage bitmapOriginal = new BitmapImage();
                bitmapOriginal.BeginInit();
                bitmapOriginal.CacheOption = BitmapCacheOption.OnLoad;
                bitmapOriginal.UriSource = new Uri(caminhoArquivo);
                bitmapOriginal.EndInit();
                bitmapOriginal.Freeze();

                FormatConvertedBitmap bitmapBgr = new FormatConvertedBitmap();
                bitmapBgr.BeginInit();
                bitmapBgr.Source = bitmapOriginal;
                bitmapBgr.DestinationFormat = PixelFormats.Bgr32;
                bitmapBgr.EndInit();

                int largura = bitmapBgr.PixelWidth;
                int altura = bitmapBgr.PixelHeight;
                int strideOrigem = largura * 4;

                byte[] pixelsOrigem = new byte[strideOrigem * altura];
                bitmapBgr.CopyPixels(pixelsOrigem, strideOrigem, 0);


                WriteableBitmap imagemParaExibir = new WriteableBitmap(largura, altura, 96, 96, PixelFormats.Bgr32, null);
                imagemParaExibir.WritePixels(new Int32Rect(0, 0, largura, altura), pixelsOrigem, strideOrigem, 0);
                imagemParaExibir.Freeze();

                double[] histogramaNormalizado = new double[768];
                for (int canal = 0; canal < 3; canal++)
                {
                    byte[] pixelsCanal = ExtrairCanalYCbCr(pixelsOrigem, largura, altura, strideOrigem, canal);
                    double[] histogramaCanal = CalcularHistogramaLBP(pixelsCanal, largura, altura);
                    Array.Copy(histogramaCanal, 0, histogramaNormalizado, canal * 256, 256);
                }


                return new ResultadoAnalise
                {
                    bitmapOriginal = imagemParaExibir,
                    PercentualAutenticidade = CalcularPercentualDoHistograma(histogramaNormalizado),
                    HistogramaNormalizado = histogramaNormalizado
                };
                // ESTUDAR //
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao analisar a imagem");
                return null;
            }
            
        }

        public static string CalcularPercentualDoHistograma(double[] histograma)
        {
            double distancia = DistanciaQui(histograma, HistogramaGabaritoReal);
            double ldDistancia = 1.5;
            double porcentagem = (1 - (distancia / ldDistancia)) * 100;
            if (porcentagem < 0) porcentagem = 0;
            if (porcentagem > 100) porcentagem = 100;
            return porcentagem.ToString("F2") + "%";
        }

        private static byte[] ExtrairCanalYCbCr(byte[] pixelsOrigem, int largura, int altura, int strideOrigem, int canal)
        {
            try
            {
                byte[] resultado = new byte[largura * altura];

                for (int y = 0; y < altura; y++)
                {
                    int linhaOrigem = y * strideOrigem;
                    int linhaResultado = y * largura;
                    for (int x = 0; x < largura; x++)
                    {
                        int idx = linhaOrigem + x * 4;
                        byte b = pixelsOrigem[idx];
                        byte g = pixelsOrigem[idx + 1];
                        byte r = pixelsOrigem[idx + 2];

                        double valor;
                        if (canal == 0)
                            valor = 0.299 * r + 0.587 * g + 0.114 * b;
                        else if (canal == 1)
                            valor = 128 - 0.168736 * r - 0.331264 * g + 0.5 * b;
                        else
                            valor = 128 + 0.5 * r - 0.418688 * g - 0.081312 * b;

                        valor = Math.Max(0, Math.Min(255, valor));
                        resultado[linhaResultado + x] = (byte)Math.Round(valor);
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao extrair canal YCbCr");
                return null;
            }
        }

        private static double[] CalcularHistogramaLBP(byte[] pixelsCanal, int largura, int altura)
        {
            try
            {
                int[] histograma = new int[256];

                for (int y = 1; y < altura - 1; y++)
                {
                    for (int x = 1; x < largura - 1; x++)
                    {
                        int centro = pixelsCanal[y * largura + x];
                        int padraoLBP = 0;

                        if (pixelsCanal[(y - 1) * largura + (x - 1)] >= centro) padraoLBP += 1;
                        if (pixelsCanal[(y - 1) * largura + x] >= centro) padraoLBP += 2;
                        if (pixelsCanal[(y - 1) * largura + (x + 1)] >= centro) padraoLBP += 4;
                        if (pixelsCanal[y * largura + (x + 1)] >= centro) padraoLBP += 8;
                        if (pixelsCanal[(y + 1) * largura + (x + 1)] >= centro) padraoLBP += 16;
                        if (pixelsCanal[(y + 1) * largura + x] >= centro) padraoLBP += 32;
                        if (pixelsCanal[(y + 1) * largura + (x - 1)] >= centro) padraoLBP += 64;
                        if (pixelsCanal[y * largura + (x - 1)] >= centro) padraoLBP += 128;

                        histograma[padraoLBP]++;
                    }
                }

                int totalPixels = (largura - 2) * (altura - 2);
                double[] histogramaNormalizado = new double[256];
                for (int i = 0; i < 256; i++)
                    histogramaNormalizado[i] = totalPixels > 0 ? (double)histograma[i] / totalPixels : 0;

                return histogramaNormalizado;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular histograma LBP");
                return null;
            }
        }

        private static double DistanciaQui(double[] histogramaAtual, double[] histogramaRef)
        {
            try
            {
                double distancia = 0;
                for (int i = 0; i < histogramaRef.Length; i++)
                {
                    double numerador = Math.Pow(histogramaRef[i] - histogramaAtual[i], 2);
                    double denominador = histogramaRef[i] + histogramaAtual[i];
                    if (denominador > 0)
                        distancia += numerador / denominador;
                }
                return distancia;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular distância qui-quadrado");
                return double.MaxValue;
            }
        }
    }
}