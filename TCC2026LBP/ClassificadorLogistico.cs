using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TCC2026LBP
{
    // Regressão logística com regularização L2 e padronização z-score.
    // Portado sem alteração de lógica — não depende de nada específico do Windows Forms.
    public class ClassificadorLogistico
    {
        private double[] pesos;
        private double bias;
        private double[] media;
        private double[] desvio;
        private double taxaAprendizado;
        private int epocas;
        private double lambda;

        public int NumCaracteristicas => pesos.Length;

        public ClassificadorLogistico(int numCaracteristicas, double taxaAprendizado = 0.01, int epocas = 3000, double lambda = 0.1)
        {
            pesos = new double[numCaracteristicas];
            Random rnd = new Random(42);
            for (int i = 0; i < pesos.Length; i++) pesos[i] = (rnd.NextDouble() - 0.5) * 0.01;
            bias = 0;
            this.taxaAprendizado = taxaAprendizado;
            this.epocas = epocas;
            this.lambda = lambda;
        }

        private double Sigmoid(double z) => 1.0 / (1.0 + Math.Exp(-z));

        public double[] Padronizar(double[] x)
        {
            double[] padrao = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                double den = desvio[i] > 0 ? desvio[i] : 1e-9;
                padrao[i] = (x[i] - media[i]) / den;
            }
            return padrao;
        }

        public double PreverProbabilidade(double[] x)
        {
            double[] xp = Padronizar(x);
            double z = bias;
            for (int j = 0; j < pesos.Length; j++) z += pesos[j] * xp[j];
            return Sigmoid(z);
        }

        public void DefinirPesos(double[] novosPesos, double novoBias, double[] novaMedia, double[] novoDesvio)
        {
            pesos = novosPesos;
            bias = novoBias;
            media = novaMedia;
            desvio = novoDesvio;
        }

        // ==========================================================
        // TREINO E PERSISTÊNCIA — comentados a pedido, sem UI de treino
        // no projeto WPF atual. Lógica idêntica ao código novo, pronta
        // para reativar bastando descomentar e ligar a algum botão/menu.
        // ==========================================================
        /*
        public void Treinar(List<double[]> X, List<int> y)
        {
            int n = X.Count;
            int d = pesos.Length;

            media = new double[d];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < d; j++)
                    media[j] += X[i][j];
            for (int j = 0; j < d; j++) media[j] /= n;

            desvio = new double[d];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < d; j++)
                {
                    double diff = X[i][j] - media[j];
                    desvio[j] += diff * diff;
                }
            for (int j = 0; j < d; j++) desvio[j] = Math.Sqrt(desvio[j] / n);

            List<double[]> Xp = new List<double[]>();
            for (int i = 0; i < n; i++) Xp.Add(Padronizar(X[i]));

            for (int epoca = 0; epoca < epocas; epoca++)
            {
                double[] gradPesos = new double[d];
                double gradBias = 0;

                for (int i = 0; i < n; i++)
                {
                    double z = bias;
                    for (int j = 0; j < d; j++) z += pesos[j] * Xp[i][j];
                    double pred = Sigmoid(z);
                    double erro = pred - y[i];

                    for (int j = 0; j < d; j++) gradPesos[j] += erro * Xp[i][j];
                    gradBias += erro;
                }

                for (int j = 0; j < d; j++)
                    pesos[j] -= taxaAprendizado * ((gradPesos[j] / n) + lambda * pesos[j]);
                bias -= taxaAprendizado * (gradBias / n);
            }
        }

        public void Salvar(string caminho)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(bias.ToString("F10", CultureInfo.InvariantCulture));
            sb.AppendLine(string.Join(",", pesos.Select(p => p.ToString("F10", CultureInfo.InvariantCulture))));
            sb.AppendLine(string.Join(",", media.Select(p => p.ToString("F10", CultureInfo.InvariantCulture))));
            sb.AppendLine(string.Join(",", desvio.Select(p => p.ToString("F10", CultureInfo.InvariantCulture))));
            File.WriteAllText(caminho, sb.ToString());
        }
        */
    }
}