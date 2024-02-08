using System;
using ClassicUO.Assets;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Game.UI.Controls
{

    internal class AnimatedGumpPic : GumpPicBase
    {
        public delegate void FinishCallback();
        public bool IsPartialHue { get; set; }
        public int TextureNum;
        public Texture2D Texture;
        public string Name;
        DateTime LastChanged;
        bool Loop;
        FinishCallback OnFinish;
        float Scale = 1.25f;
        public AnimatedGumpPic(string name, int x, int y, ushort hue, bool loop, bool sound, float scale = 1, FinishCallback finish = null)
        {
            X = x;
            Y = y;
            Name = name;
            TextureNum = 0;
            Loop = loop;
            OnFinish = finish;
            Scale = scale;
            if (GumpsLoader.Instance.AnimatedGumps.ContainsKey(name))
            {
                Texture = GumpsLoader.Instance.AnimatedGumps[name].GetTexture( Client.Game.GraphicsDevice, 0);
                LastChanged = DateTime.UtcNow;
                if (GumpsLoader.Instance.AnimatedGumps[name].SoundData != null)
                {
                    Console.WriteLine("Play Sound");
                    Client.Game.Audio.PlaySound(GumpsLoader.Instance.AnimatedGumps[name].SoundData);

                }
            }
            if (Texture == null)
            {
                Console.WriteLine("Animated Texture not found, disposing");
                Dispose();
            }
            else
            {
                Width = (int)(Texture.Width * Scale);
                Height = (int)(Texture.Height * Scale);
            }
        }
        public override void Dispose()
        {
            if (GumpsLoader.Instance.AnimatedGumps.ContainsKey(Name))
                if (GumpsLoader.Instance.AnimatedGumps[Name].SoundData != null)
                    if (GumpsLoader.Instance.AnimatedGumps[Name].SoundData.IsPlaying(Time.Ticks))
                        GumpsLoader.Instance.AnimatedGumps[Name].SoundData.Stop();
            //Client.Game.Scene.Audio.StopMusic();
            base.Dispose();

        }

        public override bool Draw(UltimaBatcher2D batcher, int x, int y)
        {
            if (IsDisposed)
                return false;
            if (DateTime.UtcNow - LastChanged > TimeSpan.FromSeconds((double)1 / GumpsLoader.Instance.AnimatedGumps[Name].FPS))
            {
                LastChanged = DateTime.UtcNow;
                TextureNum++;
                Texture = GumpsLoader.Instance.AnimatedGumps[Name].GetTexture(Client.Game.GraphicsDevice, TextureNum);
            }
            if (Texture == null)
            {
                if (Loop)
                {
                    TextureNum = 0;
                    Texture = GumpsLoader.Instance.AnimatedGumps[Name].GetTexture(Client.Game.GraphicsDevice, TextureNum);
                }
                else
                {
                    if (OnFinish != null)
                        OnFinish.Invoke();

                    Dispose();
                    return false;
                }
            }
            Vector3 hueVec = ShaderHueTranslator.GetHueVector
            (
                Hue,
                IsPartialHue,
                Alpha,
                true
            );

            if (Texture != null)
            {
                batcher.Draw
                (
                    Texture,
                    new Rectangle(
                    x,
                    y,
                    Width,
                    Height),
                    hueVec
                );
            }


            return base.Draw(batcher, x, y);
        }
    }

}
