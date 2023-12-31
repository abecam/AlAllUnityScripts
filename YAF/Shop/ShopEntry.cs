public class ShopEntry
{
    public double price;
    public double euroPrice;
    public string key;
    public string name;
    public string description;

    public ShopEntry(double price, double euroPrice, string key, string name, string description)
    {
        this.price = price;
        this.euroPrice = euroPrice;
        this.key = key;
        this.name = name;
        this.description = description;
    }
}
