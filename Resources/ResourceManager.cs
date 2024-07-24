using System.Xml.Linq;

namespace OtlobLap.Resources
{
    public static class ResourceManager
    {


        public static void UpdateTranslations(string fileName, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // Construct the full file path
            string resourceFilePath = Path.Combine("Resources", fileName);

            // Load the .resx file as XML
            XDocument doc;
            try
            {
                doc = XDocument.Load(resourceFilePath);
            }
            catch (FileNotFoundException)
            {
                doc = new XDocument(new XElement("root"));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading resource file '{fileName}': {ex.Message}", ex);
            }

            // Find the existing element by key
            XElement existingElement = doc.Descendants("data")
                .FirstOrDefault(x => string.Equals(x.Attribute("name")?.Value, key, StringComparison.OrdinalIgnoreCase));

            // Update or add the translation
            if (existingElement != null)
            {
                existingElement.Element("value").Value = value;
            }
            else
            {
                XElement newDataElement = new XElement("data",
                    new XAttribute("name", key),
                    new XElement("value", value));
                doc.Element("root").Add(newDataElement);
            }

            // Save the changes back to the .resx file
            try
            {
                doc.Save(resourceFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving resource file '{fileName}': {ex.Message}", ex);
            }
        }

    }
}
