using System.Text.Json.Serialization;

namespace Wilkywayre.Govee.Driver.Model.Color;

public class GoveeColor
{
    public GoveeColor(){}
    public GoveeColor(string hex)
    {
        SetRgbOffOfHex(hex);
    }
    public GoveeColor(int red, int green, int blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }
    
    [JsonPropertyName("r")]
    public int Red { get; set; }
    [JsonPropertyName("g")]
    public int Green { get; set; }
    [JsonPropertyName("b")]
    public int Blue { get; set; }

    private void SetRgbOffOfHex(string hex)
    {
        Red = int.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
        Green = int.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
        Blue = int.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
    }
}