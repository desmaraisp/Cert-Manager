namespace CertManagerAgent.Exporters.FileExporter;

public enum ExportFormat {
	PEM_Encoded_PKCS1_PrivateKey,
	PEM_Encoded_PKCS8_PrivateKey,
	RSA_PublicKey,
	PFX,
	CER
}


public class FileExporterConfig : BaseExporterConfig {
	public required string OutputDirectory { get; init; }
	public required ExportFormat ExportFormat { get; init; } = ExportFormat.PFX;
}