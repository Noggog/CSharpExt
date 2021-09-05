#if NETSTANDARD2_0
#else
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace Noggog.Utility
{
    public static class AssemblyLoading
    {
        class HostAssemblyLoadContext : AssemblyLoadContext
        {
            // Resolver of the locations of the assemblies that are dependencies of the
            // main plugin assembly.
            private AssemblyDependencyResolver _resolver;

            public HostAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
            {
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }

            // The Load method override causes all the dependencies present in the plugin's binary directory to get loaded
            // into the HostAssemblyLoadContext together with the plugin assembly itself.
            // NOTE: The Interface assembly must not be present in the plugin's binary directory, otherwise we would
            // end up with the assembly being loaded twice. Once in the default context and once in the HostAssemblyLoadContext.
            // The types present on the host and plugin side would then not match even though they would have the same names.
            protected override Assembly? Load(AssemblyName name)
            {
                string? assemblyPath = _resolver.ResolveAssemblyToPath(name);
                if (assemblyPath != null)
                {
                    return LoadFromAssemblyPath(assemblyPath);
                }

                return null;
            }
        }

        // It is important to mark this method as NoInlining, otherwise the JIT could decide
        // to inline it into the Main method. That could then prevent successful unloading
        // of the plugin because some of the MethodInfo / Type / Plugin.Interface / HostAssemblyLoadContext
        // instances may get lifetime extended beyond the point when the plugin is expected to be
        // unloaded.
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TRet ExecuteAndUnload<TRet>(string assemblyPath, out WeakReference alcWeakRef, Func<Assembly, TRet> getter, Func<AssemblyLoadContext>? loadContextGetter = null)
        {
            // Create the unloadable HostAssemblyLoadContext
            var alc = loadContextGetter == null ? new HostAssemblyLoadContext(assemblyPath) : loadContextGetter();

            // Create a weak reference to the AssemblyLoadContext that will allow us to detect
            // when the unload completes.
            alcWeakRef = new WeakReference(alc);

            try
            {
                // Load the plugin assembly into the HostAssemblyLoadContext.
                // NOTE: the assemblyPath must be an absolute path.
                Assembly assemb = alc.LoadFromAssemblyPath(assemblyPath);
                return getter(assemb);
            }
            finally
            {
                // This initiates the unload of the HostAssemblyLoadContext. The actual unloading doesn't happen
                // right away, GC has to kick in later to collect all the stuff.
                alc.Unload();
            }
        }

        // Unloading happens on GC, so have to run GC several times after unload
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TRet ExecuteAndForceUnload<TRet>(string assemblyPath, Func<Assembly, TRet> getter, Func<AssemblyLoadContext>? loadContextGetter = null)
        {
            WeakReference hostAlcWeakRef;
            var ret = ExecuteAndUnload(assemblyPath, out hostAlcWeakRef, getter, loadContextGetter);
            for (int i = 0; hostAlcWeakRef.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return ret;
        }
    }
}
#endif