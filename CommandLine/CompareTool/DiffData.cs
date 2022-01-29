using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareTool
{
	partial class Program
	{
		public class ModelDiffData
		{
			public int ArrayID;
		}

		public class UVDiffData : ModelDiffData
		{
			public int U;
			public int V;
		}

		public class MaterialTextureDiffData : ModelDiffData
		{
			public int TexID;
		}

		public class MaterialFlagsDiffData : ModelDiffData
		{
			public uint Flags;
		}

		public class MaterialExponentDiffData : ModelDiffData
		{
			public float Exponent;
		}

		public class VertexNormalDiffData : ModelDiffData
		{
			public float X;
			public float Y;
			public float Z;
		}

		public class ColorDiffData : ModelDiffData
		{
			public int A;
			public int R;
			public int G;
			public int B;
		}

		public class DiffuseColorDiffData : ColorDiffData { }

		public class SpecularColorDiffData : ColorDiffData { }
	}
}
