using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace BackBuero_Backup
{
    public class Config
    {
        ModifyRegistry reg = null;

        private int backupCycle;
        private bool enableBackupCycle;
        private string backupPath;
        private string lastBackup;

        string[] keys = { "backupCycle", "backupPath", "lastBackup", "enableBackupCycle" }; //TODO -> enableBackupCycle
        string datePatt = "d.M.yyyy";

        public void setBackupCycle(int cycle)
        {
            this.backupCycle = cycle;
        }

        public int getBackupCycle()
        {
            return this.backupCycle;
        }

        public void setBackupPath(string path)
        {
            this.backupPath = path;
        }

        public string getBackupPath()
        {
            return this.backupPath;
        }

        public void setLastBackup()
        {
            this.lastBackup = DateTime.Now.Date.ToString(datePatt);
        }

        public void setLastBackup(string date)
        {
            this.lastBackup = date;
        }

        public string getLastBackup()
        {
            return this.lastBackup;
        }

        public bool ReadConfig()
        {
            if (reg == null)
            {
                reg = new ModifyRegistry();
                reg.SetKeyPath(@"Software\BackBueroBackup");
            }

            try
            {
                backupCycle = Convert.ToInt32(reg.Read(keys[0]));
                backupPath = (string)reg.Read(keys[1]);
                lastBackup = (string)reg.Read(keys[2]);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool WriteConfig()
        {
            try
            {
                reg.Write(keys[0], backupCycle);
                reg.Write(keys[1], backupPath);
                reg.Write(keys[2], lastBackup);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
