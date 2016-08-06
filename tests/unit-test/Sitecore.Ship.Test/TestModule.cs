using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Test
{
    public interface IDepencency
    {
        void DoSomething();
    }

    public class TestModule : ShipBaseModule
    {
        private readonly IDepencency _dependency;

        public TestModule(IAuthoriser authoriser, IDepencency dependency)
            : base(authoriser, "/services/test")
        {
            _dependency = dependency;
            Get["/empty"] = EmptyMethod;
        }

        public virtual dynamic EmptyMethod(dynamic o)
        {
            _dependency.DoSomething();

            return Response.AsJson(DateTime.Now);
        }
    }
}
