using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Orneholm.CognitiveWorkbench.Web.Models;

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

        public static string ToEmotion(this Emotion emotion)
        {
            var emotions = new Dictionary<string, double>
            {
                { "Angry", emotion.Anger },
                { "Contempt", emotion.Contempt },
                { "Disgusted", emotion.Disgust },
                { "Feared", emotion.Fear },
                { "Happy", emotion.Happiness },
                { "Neutral", emotion.Neutral },
                { "Sad", emotion.Sadness },
                { "Surprised", emotion.Surprise }
            };

            return emotions.OrderByDescending(x => x.Value).ThenBy(x => x.Key).First().Key;
        }

        public static Dictionary<string, double> ToEmotions(this Emotion emotion)
        {
            var emotions = new Dictionary<string, double>
            {
                { "Anger", emotion.Anger },
                { "Contempt", emotion.Contempt },
                { "Disgust", emotion.Disgust },
                { "Fear", emotion.Fear },
                { "Happiness", emotion.Happiness },
                { "Neutral", emotion.Neutral },
                { "Sadness", emotion.Sadness },
                { "Surprise", emotion.Surprise }
            };

            return emotions.OrderByDescending(x => x.Value).ThenBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public static Dictionary<string, Coordinate> ToFaceLandmarks(this FaceLandmarks faceLandmarks)
        {
            var emotions = new Dictionary<string, Coordinate>
            {
                { "EyeLeftBottom", faceLandmarks.EyeLeftBottom },
                { "EyeLeftInner", faceLandmarks.EyeLeftInner },
                { "EyeLeftOuter", faceLandmarks.EyeLeftOuter },
                { "EyeLeftTop", faceLandmarks.EyeLeftTop },

                { "EyeRightBottom", faceLandmarks.EyeRightBottom },
                { "EyeRightInner", faceLandmarks.EyeRightInner },
                { "EyeRightOuter", faceLandmarks.EyeRightOuter },
                { "EyeRightTop", faceLandmarks.EyeRightTop },

                { "EyebrowLeftInner", faceLandmarks.EyebrowLeftInner },
                { "EyebrowLeftOuter", faceLandmarks.EyebrowLeftOuter },

                { "EyebrowRightInner", faceLandmarks.EyebrowRightInner },
                { "EyebrowRightOuter", faceLandmarks.EyebrowRightOuter },

                { "MouthLeft", faceLandmarks.MouthLeft },
                { "MouthRight", faceLandmarks.MouthRight },

                { "PupilLeft", faceLandmarks.PupilLeft },
                { "PupilRight", faceLandmarks.PupilRight },

                { "NoseTip", faceLandmarks.NoseTip },

                { "NoseRootLeft", faceLandmarks.NoseRootLeft },
                { "NoseRootRight", faceLandmarks.NoseRootRight },

                { "NoseLeftAlarOutTip", faceLandmarks.NoseLeftAlarOutTip },
                { "NoseLeftAlarTop", faceLandmarks.NoseLeftAlarTop },

                { "NoseRightAlarOutTip", faceLandmarks.NoseRightAlarOutTip },
                { "NoseRightAlarTop", faceLandmarks.NoseRightAlarTop },

                { "UpperLipTop", faceLandmarks.UpperLipTop },
                { "UpperLipBottom", faceLandmarks.UpperLipBottom },

                { "UnderLipTop", faceLandmarks.UnderLipTop },
                { "UnderLipBottom", faceLandmarks.UnderLipBottom }
            };

            return emotions.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public static string ToRelativeCss(this Coordinate coordinate, Microsoft.Azure.CognitiveServices.Vision.Face.Models.FaceRectangle faceRectangle, int imageWidth, int imageHeight)
        {
            var relativeWidth = faceRectangle.Width;
            var relativeHeight = faceRectangle.Height;
            var relativeX = coordinate.X - faceRectangle.Left;
            var relativeY = coordinate.Y - faceRectangle.Top;

            return $"left: {relativeX.ToCssPercentageString(relativeWidth)}; " +
                   $"top: {relativeY.ToCssPercentageString(relativeHeight)};";
        }

        public static string ToCss(this Coordinate coordinate, int imageWidth, int imageHeight)
        {
            return $"left: {coordinate.X.ToCssPercentageString(imageWidth)}; " +
                   $"top: {coordinate.Y.ToCssPercentageString(imageHeight)};";
        }

        public static string ToDescription(this Microsoft.Azure.CognitiveServices.Vision.Face.Models.FaceRectangle faceRectangle)
        {
            return $"X: {faceRectangle.Left}; Y: {faceRectangle.Top}; W: {faceRectangle.Width}; H: {faceRectangle.Height}";
        }

        public static string ToDescription(this string boundingBox)
        {
            var box = ParseBox(boundingBox);
            return $"X: {box[0]}; Y: {box[1]}; W: {box[2]}; H: {box[3]}";
        }

        public static string ToDescription(this IList<double> boundingBox)
        {
            // BoundingBox: Bounding box of a recognized region, line, or word, depending on the parent object.
            // The eight integers represent the four points (x-coordinate, y-coordinate) of the detected rectangle
            // from the left-top corner and clockwise.
            var bb = new RecognizeTextRotatedBoundingBox(boundingBox);
            return $"MinX: {bb.MinLeft()}, MinY: {bb.MinTop()}, MaxW: {bb.MaxWidth()}, MaxH: {bb.MaxHeight()}";
        }

        public static string ToDescription(this Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.FaceRectangle faceRectangle)
        {
            return $"X: {faceRectangle.Left}; " +
                   $"Y: {faceRectangle.Top}; " +
                   $"W: {faceRectangle.Width}; " +
                   $"H: {faceRectangle.Height}";
        }

        public static string ToDescription(this BoundingRect boundingRect)
        {
            return $"X: {boundingRect.X}; " +
                   $"Y: {boundingRect.Y}; " +
                   $"W: {boundingRect.W}; " +
                   $"H: {boundingRect.H}";
        }

        public static string ToDescription(this BoundingBox boundingBox, int imageWidth, int imageHeight)
        {
            return $"X: {(int)(boundingBox.Left * imageWidth)}; " +
                   $"Y: {(int)(boundingBox.Top * imageHeight)}; " +
                   $"W: {(int)(boundingBox.Width * imageWidth)}; " +
                   $"H: {(int)(boundingBox.Height * imageHeight)}";
        }

        public static string ToCss(this Microsoft.Azure.CognitiveServices.Vision.Face.Models.FaceRectangle faceRectangle, int imageWidth, int imageHeight)
        {
            return $"left: {faceRectangle.Left.ToCssPercentageString(imageWidth)}; " +
                   $"top: {faceRectangle.Top.ToCssPercentageString(imageHeight)}; " +
                   $"width: {faceRectangle.Width.ToCssPercentageString(imageWidth)}; " +
                   $"height: {faceRectangle.Height.ToCssPercentageString(imageHeight)};";
        }

        public static string ToCss(this string boundingBox, int imageWidth, int imageHeight)
        {
            var box = ParseBox(boundingBox);
            return $"left: {box[0].ToCssPercentageString(imageWidth)}; " +
                   $"top: {box[1].ToCssPercentageString(imageHeight)}; " +
                   $"width: {box[2].ToCssPercentageString(imageWidth)}; " +
                   $"height: {box[3].ToCssPercentageString(imageHeight)};";
        }

        public static string ToCss(this IList<double> boundingBox, int imageWidth, int imageHeight)
        {
            // BoundingBox: Bounding box of a recognized region, line, or word, depending on the parent object.
            // The eight integers represent the four points (x-coordinate, y-coordinate) of the detected rectangle
            // from the left-top corner and clockwise.
            var bb = new RecognizeTextRotatedBoundingBox(boundingBox);
            return $"left: {bb.MinLeft().ToCssPercentageString(imageWidth)}; " +
                   $"top: {bb.MinTop().ToCssPercentageString(imageHeight)}; " +
                   $"width: {bb.MaxWidth().ToCssPercentageString(imageWidth)}; " +
                   $"height: {bb.MaxHeight().ToCssPercentageString(imageHeight)};";
        }

        public static string ToCss(this Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.FaceRectangle boundingRect, int imageWidth, int imageHeight)
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

        public static string ToCss(this BoundingBox boundingRect)
        {
            return $"left: {boundingRect.Left.ToCssPercentageString()}; " +
                   $"top: {boundingRect.Top.ToCssPercentageString()}; " +
                   $"width: {boundingRect.Width.ToCssPercentageString()}; " +
                   $"height: {boundingRect.Height.ToCssPercentageString()};";
        }

        private static int[] ParseBox(this string boundingBox)
        {
            return boundingBox.Split(',').Select(int.Parse).ToArray();
        }

        public static string ToCssPercentageString(this double value, int fullValue)
        {
            var percentage = value / fullValue;
            var truncated = TruncateDecimal(percentage, 5);

            return truncated.ToCssPercentageString();
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

        public static string ToPercentage(this double score)
        {
            return $"{score:P}";
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

            return text.ToCapitalized() + ".";
        }

        public static string ToCapitalized(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return text.First().ToString().ToUpper() + text.Substring(1);
        }

        public static string ToCombinedTexts(this OcrResult ocrResult)
        {
            var combinedText = new StringBuilder();

            foreach (var region in ocrResult.Regions)
            {
                foreach (var line in region.Lines)
                {
                    combinedText.AppendLine(string.Join(' ', line.Words.Select(x => x.Text)));
                }

                combinedText.AppendLine();
            }

            return combinedText.ToString();
        }

        public static string ToCombinedTexts(this TextOperationResult textOperationResult)
        {
            var combinedText = new StringBuilder();

            foreach (var line in textOperationResult.RecognitionResult.Lines)
            {
                combinedText.AppendLine(string.Join(' ', line.Words.Select(x => x.Text)));
            }

            return combinedText.ToString();
        }

        public static string ToCombinedTexts(this ReadOperationResult readOperationResult)
        {
            var combinedText = new StringBuilder();

            foreach (var region in readOperationResult.RecognitionResults)
            {
                foreach (var line in region.Lines)
                {
                    combinedText.AppendLine(string.Join(' ', line.Words.Select(x => x.Text)));
                }

                combinedText.AppendLine();
            }

            return combinedText.ToString();
        }
    }
}
