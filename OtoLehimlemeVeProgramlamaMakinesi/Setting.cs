using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OtoLehimlemeVeProgramlamaMakinesi
{
    [CompilerGenerated]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]

    internal sealed class Setting : ApplicationSettingsBase
    {
        private static Setting defaultInstance = (Setting)SettingsBase.Synchronized((SettingsBase)new Setting());
        public static Setting Default
        {
            get
            {
                return Setting.defaultInstance;
            }
        }
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public BindingList<string> programListF1
        {
            get
            {
                return (BindingList<string>)this[nameof(programListF1)];
            }
            set
            {
                this[nameof(programListF1)] = (object)value;
            }
        }
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public BindingList<string> programListF2
        {
            get
            {
                return (BindingList<string>)this[nameof(programListF2)];
            }
            set
            {
                this[nameof(programListF2)] = (object)value;
            }
        }
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("B1")]
        public List<string> set2
        {
            get
            {
                return (List<string>)this[nameof(set2)];
            }
            set
            {
                this[nameof(set2)] = (object)value;
            }
        }
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("1")]
        public int num1
        {
            get
            {
                return (int)this[nameof(num1)];
            }
            set
            {
                this[nameof(num1)] = (object)value;
            }
        }
    }
}

