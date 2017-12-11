using System;

namespace Umbrella.TypeScript
{
    [Flags]
    public enum TypeScriptOutputModelType
    {
        Interface =         0b00000001,
        Class =             0b00000011, //Ensure both interface and class will be generated
        KnockoutInterface = 0b00000100,
        KnockoutClass =     0b00001100, //Ensure both interface and class will be generated
        AureliaInterface =  0b00010000,
        AureliaClass =      0b00110000  //Ensure both interface and class will be generated
    }
}