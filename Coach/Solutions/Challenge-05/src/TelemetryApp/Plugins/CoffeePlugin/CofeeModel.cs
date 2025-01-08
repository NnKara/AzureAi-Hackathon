using System.Text.Json.Serialization;

namespace TelemetryApp.Plugins.CoffeePlugin;

public class CoffeeModel {
    [JsonPropertyName("sugar")] public SugarContent SugarContent { get; set; }

    [JsonPropertyName("type")] public CoffeeType Type { get; set; }

    [JsonPropertyName("price")] public Decimal Price { get; set; }

    public static List<CoffeeModel> Catalog = [
        new() {
            Type = CoffeeType.Espresso,
            Price = 2.50m
        },

        new() {
            Type = CoffeeType.Latte,
            Price = 3.50m
        },

        new() {
            Type = CoffeeType.FreddoCappuccino,
            Price = 3.75m
        },

        new() {
            Type = CoffeeType.Mocha,
            Price = 4.00m
        },

        new() {
            Type = CoffeeType.Machiato,
            Price = 3.25m
        }
    ];
}