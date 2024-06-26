# Cognitive Workbench

[![Twitter Follow](https://img.shields.io/badge/Twitter-@PeterOrneholm-blue.svg?logo=twitter)](https://twitter.com/PeterOrneholm)

Cognitive Workbench is a site to learn about and demo the capabilities of Azure Cognitive Services. The site aims to be a complement to already existing demos, samples and sites to make it easier to try out the capabilities, visulize results and explain how the services works.

The sites is targeted towards people familiar with Microsoft Azure and Cognitive Services and will require you to create resources in Azure and bring your own keys.

As of today, the site covers these services and APIs:

## Vision

* Computer Vision
    * AnalyzeImage (ImageType, Faces, Adult, Categories, Color, Tags, Description, Objects, Brands)
    * AreaOfInterest
    * RecognizePrintedText
    * RecognizeText (Will be [deprecated](https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/concept-recognizing-text#recognize-text-api) in favor of Read operation)
    * BatchReadFile
    * Read (API 3.0)
* Face
    * Detect with detection_01 model (Age, Gender, HeadPose, Smile, FacialHair, Glasses, Emotion, Hair, Makeup, Occlusion, Accessories, Blur, Exposure, Noise)
    * Detect with detection_02 model
    * Identify on PersonGroup or LargePersonGroup
* Custom Vision
    * ClassifyImage
    * DetectImage (object detection)

# Language

* Text Analytics
    * Detect languages
    * Entities
    * Keyphrase
    * Sentiment

# Samples

## Computer Vision

### Input

![Input](docs/images/CognitiveWorkbench_Sample_Input.PNG)

### Analyze

![Car](docs/images/CognitiveWorkbench_Sample_VisionCar.PNG)

### OCR

![OCR](docs/images/CognitiveWorkbench_Sample_OCR.PNG)

### RecognizeText

![OCR](docs/images/CognitiveWorkbench_Sample_RecognizeText.png)

## Face

### Detect

![Face](docs/images/CognitiveWorkbench_Sample_Face.PNG)

### Identify

![Face](docs/images/CognitiveWorkbench_Sample_Face_Identify.PNG)

# Contributions

Feel free to contribute. A few ideas of improvements are available as GitHub issues.

Thanks to [@NicoRobPro](https://twitter.com/NicoRobPro) for some great additions to this site.

# Third party sources

Icon in logo provided by Fontawesome (Free).
https://fontawesome.com/icons/brain?style=solid
https://fontawesome.com/license/free
