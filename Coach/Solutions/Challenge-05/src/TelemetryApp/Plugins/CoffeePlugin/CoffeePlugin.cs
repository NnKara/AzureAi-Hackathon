using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace TelemetryApp.Plugins.CoffeePlugin;

public class CoffeePlugin
{
    [KernelFunction("Coffee")]
    [Description("List of availlable coffees from the catalog")]
    [return: Description("An array of coffees")]
    public static List<CoffeeModel> GetCatalog() => CoffeeModel.Catalog;

	[KernelFunction("OrderCoffee")]
	[Description("Order coffee")]
	[return: Description("One coffee")]
	public CoffeeModel? OrderCoffee(CoffeeType type, SugarContent sugar) {
		CoffeeModel? result = CoffeeModel.Catalog
			.FirstOrDefault(c => c.Type == type);

		if (result != null) result.SugarContent = sugar;
		return result;
	}
}