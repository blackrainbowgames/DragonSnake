using System.Globalization;
using System.Text.RegularExpressions;

namespace Assets.Scripts
{
    public static class Extensions
    {
        public static void SetText(this UILabel label, string text)
        {
            var scale = label.transform.localScale;

            label.text = text;
            label.transform.localScale = scale;
        }

        public static void SetText(this UILabel label, float text)
        {
            label.SetText(text.ToString(CultureInfo.InvariantCulture));
        }

        public static void SetLocalizedText(this UILabel label, string text)
        {
            var scale = label.transform.localScale;

            foreach (Match match in new Regex("%.+%").Matches(text))
            {
                text = text.Replace(match.Value, Localization.Localize(match.Value));
            }

            label.text = text;
            label.transform.localScale = scale;
        }

        public static void SetLocalizedText(this UILabel label, float text)
        {
            label.SetLocalizedText(text.ToString(CultureInfo.InvariantCulture));
        }
    }
}