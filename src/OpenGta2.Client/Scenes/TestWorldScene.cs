using BlackMarketRevolution.Slice;
using BlackMarketRevolution.Economy;
using BlackMarketRevolution.Missions;
using BlackMarketRevolution.Nap;
using BlackMarketRevolution.Wanted;
using OpenGta2.Client.Components;
using OpenGta2.Client.Diagnostics;
using OpenGta2.Client.Peds;
using OpenGta2.Client.Utilities;

namespace OpenGta2.Client.Scenes;

public class TestWorldScene : Scene
{
    public TestWorldScene(GtaGame game) : base(game)
    {
        Camera = new Camera(game.Window);
    }

    public Camera Camera { get; }
    
    public override void Initialize()
    {
        Game.Services.ReplaceService(Camera);

        Game.Services.ReplaceService(new PedManager());
        var wallet = new PlayerWallet();
        Game.Services.ReplaceService(wallet);
        var property = new UndergroundProperty();
        Game.Services.ReplaceService(property);
        var wanted = new WantedMeter();
        Game.Services.ReplaceService(wanted);
        var nap = new NapReputation();
        Game.Services.ReplaceService(nap);
        var mission = new SmuggleMission(wanted, nap);
        Game.Services.ReplaceService(mission);
        Game.Services.ReplaceService(new SliceDebugController(wallet, property, mission, wanted, nap));

        AddComponent<AudioTestComponent>();
        AddComponent<MapComponent>();
        AddComponent<SpriteTestComponent>();
        AddComponent<PlayerControllerComponent>();
        AddComponent<PedManagerComponent>();
        AddComponent<CameraComponent>();
        AddComponent<WalletHudComponent>();
        AddComponent<PropertyHudComponent>();
        AddComponent<MissionHudComponent>();
        AddComponent<WantedHudComponent>();
        AddComponent<NapHudComponent>();
        AddComponent<SliceDebugHudComponent>();
        AddComponent<DebuggingDrawingComponent>();
    }
}
