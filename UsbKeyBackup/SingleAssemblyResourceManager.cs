using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Reflection;

namespace KlerksSoft.UsbKeyBackup
{
    public class SingleAssemblyResourceManager : System.Resources.ResourceManager
    {
        private Type _contextTypeInfo;
        private CultureInfo _neutralResourcesCulture;

        public SingleAssemblyResourceManager(Type t)
            : base(t)
        {
            _contextTypeInfo = t;
        }

        public SingleAssemblyResourceManager(string baseName, Assembly assembly)
            : base (baseName, assembly)
        {
            _contextTypeInfo = null;
        }
 
        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            ResourceSet rs = (ResourceSet)this.ResourceSets[culture];
            if (rs == null)
            {
                Stream store = null;
                string resourceFileName = null;

                //lazy-load default language (without caring about duplicate assignment in race conditions, no harm done);
                if (this._neutralResourcesCulture == null)
                {
                    this._neutralResourcesCulture = GetNeutralResourcesLanguage(this.MainAssembly);
                }

                //if we're asking for the default language, then ask for the invaliant (non-specific) resources.
                if (_neutralResourcesCulture.Equals(culture))
                    culture = CultureInfo.InvariantCulture;
                resourceFileName = GetResourceFileName(culture);

                if (this._contextTypeInfo != null)
                    store = this.MainAssembly.GetManifestResourceStream(this._contextTypeInfo, resourceFileName);
                else
                    store = this.MainAssembly.GetManifestResourceStream("KlerksSoft.UsbKeyBackup." + resourceFileName);

                //If we found the appropriate resources in the local assembly
                if (store != null)
                {
                    rs = new ResourceSet(store);
                    //save for later.
                    AddResourceSet(this.ResourceSets, culture, ref rs);
                }
                else
                {
                    rs = base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
                }
            }
            return rs;
        }

        //private method in framework, had to be re-specified
        private static void AddResourceSet(Hashtable localResourceSets, CultureInfo culture, ref ResourceSet rs)
        {
            lock (localResourceSets)
            {
                ResourceSet objA = (ResourceSet)localResourceSets[culture];
                if (objA != null)
                {
                    if (!object.Equals(objA, rs))
                    {
                        rs.Dispose();
                        rs = objA;
                    }
                }
                else
                {
                    localResourceSets.Add(culture, rs);
                }
            }
        }
    }
}
