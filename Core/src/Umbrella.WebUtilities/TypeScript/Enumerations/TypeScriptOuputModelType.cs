using System;

namespace Umbrella.WebUtilities.TypeScript.Enumerations
{
    [Flags]
    public enum TypeScriptOutputModelType
    {
        Interface =         0x00000001,
        Class =             0x00000011, //Ensure both interface and class will be generated
        KnockoutInterface = 0x00000100,
        KnockoutClass =     0x00001100, //Ensure both interface and class will be generated
        AureliaInterface =  0x00010000,
        AureliaClass =      0x00110000  //Ensure both interface and class will be generated
    }
}