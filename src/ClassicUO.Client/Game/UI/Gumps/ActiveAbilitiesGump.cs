using System;
using System.Collections.Generic;
using System.Xml;
using ClassicUO.Game.Data;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Input;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Gumps
{
    internal class ActiveAbility
    {
        public string Name;
        public string Description;
        public int IconLarge;
        public TimeSpan Cooldown;
        public TimeSpan CooldownRemaining;
        public short Hue;
        public short Charges;
        public bool UseNextMove;
    }

    internal class ActiveAbilityObject
    {
        public string Name;
        public int Serial;
        public List<ActiveAbility> Abilities;
    }

    
    internal class ActiveAbilitiesGump : Gump
    {

        private bool _expanding;
        private int _width;

        private AlphaBlendControl _alphaBlendControl;

        public ActiveAbilitiesGump() : base(0, 0)
        {
            _expanding = false;
            CanMove = true;
            CanCloseWithRightClick = true;
            AcceptMouseInput = true;
        }

        public ActiveAbilitiesGump(int x, int y) : this()
        {
            X = x;
            Y = y;


            SetInScreen();

            BuildGump();

        }


        protected override void UpdateContents()
        {
            BuildGump();
        }


        private void BuildGump()
        {

            if (_width < 40)
                _width = 40;

            WantUpdateSize = true;

            //_box?.Clear();
            //_box?.Children.Clear();

            Clear();



            Add(_alphaBlendControl = new AlphaBlendControl(0.1f, Color.Aquamarine)
            {
                Width = _width,
                Height = 50,
            });


            UpdateElements();
        }

        private void UpdateElements()
        {
        }

        public override void Update()
        {
            if (Mouse.LButtonPressed && _expanding)
            {
                var width = Mouse.Position.X - (X);

                _width = Width = Math.Max(width, 40);

                RequestUpdateContents();
            }
            else
            {
                _expanding = false;
            }

            base.Update();
        }

        public override void Save(XmlTextWriter writer)
        {
            base.Save(writer);
            writer.WriteAttributeString("width", ((int)_width).ToString());
        }

        public override void Restore(XmlElement xml)
        {
            base.Restore(xml);


            try
            {
                _width = Math.Max(int.Parse(xml.GetAttribute("width")), 40);
            }
            catch
            {
                _width = 200;
            }

            BuildGump();
        }
    }
}