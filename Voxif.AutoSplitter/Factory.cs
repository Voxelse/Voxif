﻿using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Reflection;

namespace Voxif.AutoSplitter {
    public class Factory : IComponentFactory {
        public string UpdateName => ComponentName;
        public string UpdateURL => ExAssembly.GitMainURL();
        public string XMLURL => UpdateURL + "Components/ComponentsUpdate.xml";
        public Version Version => ExAssembly.GetName().Version;
        public string ComponentName => ExAssembly.FullComponentName();
        public string Description => ExAssembly.Description();
        public ComponentCategory Category => ComponentCategory.Control;
        public IComponent Create(LiveSplitState state) {
            AssemblyName asmName = ExAssembly.GetName();
            return (IComponent)Activator.CreateInstance(
                Type.GetType(asmName.Name + "." + asmName.Name.Substring(10) + "Component, " + asmName.FullName),
                new object[] { state });
        }

        public static Assembly ExAssembly = Assembly.GetExecutingAssembly();
    }
}