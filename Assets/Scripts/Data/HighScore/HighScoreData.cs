﻿using MessagePack;
using System;
namespace BeatGame.Data.Score
{
    [Serializable]
    [MessagePackObject]
    public struct HighScoreData : IEquatable<HighScoreData>
    {
        [Key("SongName")]
        public string SongName;
        [Key("LevelAuthor")]
        public string LevelAuthor;
        [Key("Difficulty")]
        public string Difficulty;
        [Key("Score")]
        public int Score;

        public bool Equals(HighScoreData other)
        {
            return SongName.Equals(other.SongName) && LevelAuthor.Equals(other.LevelAuthor) && Difficulty.Equals(other.Difficulty);
        }

        public override string ToString()
        {
            return SongName + LevelAuthor + Difficulty;
        }
    }
}