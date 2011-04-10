using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using QuickGraph;

namespace Inspector.Engine
{
	/// <summary>
	/// Represents a graph of assemblies and their dependencies to other assemblies.
	/// </summary>
	public class AssemblyDependencyGraph
	{
		private readonly BidirectionalGraph<AssemblyNode, AssemblyDependencyEdge> _dependencyGraph =
			new BidirectionalGraph<AssemblyNode, AssemblyDependencyEdge>();

		public void AddAssembly(Assembly assembly)
		{
			if (!ContainsAssembly(assembly))
			{
				DoAddAssembly(assembly);
			}
		}

		public void AddDependency(Assembly from, Assembly to)
		{
			if (!ContainsDependency(from, to))
			{
				DoAddDependency(from, to);
			}
		}

		public bool ContainsAssembly(Assembly assembly)
		{
			return _dependencyGraph.ContainsVertex(new AssemblyNode(assembly));
		}

		public bool ContainsDependency(Assembly from, Assembly to)
		{
			return _dependencyGraph.ContainsEdge(new AssemblyNode(from), new AssemblyNode(to));
		}

		public IEnumerable<Assembly> GetAssemblies()
		{
			return _dependencyGraph.Vertices.Select(v => v.Assembly);
		}

		public IEnumerable<Assembly> GetDependantAssemblies(Assembly assembly)
		{
			IEnumerable<AssemblyDependencyEdge> outEdges;
			if (_dependencyGraph.TryGetOutEdges(new AssemblyNode(assembly), out outEdges))
			{
				return outEdges.Select(e => e.Target.Assembly);
			}
			return Enumerable.Empty<Assembly>();
		}

		private void DoAddDependency(Assembly from, Assembly to)
		{
			_dependencyGraph.AddVerticesAndEdge(
				new AssemblyDependencyEdge(new AssemblyNode(from), new AssemblyNode(to)));
		}

		private void DoAddAssembly(Assembly assembly)
		{
			_dependencyGraph.AddVertex(new AssemblyNode(assembly));
		}
	}
}
