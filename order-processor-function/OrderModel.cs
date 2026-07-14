public class OrderModel
{
    public string CustomerName { get; set; }
    public string Email { get; set; }
    public string OrderDate { get; set; }
    public string OrderAmount { get; set; }
    public OrderItem[] Items { get; set; }
}