using System;
using System.IO;
using ClassicUO.IO.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.IO;

public class UOAnimatedTexture
{
    public byte[][] RawTextures;
    public Texture2D[] Textures;
    public int FPS;
    public UOSound SoundData;

    public UOAnimatedTexture(byte[][] textures, int fps, UOSound sound)
    {
        RawTextures = textures;
        FPS = fps;
        SoundData = sound;
    }

    public Texture2D GetTexture(GraphicsDevice graphicsDevice, int num)
    {
        try
        {
            if (Textures == null)
            {
                
                Textures = new Texture2D[RawTextures.Length];

                for (int i = 0; i < RawTextures.Length; i++)
                {
                    Textures[i] = Texture2D.FromStream(graphicsDevice, new MemoryStream(RawTextures[i]));
                }
            }
            if (num >= Textures.Length)
            {
                return null;
            }

            return Textures[num];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return null;
        }
    }
}