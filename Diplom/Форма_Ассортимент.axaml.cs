using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Diplom;

public partial class Форма_Ассортимент : Window
{
    private List<Ассортимент> _ассортиментs;
    private List<Категория> _категорияs;
    private string _connString="server=localhost; database=pp; port=3306;User Id=root;password=root";
    private MySqlConnection conn;

    public Форма_Ассортимент()
    {
        InitializeComponent();
        ShowProductTable();
        FillCmb();
    }
    public void ShowProductTable()
    {
        string sql =
            "SELECT ассортимент.ID, ассортимент.Название, категория.Название, Цена, Количество FROM ассортимент " +
            "join категория ON ассортимент.Категория_id = категория.ID;";
            
        _ассортиментs = new List<Ассортимент>();
        conn = new MySqlConnection(_connString);
        conn.Open();
        MySqlCommand command = new MySqlCommand(sql, conn);
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read() && reader.HasRows)
        {
            var currentYch = new Ассортимент()
            {
                ID = reader.GetInt32("ID"),
                Название = reader.GetValue(1).ToString(),
                Категория_id= reader.GetValue(2).ToString(),
                Цена = reader.GetInt32("Цена"),
                Количество = reader.GetValue(4).ToString(),
            };
            _ассортиментs.Add(currentYch);
        }
        conn.Close();
        DataGrid.ItemsSource = _ассортиментs;
    }
    private void SearchService(object? sender, TextChangedEventArgs e)
    {
        var srv = _ассортиментs;
        srv = srv.Where(x => x.Название.Contains(ServSearch.Text)).ToList();
        DataGrid.ItemsSource = srv;
    }
    public void FillCmb()
    {
        _категорияs = new List<Категория>();
        conn = new MySqlConnection(_connString);
        conn.Open();
        MySqlCommand command = new MySqlCommand("select ID, Название, Описание from категория", conn);
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read() && reader.HasRows)
        {
            var currentstatus = new Категория()
            {
                ID = reader.GetInt32("ID"),
                Название = reader.GetString("Название"),
                Описание =  reader.GetString("Описание"),
            };
            _категорияs.Add(currentstatus);
        }
        conn.Close();
        var statusComboBox = this.Find<ComboBox>("CategoryComboBox");
        statusComboBox.ItemsSource = _категорияs;
    }
    private void Cmdstatus_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var statusComboBox = (ComboBox)sender;
        var currentstatus = statusComboBox.SelectedItem as Категория;
        var filteredstatus = _ассортиментs.Where(x => x.Категория_id == currentstatus.Название).ToList();
        DataGrid.ItemsSource = filteredstatus;
        
    }
    private void SortAscending(object? sender, RoutedEventArgs e)
    {
        var sortedItems = DataGrid.ItemsSource.Cast<Ассортимент>().OrderBy(s => s.Цена).ToList();
        DataGrid.ItemsSource = sortedItems;
    }
    private void SortDescending(object? sender, RoutedEventArgs e)
    {
        var sortedItems = DataGrid.ItemsSource.Cast<Ассортимент>().OrderByDescending(s => s.Цена).ToList();
        DataGrid.ItemsSource = sortedItems;
    }

    private void AddData(object? sender, RoutedEventArgs e)
    {
        Форма_добавления add = new Форма_добавления();
        add.Show();
        this.Close();
    }
    private void Update(object? sender, RoutedEventArgs e)
    {
        ShowProductTable();
    }
    private void Edit(object? sender, RoutedEventArgs e)
    {
        Ассортимент currentServ = DataGrid.SelectedItem as Ассортимент;
        if (currentServ == null)
            return;
        Форма_редактирования edit = new Форма_редактирования(currentServ, _ассортиментs);
        edit.Show();
        this.Close();
    }
    
    private void DeleteData(object? sender, RoutedEventArgs e)
    {
        try
        {
            Ассортимент asr = DataGrid.SelectedItem as Ассортимент;
            if (asr == null)
            {
                return;
            }
            conn = new MySqlConnection(_connString);
            conn.Open();
            string sql = "DELETE FROM ассортимент WHERE ID = " + asr.ID;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            _ассортиментs.Remove(asr);
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