using System.Runtime.CompilerServices;

namespace HotelProyecto.Tests
{
    internal static class TestInit
    {
        [ModuleInitializer]
        public static void Init() => TestReporter.Initialize();
    }
}
