using System;
using System.Collections.Generic;
using System.Linq;
using ClassicUO.Assets;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Resources;

namespace ClassicUO.Game.UI.Gumps;

internal class MagerySpellbookGump : BaseSpellbookGump
{
    public MagerySpellbookGump(World world, uint item) : base(world, item) { }
    public MagerySpellbookGump(World world) : base(world) { }
    private enum ButtonCircle
    {
        Circle_1_2,
        Circle_3_4,
        Circle_5_6,
        Circle_7_8
    }
    
    protected override void GetSpellToolTip(out int offset)
    {
        offset = 1061290;
    }
    protected override SpellDefinition GetSpellDefinition(int idx) => SpellsMagery.GetSpell(idx);

    protected override void GetSpellNames(int offset, out string name, out string abbreviature, out string reagents)
    {
        SpellDefinition def = SpellsMagery.GetSpell(offset + 1);
        name = def.Name;
        abbreviature = SpellsMagery.SpecialReagentsChars[offset];
        reagents = def.CreateReagentListString("\n");
    }

    protected override void GetBookInfo
    (
        out ushort bookGraphic, out ushort minimizedGraphic, out ushort iconStartGraphic, out int maxSpellsCount, out int spellsOnPage, out int dictionaryPagesCount
    )
    {
        maxSpellsCount = SpellsMagery.MaxSpellCount;
        bookGraphic = 0x08AC;
        minimizedGraphic = 0x08BA;
        iconStartGraphic = 0x08C0;

        spellsOnPage = SpellsMagery.GetAllCircles.Values.Max((c) => c.Count);
        dictionaryPagesCount = SpellsMagery.GetAllCircles.Count;

        if (dictionaryPagesCount % 2 != 0)
        {
            dictionaryPagesCount++;
        }
    }

    protected override void BuildGump()
    {
        Item item = World.Items.Get(LocalSerial);

        if (item == null)
        {
            Dispose();

            return;
        }

        //AssignGraphic(item);

        GetBookInfo(
            out ushort bookGraphic,
            out ushort minimizedGraphic,
            out ushort iconStartGraphic,
            out int maxSpellsCount,
            out int spellsOnPage,
            out int dictionaryPagesCount
        );

        //Add(new GumpPicInPic(0, 140, bookGraphic, 0, 140, 380, 40) { });
        if (spellsOnPage == 8)
        {
            Add(new GumpPic(0, 0, bookGraphic, 0) { });
            Add(_picBase = new GumpPic(0, 0, bookGraphic, 0) { IsVisible = false });
        }
        else {
            Add(new GumpPicInPic(0, 0, bookGraphic, 0,0,380,180));
            int fillers = (spellsOnPage - 8) / 2;

            for (int i = 0; i < fillers; i++)
            {
                Add(new GumpPicInPic(0, 140 + (30 * i), bookGraphic, 0, 140, 380, 30) { });
            }
            Add(new GumpPicInPic(0, 140 + ((spellsOnPage - 8) * 15), bookGraphic, 0, 140, 380, 90) { });
            Add(_picBase = new GumpPic(0, 0, bookGraphic, 0) { IsVisible = false });
        }
        _picBase.MouseDoubleClick += _picBase_MouseDoubleClick;
        //_picBase.MouseDoubleClick += _picBase_MouseDoubleClick;

        _dataBox = new DataBox(0, 0, 0, 0)
        {
            CanMove = true,
            AcceptMouseInput = true,
            WantUpdateSize = true
        };

        Add(_dataBox);
        _hitBox = new HitBox(0, 98, 27, 23);
        Add(_hitBox);
        _hitBox.MouseUp += _hitBox_MouseUp;

        Add(_pageCornerLeft = new GumpPic(50, 8, 0x08BB, 0));
        _pageCornerLeft.LocalSerial = 0;
        _pageCornerLeft.Page = int.MaxValue;
        _pageCornerLeft.MouseUp += PageCornerOnMouseClick;
        _pageCornerLeft.MouseDoubleClick += PageCornerOnMouseDoubleClick;
        Add(_pageCornerRight = new GumpPic(321, 8, 0x08BC, 0));
        _pageCornerRight.LocalSerial = 1;
        _pageCornerRight.Page = 1;
        _pageCornerRight.MouseUp += PageCornerOnMouseClick;
        _pageCornerRight.MouseDoubleClick += PageCornerOnMouseDoubleClick;

        RequestUpdateContents();

        Client.Game.Audio.PlaySound(0x0055);
    }

    protected override void CreateBook()
    {
        _dataBox.Clear();
        _dataBox.WantUpdateSize = true;

        GetBookInfo(
            out ushort bookGraphic,
            out ushort minimizedGraphic,
            out ushort iconStartGraphic,
            out int maxSpellsCount,
            out int spellsOnPage,
            out int dictionaryPagesCount
        );

        int totalSpells = 0;

        Item item = World.Items.Get(LocalSerial);

        if (item == null)
        {
            Dispose();

            return;
        }

        for (LinkedObject i = item.Items; i != null; i = i.Next)
        {
            Item spell = (Item)i;
            int currentCount = spell.Amount;

            if (currentCount > 0 && currentCount <= maxSpellsCount)
            {
                _spells[currentCount - 1] = true;
                totalSpells++;
            }
        }

        int pagesToFill = dictionaryPagesCount >> 1;

        _maxPage = pagesToFill + ((totalSpells + 1) >> 1);

        int currentSpellIndex = 0;

        _dataBox.Add(
            new Button((int)ButtonCircle.Circle_1_2, 0x08B1, 0x08B1)
            {
                X = 58,
                Y = 175 + ((spellsOnPage - 8) * 15),
                ButtonAction = ButtonAction.Activate,
                ToPage = 1
            }
        );

        _dataBox.Add(
            new Button((int)ButtonCircle.Circle_1_2, 0x08B2, 0x08B2)
            {
                X = 93,
                Y = 175 + ((spellsOnPage - 8) * 15),
                ButtonAction = ButtonAction.Activate,
                ToPage = 1
            }
        );

        _dataBox.Add(
            new Button((int)ButtonCircle.Circle_3_4, 0x08B3, 0x08B3)
            {
                X = 130,
                Y = 175 + ((spellsOnPage - 8) * 15),
                ButtonAction = ButtonAction.Activate,
                ToPage = 2
            }
        );

        _dataBox.Add(
            new Button((int)ButtonCircle.Circle_3_4, 0x08B4, 0x08B4)
            {
                X = 164,
                Y = 175 + ((spellsOnPage - 8) * 15),
                ButtonAction = ButtonAction.Activate,
                ToPage = 2
            }
        );

        _dataBox.Add(
            new Button((int)ButtonCircle.Circle_5_6, 0x08B5, 0x08B5)
            {
                X = 227,
                Y = 175 + ((spellsOnPage - 8) * 15),
                ButtonAction = ButtonAction.Activate,
                ToPage = 3
            }
        );

        _dataBox.Add(
            new Button((int)ButtonCircle.Circle_5_6, 0x08B6, 0x08B6)
            {
                X = 260,
                Y = 175 + ((spellsOnPage - 8) * 15),
                ButtonAction = ButtonAction.Activate,
                ToPage = 3
            }
        );

        _dataBox.Add(
            new Button((int)ButtonCircle.Circle_7_8, 0x08B7, 0x08B7)
            {
                X = 297,
                Y = 175 + ((spellsOnPage - 8) * 15),
                ButtonAction = ButtonAction.Activate,
                ToPage = 4
            }
        );

        _dataBox.Add(
            new Button((int)ButtonCircle.Circle_7_8, 0x08B8, 0x08B8)
            {
                X = 332,
                Y = 175 + ((spellsOnPage - 8) * 15),
                ButtonAction = ButtonAction.Activate,
                ToPage = 4
            }
        );

        int spellDone = 0;

        for (int page = 1; page <= pagesToFill; page++)
        {
            for (int j = 0; j < 2; j++)
            {
                
                int indexX = 106;
                int dataX = 62;
                int y = 0;

                if (j % 2 != 0)
                {
                    indexX = 269;
                    dataX = 225;
                }

                Label text = new Label(ResGumps.Index, false, 0x0288, font: 6)
                {
                    X = indexX,
                    Y = 10
                };

                _dataBox.Add(text, page);
                
                text = new Label(
                    SpellsMagery.CircleNames[(page - 1) * 2 + j % 2],
                    false,
                    0x0288,
                    font: 6
                )
                {
                    X = dataX,
                    Y = 30
                };

                _dataBox.Add(text, page);

                int topage = pagesToFill + ((spellDone + 1) >> 1);

                //for (int k = 0; k < spellsOnPage; k++, currentSpellIndex++)
                foreach (var spell in SpellsMagery.GetAllCircles[((page - 1) * 2 + j % 2) + 1])
                {
                    bool[] spells = _spells;

                    if ((spell.ID - 1) >= 64)
                        spells = _extraspells;
                    if (spells[(spell.ID - 1) % 64])
                    {
                        GetSpellNames(
                            spell.ID - 1,
                            out string name,
                            out string abbreviature,
                            out string reagents
                        );

                        if (spellDone % 2 == 0)
                        {
                            topage++;
                        }

                        spellDone++;

                        text = new HoveredLabel(
                            name,
                            false,
                            0x0288,
                            0x33,
                            0x0288,
                            font: 9,
                            maxwidth: 130,
                            style: FontStyle.Cropped
                        )
                        {
                            X = dataX,
                            Y = 52 + y,
                            LocalSerial = (uint)topage,
                            AcceptMouseInput = true,
                            Tag = spell.ID + 1,
                            CanMove = true
                        };

                        text.MouseUp += OnLabelMouseUp;
                        text.MouseDoubleClick += OnLabelMouseDoubleClick;
                        _dataBox.Add(text, page);

                        y += 15;
                    }
                }
            }
        }

        int page1 = pagesToFill + 1;
        int topTextY = 6;

        for (int i = 0, spellsDone = 0; i < maxSpellsCount; i++)
        {
            var spells = _spells;

            if (i >= 64)
                spells = _extraspells;
            if (!_spells[i % 64])
            {
                continue;
            }

            int iconX = 62;
            int topTextX = 87;
            int iconTextX = 112;
            uint iconSerial = 100 + (uint)i;

            if (spellsDone > 0)
            {
                if (spellsDone % 2 != 0)
                {
                    iconX = 225;
                    topTextX = 224;
                    iconTextX = 275;
                    iconSerial = 1000 + (uint)i;
                }
                else
                {
                    page1++;
                }
            }

            spellsDone++;

            var spellDef = GetSpellDefinition(iconSerial);
            GetSpellNames(i, out string name, out string abbreviature, out string reagents);

            Label text = new Label(
                SpellsMagery.CircleNames[spellDef.SpellCircle - 1],
                false,
                0x0288,
                font: 6
            )
            {
                X = topTextX,
                Y = topTextY + 4
            };

            _dataBox.Add(text, page1);

            text = new Label(name, false, 0x0288, 80, 6) { X = iconTextX, Y = 34 };

            _dataBox.Add(text, page1);
            int abbreviatureY = 26;

            if (text.Height < 24)
            {
                abbreviatureY = 31;
            }

            abbreviatureY += text.Height;

            text = new Label(abbreviature, false, 0x0288, font: 8)
            {
                X = iconTextX,
                Y = abbreviatureY
            };

            _dataBox.Add(text, page1);
            
            ushort iconGraphic;
            int toolTipCliloc;

            iconGraphic = (ushort)(iconStartGraphic + i);
            GetSpellToolTip(out toolTipCliloc);

            HueGumpPic icon = new HueGumpPic(
                this,
                iconX,
                40,
                iconGraphic,
                0,
                (ushort)spellDef.ID,
                spellDef.Name
            )
            {
                X = iconX,
                Y = 40,
                LocalSerial = iconSerial
            };

            if (toolTipCliloc > 0)
            {
                string tooltip = ClilocLoader.Instance.GetString(toolTipCliloc + i);
                icon.SetTooltip(tooltip, 250);
            }

            icon.MouseDoubleClick += OnIconDoubleClick;
            icon.DragBegin += OnIconDragBegin;

            _dataBox.Add(icon, page1);

            if (!string.IsNullOrEmpty(reagents))
            {
                _dataBox.Add(new GumpPicTiled(iconX, 88, 120, 5, 0x0835), page1);

                Label textReags = new Label(ResGumps.Reagents, false, 0x0288, font: 6)
                {
                    X = iconX,
                    Y = 92
                };

                _dataBox.Add(textReags, page1);

                textReags = new Label(reagents, false, 0x0288, font: 9) { X = iconX, Y = 114 };

                _dataBox.Add(textReags, page1);
            }
        }

        SetActivePage(1);
    }
    
    

    public override void OnButtonClick(int buttonID)
    {
        switch ((ButtonCircle)buttonID)
        {
            case ButtonCircle.Circle_1_2:
                SetActivePage(1);

                break;

            case ButtonCircle.Circle_3_4:
                SetActivePage(2);

                break;

            case ButtonCircle.Circle_5_6:
                SetActivePage(3);

                break;

            case ButtonCircle.Circle_7_8:
                SetActivePage(4);

                break;
        }
    }
    
    
    protected override void GetSpellRequires(int offset, out int y, out string text)
    {
        y = 162;
        text = "";
    }
}