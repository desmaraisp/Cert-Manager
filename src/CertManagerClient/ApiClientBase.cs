using System.Text.Json;

namespace CertManagerClient.Base;

public class ApiClientBase
{
	/// <summary>
	/// If you need to transform jsonSerializerOptions for aot compilation, 
	/// this is where you can insert your JsonSerializerContext. 
	/// We have to do it this way since nswag code generation doesn't support 
	/// injecting a singleton jsonSerializerOptions
	/// </summary>
	public static Action<JsonSerializerOptions>? JsonSerializerTransform { get; set; }

	public static void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
	{
		JsonSerializerTransform?.Invoke(settings);
	}
}