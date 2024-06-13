using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Avalonia.Data;
using Avalonia.Threading;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using OfficeOpenXml;
using System.Globalization;
using System.Data;
using System.Text;


namespace Diplom;

public partial class Форма_Заявки : Window
{
    private List<Заказ> _заказs;
    private List<Товар> _ассортиментs;
    private List<Услуги> _услугиs;
    private List<Статус> _статус;
    private string _connString="server=localhost; database=pp; port=3306;User Id=root;password=root";
    private MySqlConnection conn;

    public Форма_Заявки()
    {
        InitializeComponent();
        ShowProductTable();
        FillCmb();
    }
    public void ShowProductTable()
    {
        string sql =
            "SELECT заказ.ID, товар.Название,клиент.Имя, сотрудник.Имя, услуги.Название, заказ.Цена, статус.Название From заказ " +
            "Join товар ON заказ.Товар_id = товар.ID " +
            "Join клиент ON заказ.Клиент_id = клиент.ID " +
            "Join сотрудник ON заказ.Сотрудник_id = сотрудник.ID " +
            "Join статус ON заказ.Статус_id = статус.ID " +
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
                Товар_id = reader.GetValue(1).ToString(),
                Клиент_id= reader.GetValue(2).ToString(),
                Сотрудник_id= reader.GetValue(3).ToString(),
                Услуга_id = reader.GetValue(4).ToString(),
                Цена = reader.GetInt32("Цена"),
                Статус_id = reader.GetValue(6).ToString(),
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
    public void FillCmb()
    {
        _статус = new List<Статус>();
        conn = new MySqlConnection(_connString);
        conn.Open();
        MySqlCommand command = new MySqlCommand("select ID, Название from статус", conn);
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read() && reader.HasRows)
        {
            var currentstatus = new Статус()
            {
                ID = reader.GetInt32("ID"),
                Название = reader.GetString("Название"),
            };
            _статус.Add(currentstatus);
        }
        conn.Close();
        var statusComboBox = this.Find<ComboBox>("CategoryComboBox");
        statusComboBox.ItemsSource = _статус;
    }
    private void Cmdstatus_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var statusComboBox = (ComboBox)sender;
        var currentstatus = statusComboBox.SelectedItem as Статус;
        var filteredstatus = _заказs.Where(x => x.Статус_id == currentstatus.Название).ToList();
        DataGridD.ItemsSource = filteredstatus;
        
    }
    private void DeleteData(object? sender, RoutedEventArgs e)
    {
        try
        {
            Заказ zak = DataGridD.SelectedItem as Заказ;
            if (zak == null)
            {
                return;
            }
            conn = new MySqlConnection(_connString);
            conn.Open();
            string sql = "DELETE FROM заказ WHERE ID = " + zak.ID;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            _заказs.Remove(zak);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        ShowProductTable();
    }
    private void Update(object? sender, RoutedEventArgs e)
    {
        ShowProductTable();
    }

    private void SAVE(object? sender, RoutedEventArgs e)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        string outputFile = @"C:\Users\mrble\Desktop\otchet.xlsx";
        string query =
            "SELECT заказ.ID, товар.Название,клиент.Имя, сотрудник.Имя, услуги.Название, заказ.Цена, статус.Название From заказ " +
            "Join товар ON заказ.Товар_id = товар.ID " +
            "Join клиент ON заказ.Клиент_id = клиент.ID " +
            "Join сотрудник ON заказ.Сотрудник_id = сотрудник.ID " +
            "Join статус ON заказ.Статус_id = статус.ID " +
            "Join услуги on заказ.Услуга_id = услуги.ID;";
        MySqlCommand command = new MySqlCommand(query, conn);
        conn.Open();
        MySqlDataReader dataReader = command.ExecuteReader();
        using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(outputFile)))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Заказы");
            int row = 1;

            for (int i = 1; i <= dataReader.FieldCount; i++)
            {
                worksheet.Cells[row, i].Value = dataReader.GetName(i - 1);
            }
            while (dataReader.Read())
            {
                row++;
                for (int i = 1; i <= dataReader.FieldCount; i++)
                {
                    worksheet.Cells[row, i].Value = dataReader[i - 1];
                }
            }
            
            excelPackage.SaveAs(new FileInfo(outputFile));
        }
        dataReader.Close();
        conn.Close();
        Console.WriteLine("Данные успешно экспортированы в Excel файл.");
    }

    private void back(object? sender, RoutedEventArgs e)
    {
        Главное_меню logout = new Главное_меню();
        logout.Show();
        this.Close();
    }
    private void Main(object? sender, RoutedEventArgs e)
    {
        Главное_меню logout = new Главное_меню();
        logout.Show();
        this.Close();
    }
}