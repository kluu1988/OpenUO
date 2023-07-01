using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using ClassicUO.Assets;
using ClassicUO.Configuration;
using ClassicUO.Game;
using ClassicUO.Game.Data.OpenUO;
using ClassicUO.Game.Managers;
using ClassicUO.Game.UI.Gumps;
using ClassicUO.IO;
using ClassicUO.Utility;

namespace ClassicUO.Network;

internal class EnhancedPacketHandler
{
    static EnhancedPacketHandler()
    {
        Handler.Add(0x1, SettingsPacket);
        Handler.Add(0x2, DefaultMovementSpeedPacket);
        Handler.Add(0x3, EnhancedPotionMacrosPacket);
        
        Handler.Add(151, ActiveAbilityCompletePacket);
        Handler.Add(150, ActiveAbilityUpdatePacket);
    }
    
    private static void PacketTemplate(ref StackDataReader p)
    {
        int version = p.ReadUInt16BE();

        switch (version)
        {
            case 0:
            {
                break;
            }
            default: InvalidVersionReceived( ref p ); break;
        }
    }

    private static void ActiveAbilityUpdatePacket(ref StackDataReader p, int version)
    {
        switch (version)
        {
            case 0:
            {
                //active abilities update for single item
                    ushort row = p.ReadUInt16BE();
                    ushort slot = p.ReadUInt16BE();
                    
                    TimeSpan cooldown = TimeSpan.FromMilliseconds(p.ReadUInt32BE());
                    TimeSpan cooldownRemaining = TimeSpan.FromMilliseconds(p.ReadUInt32BE());
                    DateTime cooldownEnd = DateTime.UtcNow + cooldownRemaining;
                    short hue = (short) p.ReadUInt16BE();
                    short charge = (short) p.ReadUInt16BE();
                    bool useNextMove = p.ReadUInt16BE() == 0x1 ? true : false;
                    var inUseMS = p.ReadUInt32BE();
                    var inUseUntilMS = p.ReadUInt32BE();
                    DateTime inUseEnd = DateTime.MinValue;
                    DateTime inUseStart = DateTime.MinValue;
                    if (inUseMS > 0 && inUseUntilMS > 0)
                    {
                        TimeSpan inUseTotal = TimeSpan.FromMilliseconds(inUseMS);
                        TimeSpan inUseRemaining = TimeSpan.FromMilliseconds(inUseUntilMS);
                        inUseEnd = DateTime.UtcNow + inUseRemaining;
                        inUseStart = inUseEnd - inUseTotal;
                    }

                    if (row >= ActiveAbilitiesGump.ActiveAbilities.Count)
                    {
                        Console.WriteLine($"Recieved Ability info for an ability I don't have. Ability: {row} {slot}");
                        return;
                    }
                    
                    if (slot >= ActiveAbilitiesGump.ActiveAbilities[row].Abilities.Count)
                    {
                        Console.WriteLine($"Recieved Ability info for an ability I don't have. Ability: {row} {slot}");
                        return;
                    }

                    var a = ActiveAbilitiesGump.ActiveAbilities[row].Abilities[slot];
                    a.Cooldown = cooldown;
                    a.CooldownStart = cooldownEnd - cooldown;
                    a.CooldownEnd = cooldownEnd;
                    a.Hue = hue;
                    a.Charges = charge;
                    a.UseNextMove = useNextMove;
                    a.InUseStart = inUseStart;
                    a.InUseUntil = inUseEnd;
                    
                    var gump = UIManager.GetGump<ActiveAbilitiesGump>();

                    if (gump == null)
                    {
                        UIManager.Add(gump = new ActiveAbilitiesGump());
                    }

                    gump.Updated();
                    
                    break;
            }
        }
    }

    private static void ActiveAbilityCompletePacket(ref StackDataReader p, int version)
    {
        switch (version)
        {
            case 0:
            {
                ushort rows = p.ReadUInt16BE();
                if (rows == 0)
                {
                    
                    UIManager.GetGump<ActiveAbilitiesGump>()?.Dispose();

                    break;
                }
                var list = new List<ActiveAbilityObject>();
                for (int i = 0; i < rows; i++)
                {
                    
                    uint serial = p.ReadUInt32BE();
                    //ushort len = p.ReadUInt16BE();
                    
                    string name = p.ReadASCII(p.ReadUInt16BE())?.Replace('\r', '\n');
                    //string name = p.ReadUnicodeLE((int)len);
                    ushort slots = p.ReadUInt16BE();

                    var obj = new ActiveAbilityObject() { Name = name, Serial = (int)serial, Abilities = new List<ActiveAbility>() };
                    list.Add(obj);

                    for (int j = 0; j < slots; j++)
                    {
                        int abilityName = (int)p.ReadUInt32BE();
                        int abilityDescription = (int)p.ReadUInt32BE();
                        int count = p.ReadUInt8();

                        string args = "";

                        for (int k = 0; k < count; k++)
                        {
                            uint color = p.ReadUInt32BE();
                            float val = ((float)p.ReadUInt32BE()) / 1000f;
                            args += $"<BASEFONT COLOR=#\"{color:x6}>{val}<BASEFONT COLOR=#FFFFFF>";

                            if (k < count - 1)
                                args += "\t";
                        }
                        
                        string description = 
                             ClilocLoader.Instance.Translate((int) abilityDescription, args );
                        ;
                        
                        int gumpid = (int) p.ReadUInt32BE();
                        TimeSpan cooldown = TimeSpan.FromSeconds(p.ReadUInt32BE() / 1000d);
                        TimeSpan cooldownRemaining = TimeSpan.FromSeconds(p.ReadUInt32BE() / 1000d);
                        DateTime cooldownEnd = DateTime.UtcNow + cooldownRemaining;
                        short hue = (short) p.ReadUInt16BE();
                        short charge = (short) p.ReadUInt16BE();
                        bool useNextMove = p.ReadUInt16BE() == 0x1 ? true : false;
                        
                        var inUseMS = p.ReadUInt32BE();
                        var inUseUntilMS = p.ReadUInt32BE();
                        DateTime inUseEnd = DateTime.MinValue;
                        DateTime inUseStart = DateTime.MinValue;
                        if (inUseMS > 0 && inUseUntilMS > 0)
                        {
                            TimeSpan inUseTotal = TimeSpan.FromSeconds(inUseMS / 1000d);
                            TimeSpan inUseRemaining = TimeSpan.FromSeconds(inUseUntilMS / 1000d);
                            inUseEnd = DateTime.UtcNow + inUseRemaining;
                            inUseStart = inUseEnd - inUseTotal;
                        }

                        
                        obj.Abilities.Add(new ActiveAbility()
                        {
                            Name = ClilocLoader.Instance.GetString(abilityName), 
                            Description = description,
                            IconLarge = gumpid,
                            Cooldown = cooldown,
                            CooldownStart = cooldownEnd - cooldown,
                            CooldownEnd = cooldownEnd,
                            Hue = hue,
                            Charges = charge,
                            UseNextMove = useNextMove,
                            InUseStart = inUseStart,
                            InUseUntil = inUseEnd,
                        });
                    }
                }

                ActiveAbilitiesGump.ActiveAbilities = list;

                var gump = UIManager.GetGump<ActiveAbilitiesGump>();

                if (gump == null)
                {
                    UIManager.Add(gump = new ActiveAbilitiesGump());
                }

                gump.Updated();
                break;
            }
        }
    }

    private static void EnhancedPotionMacrosPacket(ref StackDataReader p, int version)
    {
        switch (version)
        {
            case 0:
            {
                ushort count = p.ReadUInt16BE();
                for (int i = 0; i < count; i++)
                {
                    ushort id = p.ReadUInt16BE();
                    int cliloc = p.ReadInt32BE();
                    World.Settings.Potions.Add(new PotionDefinition()
                    {
                        ID = id,
                        Name = StringHelper.CapitalizeAllWords(ClilocLoader.Instance.Translate(cliloc))
                    });
                }
                
                count = p.ReadUInt16BE();
                for (int i = 0; i < count; i++)
                {
                    ushort id = p.ReadUInt16BE();
                    ushort len = p.ReadUInt16BE();
                    string name = p.ReadASCII(len);
                    World.Settings.Potions.Add(new PotionDefinition()
                    {
                        ID = id,
                        Name = StringHelper.CapitalizeAllWords(name)
                    });
                }
                break;
            }
            default: InvalidVersionReceived( ref p ); break;
        }
    }
    
    private static void DefaultMovementSpeedPacket(ref StackDataReader p, int version)
    {
        switch (version)
        {
            case 0:
            {
                World.Settings.MovementSettings.TurnDelay = p.ReadUInt16BE();
                World.Settings.MovementSettings.MoveSpeedWalkingUnmounted = p.ReadUInt16BE();
                World.Settings.MovementSettings.MoveSpeedRunningUnmounted = p.ReadUInt16BE();
                World.Settings.MovementSettings.MoveSpeedWalkingMounted = p.ReadUInt16BE();
                World.Settings.MovementSettings.MoveSpeedRunningMounted = p.ReadUInt16BE();
                break;
            }
            default: InvalidVersionReceived( ref p ); break;
        }
    }

    private static void SettingsPacket(ref StackDataReader p, int version)
    {
        switch (version)
        {
            case 0:
            {
                int length = (int)p.ReadInt32BE();
                byte[] clientOptions = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    clientOptions[i] = p.ReadUInt8();
                }

                length = (int)p.ReadInt32BE();
                byte[] generalOptions = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    generalOptions[i] = p.ReadUInt8();
                }

                length = (int)p.ReadInt32BE();
                byte[] macroOptions = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    macroOptions[i] = p.ReadUInt8();
                }

                var props = typeof(SettingGeneralFlags).GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                {
                    var id = GetID(prop);

                    if (id > -1)
                    {
                        var isOn = IsSettingOn(generalOptions, id);
                        Console.WriteLine($"{prop.Name} => {isOn}");
                        prop.SetValue(World.Settings.GeneralFlags, isOn);
                    }
                }

                props = typeof(SettingsMacrosFlags).GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                {
                    var id = GetID(prop);
                    if (id > -1)
                    {
                        var isOn = IsSettingOn(macroOptions, id);
                        Console.WriteLine($"{prop.Name} => {isOn}");
                        prop.SetValue(World.Settings.MacroFlags, isOn);
                    }
                }

                props = typeof(SettingOptionFlags).GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                {
                    var id = GetID(prop);
                    if (id > -1)
                    {
                        var isOn = IsSettingOn(clientOptions, id);
                        Console.WriteLine($"{prop.Name} => {isOn}");
                        prop.SetValue(World.Settings.ClientOptionFlags, isOn);
                    }
                }
                TopBarGump.Create();
                break;
            }
            default: InvalidVersionReceived( ref p ); break;
        }
    }
    
    private static bool IsSettingOn(byte[] settings, int id)
    {
        var pos = id / 8;
        var bit = id % 8;
        var val = settings[pos] & (1 << bit);

        if (val != 0)
            return true;
        return false;
    }
    private static int GetID(FieldInfo prop)
    {
        object[] attrs = prop.GetCustomAttributes(typeof(OptionIDAttribute), false);

        return attrs.Length > 0 ? (attrs[0] as OptionIDAttribute).ID : -1;
    }

    
    public static void OpenUOEnhancedRx(ref StackDataReader p)
    {
        ushort id = p.ReadUInt16BE();
        ushort ver = p.ReadUInt16BE();
        Handler.HandlePacket(id, ref p, ver);
    }
    
    public delegate void EnhancedOnPacketBufferReader(ref StackDataReader p, int version);
    public static EnhancedPacketHandler Handler { get; } = new EnhancedPacketHandler();
    
    public void Add(ushort id, EnhancedOnPacketBufferReader handler)
        => _handlers[id] = handler;

    public void HandlePacket(ushort packetID, ref StackDataReader p, int version)
    {
        if (_handlers.ContainsKey(packetID))
        {
            _handlers[packetID].Invoke(ref p, version);
        }
        else
        {
            Console.WriteLine($"Received invalid enhanced packet {packetID} (0x{packetID:X}) len={p.Length}");
        }
    }


    private readonly Dictionary<ushort, EnhancedOnPacketBufferReader> _handlers = new Dictionary<ushort, EnhancedOnPacketBufferReader>();

    private static void InvalidVersionReceived(ref StackDataReader p)
    {
        Console.WriteLine($"Version of a packet recieved, likely client is out of date.");
    }
}