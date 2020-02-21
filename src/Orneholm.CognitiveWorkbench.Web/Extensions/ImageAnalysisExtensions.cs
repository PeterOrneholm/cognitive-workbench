using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Orneholm.CognitiveWorkbench.Web.Extensions
{
    public static class ImageAnalysisExtensions
    {
        public static string GetClipArtTypeDescription(this ImageType imageType)
        {
            return imageType.ClipArtType switch
            {
                0 => "Non ClipArt",
                1 => "Ambiguous",
                2 => "Normal ClipArt",
                3 => "Good ClipArt",
                _ => "Unknown"
            };
        }

        public static string GetLineDrawingTypeDescription(this ImageType imageType)
        {
            return imageType.ClipArtType switch
            {
                0 => "Non LineDrawing",
                1 => "LineDrawing",
                _ => "Unknown"
            };
        }

        public static List<ObjectHierarchy> ToFlatList(this ObjectHierarchy objectHierarchy)
        {
            var list = new List<ObjectHierarchy>();
            var current = objectHierarchy;

            while (current != null)
            {
                list.Add(current);
                current = current.Parent;
            }

            return list;
        }

        public static string ToDescription(this FaceRectangle faceRectangle)
        {
            return $"X: {faceRectangle.Left}; Y: {faceRectangle.Top}; W: {faceRectangle.Width}; H: {faceRectangle.Height}";
        }

        public static string ToDescription(this BoundingRect boundingRect)
        {
            return $"X: {boundingRect.X}; Y: {boundingRect.Y}; W: {boundingRect.W}; H: {boundingRect.H}";
        }

        public static string ToCss(this FaceRectangle boundingRect, int imageWidth, int imageHeight)
        {
            return $"left: {boundingRect.Left.ToCssPercentageString(imageWidth)}; " +
                   $"top: {boundingRect.Top.ToCssPercentageString(imageHeight)}; " +
                   $"width: {boundingRect.Width.ToCssPercentageString(imageWidth)}; " +
                   $"height: {boundingRect.Height.ToCssPercentageString(imageHeight)};";
        }

        public static string ToCss(this BoundingRect boundingRect, int imageWidth, int imageHeight)
        {
            return $"left: {boundingRect.X.ToCssPercentageString(imageWidth)}; " +
                   $"top: {boundingRect.Y.ToCssPercentageString(imageHeight)}; " +
                   $"width: {boundingRect.W.ToCssPercentageString(imageWidth)}; " +
                   $"height: {boundingRect.H.ToCssPercentageString(imageHeight)};";
        }

        public static string ToCssPercentageString(this int value, int fullValue)
        {
            var percentage = (double)value / fullValue;
            var truncated = TruncateDecimal(percentage, 5);

            return truncated.ToCssPercentageString();
        }

        public static string ToCssPercentageString(this double value)
        {
            return $"{(value * 100).ToString(CultureInfo.GetCultureInfo("en-US"))}%";
        }

        public static string ToDescriptivePercentage(this double score)
        {
            return $"{score:P} ({TruncateDecimal(score, 5)})";
        }

        private static double TruncateDecimal(double value, int precision)
        {
            var step = Math.Pow(10, precision);
            return Math.Truncate(step * value) / step;
        }

        public static string ToSentence(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return text.First().ToString().ToUpper() + text.Substring(1) + ".";
        }
    }
}
