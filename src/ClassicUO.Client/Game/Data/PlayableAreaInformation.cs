using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.Data;

public class PlayableAreaInformation
{
    private bool m_Blocking;
    private ushort m_Hue;
    public ushort Hue => m_Hue;
    private List<Rectangle> m_Area;
    public bool Blocking => m_Blocking;

    public PlayableAreaInformation(bool blocking, ushort hue, List<Rectangle> area)
    {
        m_Blocking = blocking;
        m_Hue = hue;
        m_Area = area;
    }

    public bool HighLightObject(int x, int y, out ushort hue)
    {
        hue = m_Hue;
        return !Contains(x, y);
    }
    
    public bool Contains(int x, int y)
    {
        for (int i = 0; i < m_Area.Count; i++)
        {
            var rect = m_Area[i];

            if (rect.Contains(x, y))
            {
                return true;
            }
        }
        return false;
    }
    public bool ContainsY(int y)
    {
        for (int i = 0; i < m_Area.Count; i++)
        {
            var rect = m_Area[i];

            if (y >= m_Area[i].Y && y <= m_Area[i].Y + m_Area[i].Height)
            {
                return true;
            }
        }
        return false;
    }
    
    public List<int> XInvertEdgesOfY(int y)
    {
        List<Tuple<int,int>> points = new List<Tuple<int,int>>();
        for (int i = 0; i < m_Area.Count; i++)
        {
            var rect = m_Area[i];

            if (y >= m_Area[i].Y && y <= m_Area[i].Y + m_Area[i].Height)
            {
                points.Add(new Tuple<int, int>(m_Area[i].X, m_Area[i].Width));
            }
        }

        List<int> ret = new List<int>();
        ret.Add(0);

        points.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));
        
        for (int i = 0; i < points.Count; i++)
        {
            if (i == 0)
                ret.Add(points[i].Item1);

            if (i > 0 && i < points.Count)
            {
                if (points[i - 1].Item1 + points[i - 1].Item2 < points[i].Item1)
                {
                    ret.Add(points[i - 1].Item1 + points[i - 1].Item2);
                    ret.Add(points[i].Item1);
                }
            }
            
            if (i == points.Count - 1)
                ret.Add(points[i].Item1 + points[i].Item2);
        }

        ret.Add(9999); // 9999 is to end of map
        
        return ret;
    }
}