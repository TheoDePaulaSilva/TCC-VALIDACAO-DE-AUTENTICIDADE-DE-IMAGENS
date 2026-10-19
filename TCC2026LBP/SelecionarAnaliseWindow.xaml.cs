using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TCC2026LBP
{
    public partial class SelecionarAnaliseWindow : Window
    {
        public int? IdSelecionado { get; private set; }

        private class ItemLista
        {
            public int Id { get; set; }
            public string Descricao { get; set; }
        }

        public SelecionarAnaliseWindow()
        {
            InitializeComponent();
            CarregarLista();
        }

        private void CarregarLista()
        {
            var registros = RepositorioAnalises.Listar();

            ListaAnalises.ItemsSource = registros.Select(r => new ItemLista
            {
                Id = r.Id,
                Descricao = $"{r.NomeArquivo} — {r.DataAnalise:dd/MM/yyyy HH:mm}"
            }).ToList();
        }

        private void ButtonCarregar_Click(object sender, RoutedEventArgs e)
        {
            if (ListaAnalises.SelectedItem is ItemLista item)
            {
                IdSelecionado = item.Id;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Selecione uma análise na lista.", "Carregar",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ButtonExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (ListaAnalises.SelectedItem is ItemLista item)
            {
                var resposta = MessageBox.Show($"Excluir \"{item.Descricao}\" permanentemente do banco?",
                    "Excluir", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (resposta == MessageBoxResult.Yes)
                {
                    RepositorioAnalises.Excluir(item.Id);
                    CarregarLista();
                }
            }
            else
            {
                MessageBox.Show("Selecione uma análise na lista.", "Excluir",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ButtonCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}