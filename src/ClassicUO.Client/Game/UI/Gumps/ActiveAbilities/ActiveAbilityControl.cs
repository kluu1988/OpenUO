using System;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Gumps
{
    internal class ActiveAbilityControl : Control
    {
        private Control _Button;
        public bool Locked { get; set; }
        public bool InUse { get; set; }

        public ActiveAbility Ability;

        private int m_Size { get; set; }

        public ActiveAbilityControl(ActiveAbility ability, int row, int item, int size)
        {
            m_Size = size;
            Ability = ability;
            if (ability.CooldownEnd <= DateTime.UtcNow && ability.InUseUntil <= DateTime.UtcNow)
            {
                Locked = false;
                _Button = new Button(item, 
                                     (ushort) ability.IconLarge, 
                                     (ushort) ability.IconLarge, 
                                     (ushort) ability.IconLarge)
                {
                    X = ActiveAbilitiesGump.BorderAround / 2, Y = ActiveAbilitiesGump.BorderAround / 2,
                    AcceptMouseInput = false,
                    Width = size,
                    Height = size
                };
                
                MouseDown += ((sender, args) =>
                {
                    GameActions.UseActiveBySlot(row, item);
                    Console.WriteLine($"{row} {item}");
                });
                
                
                
                Add(_Button);
            }
            else if (ability.InUseUntil >= DateTime.UtcNow)
            {
                InUse = true;
                Add(new SquareProgressionTimed(34, 
                                               ability.InUseStart, 
                                               ability.InUseUntil, 
                                               0,
                                               0, 0, 0, 0)
                {
                    
                    X = 0, Y = 0,
                    Alpha = 0.7f,
                    TrackSize = (size + ActiveAbilitiesGump.BorderAround) / 7,
                    CountdownHue = 0x0481,
                    CircleHue = 0x50,
                    TextInMiddle = true,
                    Precision = 0,
                    Width = size + ActiveAbilitiesGump.BorderAround,
                    Height = size + ActiveAbilitiesGump.BorderAround,
                    AcceptMouseInput = false,
                    //Texture = SolidColorTextureCache.GetTexture(Color.Gold)
                    
                    //CircleHue = k.CircleHue,
                    //TextHue = k.TextHue,

                });
                
                _Button = new GumpPic(0, 0, (ushort)ability.IconLarge, 0)
                {
                    X = ActiveAbilitiesGump.BorderAround / 2, Y = ActiveAbilitiesGump.BorderAround / 2,
                    Width = size,
                    Height = size,
                    AcceptMouseInput = false,
                };

                Add(_Button);
                Add
                (
                    new AlphaBlendControl(0.7f, Color.Black)
                    {
                        Width = _Button.Width,
                        Height = _Button.Height,
                        X = ActiveAbilitiesGump.BorderAround / 2, Y = ActiveAbilitiesGump.BorderAround / 2,
                        AcceptMouseInput = false,
                    }
                );
                
            }
            else
            {
                Locked = true;
                _Button = new GumpPic(0, 0, (ushort)ability.IconLarge, 0)
                {
                    X = ActiveAbilitiesGump.BorderAround / 2, Y = ActiveAbilitiesGump.BorderAround / 2,
                    Width = size,
                    Height = size,
                    AcceptMouseInput = false,
                };

                Add(_Button);
                Add
                (
                    new AlphaBlendControl(0.7f, Color.Black)
                    {
                        Width = _Button.Width,
                        Height = _Button.Height,
                        X = ActiveAbilitiesGump.BorderAround / 2, Y = ActiveAbilitiesGump.BorderAround / 2,
                        AcceptMouseInput = false,
                    }
                );
                
                Add(new SquareProgressionTimed(34, 
                                               ability.CooldownStart, 
                                               ability.CooldownEnd, 
                                               0,
                                                 0, 0, 0, 0)
                {
                    
                    X = ActiveAbilitiesGump.BorderAround / 2, Y = ActiveAbilitiesGump.BorderAround / 2,
                    Alpha = 0.3f,
                    TrackSize = size / 7,
                    CountdownHue = 0x0481,
                    CircleHue = 0x59,
                    TextInMiddle = true,
                    Precision = 0,
                    Width = size,
                    Height = size,
                    AcceptMouseInput = false,
                    //CircleHue = k.CircleHue,
                    //TextHue = k.TextHue,

                });

                /*Add(new Label($"{ability.Charges}", true, 0x0481, 0)
                {
                    X = ActiveAbilitiesGump.BorderAround / 2, Y = ActiveAbilitiesGump.BorderAround / 2,
                });*/
            }
            SetTooltip($"<BASEFONT COLOR=#AA00AA>{ability.Name}</BASEFONT>\n<BASEFONT COLOR=#2222CC>Cooldown: {ability.Cooldown.TotalSeconds} seconds</BASEFONT>\n{ability.Description}");
            Height = _Button.Height + ActiveAbilitiesGump.BorderAround;
            Width = _Button.Width + ActiveAbilitiesGump.BorderAround;
            //WantUpdateSize = true;
        }

        private float borderAlpha = 0.8f;
        private bool minus = false;

        public override bool Draw(UltimaBatcher2D batcher, int x, int y)
        {
            Vector3 hue_vec = ShaderHueTranslator.GetHueVector(0, false, 1);
            //batcher.dr
            batcher.DrawSolidRectangle
            (
                SolidColorTextureCache.GetTexture(Color.Black),
                x,
                y,
                m_Size + ActiveAbilitiesGump.BorderAround,
                m_Size + ActiveAbilitiesGump.BorderAround,
                hue_vec
            );

            if (Ability.UseNextMove)
            {
                if (!minus)
                    borderAlpha += 0.002f;
                else
                    borderAlpha -= 0.002f;

                if (!minus)
                {
                    if (borderAlpha >= 1f)
                        minus = true;
                }
                else
                {
                    if (borderAlpha <= 0.4f)
                        minus = false;
                }

            }
            else
            {
                
                borderAlpha = 0.8f;
            }
            hue_vec = ShaderHueTranslator.GetHueVector(0, false, borderAlpha);
            batcher.DrawSolidRectangle
            (
                Locked || InUse ? SolidColorTextureCache.GetTexture(Color.DarkGray) : SolidColorTextureCache.GetTexture(Color.Gold),
                x + ActiveAbilitiesGump.BorderAround / 4,
                y + ActiveAbilitiesGump.BorderAround / 4,
                m_Size + ActiveAbilitiesGump.BorderAround / 2,
                m_Size + ActiveAbilitiesGump.BorderAround / 2,
                hue_vec
            );
            
            var ret = base.Draw(batcher, x, y);
            if (Ability.Charges > 0)
            {
                string charges = $"{Ability.Charges}";
                Vector2 size = Fonts.Bold.MeasureString(charges);
                batcher.DrawString
                (
                    Fonts.Bold, charges, (int)Width - (int)size.X + x - 4, (int)Width - (int)size.Y + y - 4,
                    ShaderHueTranslator.GetHueVector(0x0481, false, 1)
                );
            }

            return ret;
        }
    }
    
}