namespace DPL.Helpers
{
	public static class DocumentSettings
	{
		public static string UploadFile(IFormFile file, string folderName)
		{
			if (file is null)
				return string.Empty;

			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", $"{folderName}");

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file?.FileName)}";

			var filePath = Path.Combine(folderPath, fileName);

			using FileStream stream = new(filePath, FileMode.Create);
			file?.CopyTo(stream);

			return fileName;
		}

		public static void DeleteFile(string folderName, string fileName)
		{
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName, fileName);
			if (File.Exists(filePath))
				File.Delete(filePath);
		}
	}
}
