namespace Assets.Scripts
{
	public enum Drake
	{
        Astarot,
        Leviathan,
        Ryuu,
        Viper,
        Volos
	}

    public enum GameState
    {
        Ready,
        Initializing,
        Running,
        Paused,
        Completed
    }

    public enum ControllerType
    {
        A,
        B,
        C,
        D
    }

    public enum Achivements
    {
        DummySnake,
        LongSnake,
        BossSnake,
        MegaSnake,
        KingSnake,
        ChuckNorrisSnake,
        LuckySnake,
        SpeedySnake,
        EpicFailSnake,
        JerkSnake,
        //
        AstarotScale,
        LeviathanScale,
        RyuuScale,
        ViperScale,
        VolosScale,
        //
        UnlockViper,
        UnlockVolos,
        GameMaster,
        GameKing,
        //
        Played1Hours,
        Played2Hours,
        Played3Hours,
        Played4Hours,
        Eaten100Apples,
        Eaten500Apples,
        Eaten1000Apples,
        Eaten2500Apples
    }

    public enum GameEdition
    {
        Free,
        Delux,
        WebPlayer
    }
}