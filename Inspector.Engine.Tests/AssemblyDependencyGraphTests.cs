using System.Reflection;
using NUnit.Framework;
using SharpTestsEx;

namespace Inspector.Engine.Tests
{
	[TestFixture]
	public class AssemblyDependencyGraphTests
	{
		private AssemblyDependencyGraph _graph;
		private Assembly _assembly1;
		private Assembly _assembly2;

		[SetUp]
		public void SetUp()
		{
			_assembly1 = GetType().Assembly;
			_assembly2 = typeof(AssemblyDependencyGraph).Assembly;
			_graph = new AssemblyDependencyGraph();
		}

		[Test]
		public void Can_add_assembly()
		{
			_graph.AddAssembly(_assembly1);

			_graph.GetAssemblies().Should().Have.Count.EqualTo(1);
			_graph.ContainsAssembly(_assembly1).Should().Be.True();
			_graph.GetAssemblies().Should().Contain(_assembly1);
		}

		[Test]
		public void Can_add_2_assemblies()
		{
			_graph.AddAssembly(_assembly1);
			_graph.AddAssembly(_assembly1);

			_graph.GetAssemblies().Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void AddAssembly_is_idempotent()
		{
			_graph.AddDependency(_assembly1, _assembly2);
			_graph.AddDependency(_assembly1, _assembly2);
			_graph.GetAssemblies().Should().Have.Count.EqualTo(2);
			_graph.GetDependantAssemblies(_assembly1).Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void Can_add_assembly_dependency()
		{
			_graph.AddDependency(_assembly1, _assembly2);
			_graph.ContainsDependency(_assembly1, _assembly2).Should().Be.True();
			_graph.GetDependantAssemblies(_assembly1).Should().Have.Count.EqualTo(1);
			_graph.GetDependantAssemblies(_assembly1).Should().Contain(_assembly2);
		}

		[Test]
		public void AddDependency_is_idempotent()
		{
			_graph.AddDependency(_assembly1, _assembly2);
			_graph.AddDependency(_assembly1, _assembly2);
			_graph.GetAssemblies().Should().Have.Count.EqualTo(2);
			_graph.GetDependantAssemblies(_assembly1).Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void GetDependantAssemblies_returns_empty_when_edge_does_not_exist()
		{
			_graph.GetDependantAssemblies(_assembly1).Should().Have.Count.EqualTo(0);
		}
	}
}
