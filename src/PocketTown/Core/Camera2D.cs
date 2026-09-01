using Microsoft.Xna.Framework;

namespace PocketTown.Core;

/// <summary>
/// Simple 2D camera that follows a target and clamps itself to the map bounds.
/// Positions are rounded to whole pixels to keep the pixel art crisp.
/// </summary>
public class Camera2D
{
    public Vector2 Center;

    public Matrix Transform => Matrix.CreateTranslation(
        -MathF.Round(Center.X - Constants.VirtualWidth / 2f),
        -MathF.Round(Center.Y - Constants.VirtualHeight / 2f),
        0f);

    /// <summary>Center on <paramref name="target"/>, clamped inside a map of the given pixel size.</summary>
    public void Follow(Vector2 target, int mapPixelWidth, int mapPixelHeight)
    {
        float halfW = Constants.VirtualWidth / 2f;
        float halfH = Constants.VirtualHeight / 2f;

        Center.X = mapPixelWidth <= Constants.VirtualWidth
            ? mapPixelWidth / 2f
            : MathHelper.Clamp(target.X, halfW, mapPixelWidth - halfW);

        Center.Y = mapPixelHeight <= Constants.VirtualHeight
            ? mapPixelHeight / 2f
            : MathHelper.Clamp(target.Y, halfH, mapPixelHeight - halfH);
    }
}
