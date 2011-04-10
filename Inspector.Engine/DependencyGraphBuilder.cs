using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Inspector.Engine
{
	public class DependencyGraphBuilder
	{
		private readonly HashSet<string> _assemblyPaths = new HashSet<string>();
		private readonly AssemblyLoader _assemblyLoader = new AssemblyLoader();

		private AssemblyDependencyGraph _dependencyGraph;

		public AssemblyDependencyGraph BuildAssemblyDependencyGraph()
		{
			_dependencyGraph = new AssemblyDependencyGraph();
			foreach (string assemblyPath in _assemblyPaths)
			{
				Assembly assembly = _assemblyLoader.LoadAssembly(assemblyPath);
				if (ShouldIncludeAssembly(assembly))
				{
					AddAssemblyAndScanForDependencies(assembly);
				}
			}
			return _dependencyGraph;
		}

		public bool IncludeSystemAssemblies { get; set; }

		public void AddAssemblyAndAllDependencies(string assemblyFilePath)
		{
			if (!File.Exists(assemblyFilePath))
			{
				throw new FileNotFoundException("Could not find the specified assembly", assemblyFilePath);
			}
			_assemblyPaths.Add(assemblyFilePath);
		}

		public void AddAssemblyAndAllDependencies(Assembly assembly)
		{
			AddAssemblyAndAllDependencies(AssemblyLoader.GetAssemblyLocalPath(assembly));
		}

		private void AddAssemblyAndScanForDependencies(Assembly assembly)
		{
			_dependencyGraph.AddAssembly(assembly);
			foreach (AssemblyName referencedAssemblyName in assembly.GetReferencedAssemblies())
			{
				if (ShouldIncludeAssembly(referencedAssemblyName.Name))
				{
					Assembly referencedAssembly = _assemblyLoader.LoadAssembly(referencedAssemblyName.Name);
					if (referencedAssembly != null)
					{
						// recurse all dependencies
						AddAssemblyAndScanForDependencies(referencedAssembly);

						// create dependency
						_dependencyGraph.AddDependency(assembly, referencedAssembly);
					}
				}
			}
		}

		private bool ShouldIncludeAssembly(string assemblyName)
		{
			if (assemblyName == null || (AssemblyLoader.IsSystemAssembly(assemblyName) && !IncludeSystemAssemblies))
			{
				return false;
			}
			return true;
		}

		private bool ShouldIncludeAssembly(Assembly assembly)
		{
			if (assembly == null || (AssemblyLoader.IsSystemAssembly(assembly) && !IncludeSystemAssemblies))
			{
				return false;
			}
			return true;
		}
	}
}
