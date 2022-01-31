using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Cinnamon
{
    public class CinnamonInfo : GH_AssemblyInfo
    {
        public override string Name => "Cinnamon";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("D08669DB-5E42-4019-BEA8-C49B65B4B84B");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}