using System;
using System.Collections.Generic;
using System.Linq;

namespace Orneholm.CognitiveWorkbench.Web.Models.Generic
{
    public class RecognizeTextRotatedBoundingBox
    {
        public RecognizeTextRotatedBoundingBox(IList<double> boundingBoxCoords)
        {
            var boundingBoxPoints = new List<RecognizeTextRotatedBoundingBoxPoint>
            {
                new RecognizeTextRotatedBoundingBoxPoint
                {
                    X = boundingBoxCoords[0],
                    Y = boundingBoxCoords[1]
                },
                new RecognizeTextRotatedBoundingBoxPoint
                {
                    X = boundingBoxCoords[2],
                    Y = boundingBoxCoords[3]
                },
                new RecognizeTextRotatedBoundingBoxPoint
                {
                    X = boundingBoxCoords[4],
                    Y = boundingBoxCoords[5]
                },
                new RecognizeTextRotatedBoundingBoxPoint
                {
                    X = boundingBoxCoords[6],
                    Y = boundingBoxCoords[7]
                }
            };

            // Points may not be ordered correctly 
            TopLeft = boundingBoxPoints
                .OrderBy(p => p.X).Take(2)
                .OrderBy(p => p.Y).First();
            TopRight = boundingBoxPoints
                .OrderByDescending(p => p.X).Take(2)
                .OrderBy(p => p.Y).First();
            BottomRight = boundingBoxPoints
                .OrderByDescending(p => p.X).Take(2)
                .OrderByDescending(p => p.Y).First();
            BottomLeft = boundingBoxPoints
                .OrderBy(p => p.X).Take(2)
                .OrderByDescending(p => p.Y).First();
        }

        public RecognizeTextRotatedBoundingBoxPoint TopLeft { get; set; }

        public RecognizeTextRotatedBoundingBoxPoint TopRight { get; set; }

        public RecognizeTextRotatedBoundingBoxPoint BottomLeft { get; set; }

        public RecognizeTextRotatedBoundingBoxPoint BottomRight { get; set; }

        public double MinLeft()
        {
            return Math.Min(TopLeft.X, BottomLeft.X);
        }

        public double MinTop()
        {
            return Math.Min(TopLeft.Y, TopRight.Y);
        }

        public double MaxWidth()
        {
            return Math.Max(TopRight.X, BottomRight.X) - Math.Min(TopLeft.X, BottomLeft.X);
        }

        public double MaxHeight()
        {
            return Math.Max(BottomLeft.Y, BottomRight.Y) - Math.Min(TopLeft.Y, TopRight.Y);
        }
    }
}
