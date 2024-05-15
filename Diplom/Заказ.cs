namespace Diplom;

public class Заказ
{
    public int ID { get; set; }
    public string Ассортимент_id { get; set; }
    public string Клиент_id { get; set; }
    
    public string Услуга_id { get; set; }
    public int Цена { get; set; }
}