using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TodoApi.Converters;

public class FlexibleNullableDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly string[] _formats = new[]
    {
        "yyyy-MM-dd",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ssZ",
        "dd/MM/yyyy",
        "MM/dd/yyyy",
    };

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        if (DateTime.TryParseExact(dateString, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        if (DateTime.TryParse(dateString, out var parsedDate))
        {
            return parsedDate;
        }

        throw new JsonException($"Unable to parse '{dateString}' as a date. Expected formats: yyyy-MM-dd, yyyy-MM-ddTHH:mm:ss, etc.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}