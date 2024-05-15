using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Diplom;

public partial class Форма_Заявки : Window
{
    private List<Заказ> _заказs;
    private List<Ассортимент> _ассортиментs;
    private List<Услуги> _услугиs;
    private string _connString="server=localhost; database=pp; port=3306;User Id=root;password=root";
    private MySqlConnection conn;
    public Форма_Заявки()
    {
        InitializeComponent();
        ShowProductTable();
    }
    public void ShowProductTable()
    {
        string sql =
            "SELECT заказ.ID, ассортимент.Название,клиент.Имя, услуги.Название, заказ.Цена From заказ " +
            "Join ассортимент ON заказ.Ассортимент_id = ассортимент.ID " +
            "Join клиент ON заказ.Клиент_id = клиент.ID " +
            "Join услуги on заказ.Услуга_id = услуги.ID;";
            
        _заказs = new List<Заказ>();
        conn = new MySqlConnection(_connString);
        conn.Open();
        MySqlCommand command = new MySqlCommand(sql, conn);
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read() && reader.HasRows)
        {
            var currentYch = new Заказ()
            {
                ID = reader.GetInt32("ID"),
                Ассортимент_id = reader.GetValue(1).ToString(),
                Клиент_id= reader.GetValue(2).ToString(),
                Услуга_id = reader.GetValue(3).ToString(),
                Цена = reader.GetInt32("Цена"),
            };
            _заказs.Add(currentYch);
        }
        conn.Close();
        DataGridD.ItemsSource = _заказs;
    }
    private void SearchService(object? sender, TextChangedEventArgs e)
    {
        var srv = _заказs;
        srv = srv.Where(x => x.Клиент_id.Contains(ServSearch.Text)).ToList();
        DataGridD.ItemsSource = srv;
    }
    private void SortAscending(object? sender, RoutedEventArgs e)
    {
        var sortedItems = DataGridD.ItemsSource.Cast<Заказ>().OrderBy(s => s.Цена).ToList();
        DataGridD.ItemsSource = sortedItems;
    }
    private void SortDescending(object? sender, RoutedEventArgs e)
    {
        var sortedItems = DataGridD.ItemsSource.Cast<Заказ>().OrderByDescending(s => s.Цена).ToList();
        DataGridD.ItemsSource = sortedItems;
    }
    private void DeleteData(object? sender, RoutedEventArgs e)
    {
        try
        {
            Заказ asr = DataGridD.SelectedItem as Заказ;
            if (asr == null)
            {
                return;
            }
            conn = new MySqlConnection(_connString);
            conn.Open();
            string sql = "DELETE FROM заказ WHERE ID = " + asr.ID;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            _заказs.Remove(asr);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        ShowProductTable();
    }
    private void Main(object? sender, RoutedEventArgs e)
    {
        Главное_меню logout = new Главное_меню();
        logout.Show();
        this.Close();
    }
}