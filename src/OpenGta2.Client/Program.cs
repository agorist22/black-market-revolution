using System;
using OpenGta2.Client;

if (!TestGamePath.TryGetRoot(out var root, out var error))
{
    Console.Error.WriteLine(error);
    Console.Error.WriteLine();
    Console.Error.WriteLine("Black Market Revolution / OpenGta2 cannot start without a legal GTA2 data path.");
    return 1;
}

Console.WriteLine($"Using GTA2 data at: {root}");

using var game = new GtaGame();
game.Window.Title = "Black Market Revolution";
game.Run();
return 0;
