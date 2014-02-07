﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace pserv4.uninstaller
{
    public class UninstallerDataController : IObjectController
    {
        private static List<ObjectColumn> ActualColumns;

        public UninstallerDataController()
        {
        }

        public IEnumerable<ObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<ObjectColumn>();
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_ApplicationName, "ApplicationName"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_InstallLocation, "InstallLocation"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_Key, "Key"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_Version, "Version"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_Publisher, "Publisher"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_HelpLink, "HelpLink"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_AboutLink, "AboutLink"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_Action, "Action"));
                }
                return ActualColumns;
            }
        }

        public void Refresh(ObservableCollection<IObject> objects)
        {
            Dictionary<string, UninstallerDataObject> existingObjects = new Dictionary<string, UninstallerDataObject>();

            foreach (IObject o in objects)
            {
                UninstallerDataObject sdo = o as UninstallerDataObject;
                if (sdo != null)
                {
                    existingObjects[sdo.Key] = sdo;
                }
            }
            RefreshEntries(existingObjects, objects, Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
            RefreshEntries(existingObjects, objects, Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
        }
        
        private void RefreshEntries(Dictionary<string, UninstallerDataObject> existingObjects, ObservableCollection<IObject> objects, RegistryKey rootKey, string keyName)
        {
            using(RegistryKey hkKey = rootKey.OpenSubKey(keyName,false))
            {
                foreach (string subKeyName in hkKey.GetSubKeyNames())
                {
                    UninstallerDataObject mdo;
                    if (existingObjects.TryGetValue(subKeyName, out mdo))
                    {
                        // todo: refresh existing instance from updated data
                    }
                    else
                    {
                        mdo = new UninstallerDataObject(rootKey, string.Format("{0}\\{1}", keyName, subKeyName), subKeyName);
                        if( !string.IsNullOrEmpty(mdo.ApplicationName) )
                        {
                            objects.Add(mdo);
                        }
                    }
                }
            }
        }
    }
}
