using System.Reflection;
using QuickGraph;

namespace Inspector.Engine
{
	public static class AssemblyDependencyGraphExtensions
	{
		public static BidirectionalGraph<object, IEdge<object>> ToBidirectionalGraph(this AssemblyDependencyGraph graph)
		{
			// convert schedule graph into generic bidirectional graph that is bindable to the WPF control
			var g = new BidirectionalGraph<object, IEdge<object>>();
			foreach (Assembly assembly in graph.GetAssemblies())
			{
				string fromAssemblyName = assembly.GetName().Name;
				g.AddVertex(fromAssemblyName);

				foreach (Assembly dependantAssembly in graph.GetDependantAssemblies(assembly))
				{
					string toAssemblyName = dependantAssembly.GetName().Name;
					if (!g.ContainsVertex(toAssemblyName))
					{
						g.AddVertex(toAssemblyName);
					}
					g.AddEdge(new Edge<object>(fromAssemblyName, toAssemblyName));
				}
			}
			return g;
		}		
	}
}