using System;
using System.Collections.Generic;

[Serializable]
public class PathTile
{
    public int row;
    public int col;
}

[Serializable]
public class LevelData
{
    public int level;
    public List<PathTile> paths;
}

[Serializable]
public class LevelCollection
{
    public List<LevelData> levels;
}
