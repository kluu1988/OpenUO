using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using ClassicUO.Assets;
using ClassicUO.Configuration;
using ClassicUO.Game;
using ClassicUO.Game.Data.OpenUO;
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

    private static void EnhancedPotionMacrosPacket(ref StackDataReader p)
    {
        int version = p.ReadUInt16BE();

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
    
    private static void DefaultMovementSpeedPacket(ref StackDataReader p)
    {
        int version = p.ReadUInt16BE();

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

    private static void SettingsPacket(ref StackDataReader p)
    {
        int version = p.ReadUInt16BE();

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
        Handler.HandlePacket(p.ReadUInt16BE(), ref p);
    }
    
    public static EnhancedPacketHandler Handler { get; } = new EnhancedPacketHandler();
    
    public void Add(ushort id, PacketHandlers.OnPacketBufferReader handler)
        => _handlers[id] = handler;

    public void HandlePacket(ushort packetID, ref StackDataReader p)
    {
        if (_handlers.ContainsKey(packetID))
        {
            _handlers[packetID].Invoke(ref p);
        }
        else
        {
            Console.WriteLine($"Received invalid enhanced packet {packetID} (0x{packetID:X}) len={p.Length}");
        }
    }


    private readonly Dictionary<ushort, PacketHandlers.OnPacketBufferReader> _handlers = new Dictionary<ushort, PacketHandlers.OnPacketBufferReader>();

    private static void InvalidVersionReceived(ref StackDataReader p)
    {
        Console.WriteLine($"Version of a packet recieved, likely client is out of date.");
    }
}