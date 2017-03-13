using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.TypeScript.Generators;

namespace Umbrella.TypeScript.Aurelia
{
    public class AureliaInterfaceGenerator : BaseInterfaceGenerator
    {
        #region Overridden Properties
        public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.AureliaInterface; 
        #endregion
    }
}