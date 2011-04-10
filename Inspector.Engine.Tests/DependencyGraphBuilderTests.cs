using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SharpTestsEx;

namespace Inspector.Engine.Tests
{
	[TestFixture]
	public class DependencyGraphBuilderTests
	{
		private Assembly _testAssembly;
		private DependencyGraphBuilder _graphBuilder;

		[SetUp]
		public void SetUp()
		{
			_testAssembly = GetType().Assembly;
			_graphBuilder = new DependencyGraphBuilder();
		}

		[Test]
		public void BuildAssemblyDependencyGraph_creates_a_graph_of_all_dependencies()
		{
			_graphBuilder.AddAssemblyAndAllDependencies(_testAssembly);
			AssemblyDependencyGraph graph = _graphBuilder.BuildAssemblyDependencyGraph();

			graph.GetAssemblies().Should().Have.Count.EqualTo(5); // 5 non-System assemblies
			graph.GetAssemblies().Single(o => o.GetName().Name == "nunit.framework");
			graph.GetAssemblies().Single(o => o.GetName().Name == "Inspector.Engine");
		}

		[Test]
		public void BuildAssemblyDependencyGraph_creates_edges_between_dependencies()
		{
			_graphBuilder.AddAssemblyAndAllDependencies(_testAssembly);
			AssemblyDependencyGraph graph = _graphBuilder.BuildAssemblyDependencyGraph();

			IEnumerable<Assembly> dependencies = graph.GetDependantAssemblies(_testAssembly);
			dependencies.Single(o => o.GetName().Name == "nunit.framework");
			dependencies.Single(o => o.GetName().Name == "Inspector.Engine");
		}

		[Test]
		public void Can_load_external_dlls_from_outside_bin_directory()
		{
			string libDir = GetLibDirectory();
			string graphSharpFullPath = Path.Combine(libDir, "GraphSharp.dll");

			_graphBuilder.AddAssemblyAndAllDependencies(graphSharpFullPath);
			AssemblyDependencyGraph graph = _graphBuilder.BuildAssemblyDependencyGraph();

			Assembly graphSharpAssembly = new AssemblyLoader().LoadAssembly(graphSharpFullPath);
			IEnumerable<Assembly> dependencies = graph.GetDependantAssemblies(graphSharpAssembly);
			dependencies.Single(o => o.GetName().Name == "QuickGraph");
		}

		private string GetLibDirectory()
		{
			string[] pathParts = GetBinDirectory().Split('\\');
			return string.Join(@"\", pathParts, 0, pathParts.Length - 3) + @"\lib";
		}

		private string GetBinDirectory()
		{
			string filePath = new Uri(_testAssembly.CodeBase).LocalPath;
			return Path.GetDirectoryName(filePath); 
		}
	}
}
