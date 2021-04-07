namespace Umbrella.AspNetCore.Blazor.Components.Grid
{
	public interface IUmbrellaGrid
	{
		UmbrellaGridRenderMode RenderMode { get; }
		string? FirstColumnPropertyName { get; }
		void AddColumnDefinition(UmbrellaColumnDefinition column);
		void SetColumnScanCompleted();
	}
}