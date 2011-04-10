using QuickGraph;

namespace Inspector.Engine
{
	public class AssemblyDependencyEdge : Edge<AssemblyNode>
	{
		public AssemblyDependencyEdge(AssemblyNode source, AssemblyNode target)
			: base(source, target)
		{
		}
	}
}