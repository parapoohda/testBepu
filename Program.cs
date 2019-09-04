using BepuUtilities;
using DemoContentLoader;
using DemoUtilities;
using OpenTK;
namespace bepuphysics2_for_nelalen
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new Window("pretty cool multicolored window", new Int2((int)(DisplayDevice.Default.Width * 0.75f), (int)(DisplayDevice.Default.Height * 0.75f)), WindowMode.Windowed);
            var loop = new GameLoop(window);
            ContentArchive content;
            using (var stream = typeof(Program).Assembly.GetManifestResourceStream("bepuphysics2_for_nelalen.Demos.contentarchive"))
            {
                content = ContentArchive.Load(stream);
            }
            var demo = new DemoHarness(loop, content);

            loop.Run(demo);
            loop.Dispose();
            window.Dispose();
        }
    }
}