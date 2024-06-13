using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Diplom;

public partial class Форма_добавления : Window
{
    private MySqlConnection conn;
    private string _connString="server=localhost; database=pp; port=3306;User Id=root;password=root";
    public Форма_добавления()
    {
        InitializeComponent();
    }
    private void AddProduct(object sender, RoutedEventArgs e)
    {
        string название = Name.Text; // Получите значение из элемента управления для ввода имени агента
        string категория = Category.Text;
        string цена = Price.Text;
        string количество = Quantity.Text;
        string sql = "INSERT INTO товар (Название, Категория_id, Цена, Количество) VALUES (@Название, @Категория_id, @Цена, @Количество)";

        conn = new MySqlConnection(_connString);
        conn.Open();
        MySqlCommand command = new MySqlCommand(sql, conn);
        command.Parameters.AddWithValue("@Название", название);
        command.Parameters.AddWithValue("@Категория_id", категория);
        command.Parameters.AddWithValue("@Цена", цена);
        command.Parameters.AddWithValue("@Количество", количество);
        command.ExecuteNonQuery();
        conn.Close();
        Форма_Ассортимент back = new Форма_Ассортимент();
        back.Show();
        this.Close();
    }
    
    
    private void GoBack(object? sender, RoutedEventArgs e)
    {
        Форма_Ассортимент back = new Форма_Ассортимент();
        back.Show();
        this.Close();
    }
}