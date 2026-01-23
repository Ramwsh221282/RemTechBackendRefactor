namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

public sealed class CategoryResponse
{
	public CategoryResponse(Guid id, string name)
	{
		Id = id;
		Name = name;
	}

	public Guid Id { get; set; }
	public string Name { get; set; }
	public int? VehiclesCount { get; set; }
	public float? TextSearchScore { get; set; }
}
