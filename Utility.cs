using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GetYourOwnPortal;

class PortalInfo
{
    public static readonly char DELIM = '%';

    string m_playerName;
    public string playerName
    {
        get { return m_playerName; }
    }

    string m_text;
    public string text
    {
        get { return m_text; }
    }

    public PortalInfo(string playerName, string text)
    {
        this.m_playerName = playerName;
        this.m_text = text;
    }
}

static class Util
{
    public static PortalInfo GetPortalInfo(string text)
    {
        string[] arr = text.Split(PortalInfo.DELIM);
        return new PortalInfo(arr[0], arr[1]);
    }

    public static string GetPortalPrefix(string playerName)
    {
        return $"{playerName}{PortalInfo.DELIM}";
    }
}
