using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Util
{
    public class ModifyRegistry
    {
        private string objKeyPath { get; set; }

        public void SetKeyPath(string keyPath)
        {
            this.objKeyPath = keyPath;
        }

        #region Reading registry
        public object Read(string key)
        {
            RegistryKey regKey = Registry.CurrentConfig.CreateSubKey(this.objKeyPath);
            return regKey.GetValue(key).ToString();
        }

        public string Read(string keyPath, string key)
        {
            RegistryKey regKey = Registry.CurrentConfig.CreateSubKey(keyPath);
            return regKey.GetValue(key).ToString();
        }
        #endregion

        #region Writing registry
        public void Write(string key, object value)
        {
            RegistryKey regKey = Registry.CurrentConfig.CreateSubKey(this.objKeyPath);
            regKey.SetValue(key, value);
        }

        public void Write(string keyPath, string key, object value)
        {
            RegistryKey regKey = Registry.CurrentConfig.CreateSubKey(keyPath);
            regKey.SetValue(key, value);
        }
        #endregion
    }
}
