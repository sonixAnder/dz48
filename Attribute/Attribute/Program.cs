using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

class Program
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IniValueAttribute : Attribute
    {
        public string IniFile { get; }
        public string Key { get; }

        public IniValueAttribute(string iniFile, string key)
        {
            IniFile = iniFile;
            Key = key;
        }
    }

    public class IniReader
    {
        private readonly Dictionary<string, string> _iniData = new Dictionary<string, string>();

        public IniReader(string iniFile)
        {
            if (File.Exists(iniFile))
            {
                foreach (var line in File.ReadAllLines(iniFile))
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        _iniData[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
            else
            {
                throw new FileNotFoundException($"Ini file not found: {iniFile}");
            }
        }

        public string GetValue(string key)
        {
            return _iniData.ContainsKey(key) ? _iniData[key] : null;
        }
    }

    public class AppSettings
    {
        [IniValue("settings1.ini", "AppName")]
        public string AppName { get; set; }

        [IniValue("settings2.ini", "MaxUsers")]
        public int MaxUsers { get; set; }

        [IniValue("settings3.ini", "Version")]
        public string Version { get; set; }
    }

    static void Main()
    {
        var appSettings = new AppSettings();

        LoadSettings(appSettings);

        Console.WriteLine($"AppName: {appSettings.AppName}");
        Console.WriteLine($"MaxUsers: {appSettings.MaxUsers}");
        Console.WriteLine($"Version: {appSettings.Version}");
    }

    public static void LoadSettings(object obj)
    {
        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            var attribute = (IniValueAttribute)Attribute.GetCustomAttribute(property, typeof(IniValueAttribute));

            if (attribute != null)
            {
                var iniReader = new IniReader(attribute.IniFile);
                var value = iniReader.GetValue(attribute.Key);

                if (value != null)
                {
                    if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(obj, int.Parse(value));
                    }
                    else
                    {
                        property.SetValue(obj, value);
                    }
                }
            }
        }
    }
}
