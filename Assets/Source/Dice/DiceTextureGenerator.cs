using UnityEngine;

public static class DiceTextureGenerator
{
    private const int _oneSidePixelWidth = 32;

    /* cube layout:
     x5xx 
     1234 
     x0xx
    */

    public static Texture GetTexture()
    {
        var texture = new Texture2D(_oneSidePixelWidth * 4, _oneSidePixelWidth * 3);

        for (int y = 0; y < texture.height; y += 1)
        {
            for (int x = 0; x < texture.width; x += 1)
            {
                var color = new Color(Random.value, Random.value, Random.value);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }
}
