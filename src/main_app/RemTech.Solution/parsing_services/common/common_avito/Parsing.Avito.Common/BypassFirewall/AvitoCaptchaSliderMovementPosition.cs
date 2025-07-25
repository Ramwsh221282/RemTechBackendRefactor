using OpenCvSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoCaptchaSliderMovementPosition(AvitoCaptchaImages images)
{
    public int CenterPoint()
    {
        using DecodedCaptchaImage background = new(images.ReadMax());
        using DecodedCaptchaImage puzzle = new(images.ReadMin());
        return CalculateCenterPoint(background, puzzle);
    }

    private int CalculateCenterPoint(DecodedCaptchaImage background, DecodedCaptchaImage puzzle)
    {
        try
        {
            using Mat edgePuzzle = Edges(puzzle.Read());
            using Mat edgeBackground = Edges(background.Read());
            using Mat rgbPuzzle = MatBgr2Rgb(edgePuzzle);
            using Mat rgbBackground = MatBgr2Rgb(edgeBackground);
            int centerPoint = Calculate(rgbBackground, edgePuzzle, edgePuzzle);
            return centerPoint;
        }
        catch
        {
            return 1;
        }
    }

    private Mat Edges(Mat mat)
    {
        Mat edges = new();
        Cv2.Canny(mat, edges, 100, 200);
        return edges;
    }

    private Mat MatBgr2Rgb(Mat edged)
    {
        Mat bgr2Rgb = new();
        Cv2.CvtColor(edged, bgr2Rgb, ColorConversionCodes.BGR2RGB);
        return bgr2Rgb;
    }

    private int Calculate(
        Mat edgeBackgroundRgb,
        Mat edgePuzzleRgb,
        Mat edgePuzzle
    )
    {
        Mat result = new();
        Cv2.MatchTemplate(
            edgeBackgroundRgb,
            edgePuzzleRgb,
            result,
            TemplateMatchModes.CCoeffNormed);
        Cv2.MinMaxLoc(result, out _, out _, out _, out Point maxLoc);
        int w = edgePuzzle.Cols;
        int center = maxLoc.X + w / 2;
        result.Dispose();
        return center;
    }
}