public enum SceneType
{
    Title = 0,
    Loading,
    InGame,
}

public enum EnemyState
{
    None = -1,  // 사용전
    Ready,      // 준비 완료
    Appear,     // 등장
    Battle,     // 전투중
    Dead,       // 사망
    Disappear,  // 퇴장
}

public enum BulletType
{
    PlayerBullet = 0,
    EnemyBullet,
}

public enum EffectType
{
    DisappearFx = 0,
    DeadFx,
}

public enum UIDamageState
{
    None = 0,
    SizeUp,
    Display,
    FadeOut,
}

public enum DamageType
{
    Normal = 0,
}

public enum GameState
{
    Ready = 0,
    Running,
    End,
}