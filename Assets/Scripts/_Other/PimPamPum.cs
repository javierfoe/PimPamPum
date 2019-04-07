using UnityEngine;

namespace PimPamPum
{

    public static class PimPamPumConstants
    {
        public const int NoOne = -1;
    }

    public static class Roles
    {

        private const string SheriffName = "SHERIFF";
        private const string DeputyName = "DEPUTY";
        private const string OutlawName = "OUTLAW";
        private const string RenegadeName = "RENEGADE";

        private readonly static Color SheriffColor = Color.yellow;
        private readonly static Color DeputyColor = Color.green;
        private readonly static Color OutlawColor = Color.red;
        private readonly static Color RenegadeColor = Color.blue;

        public static string GetNameFromRole(Role role)
        {
            string res = null;
            switch (role)
            {
                case Role.Sheriff:
                    res = SheriffName;
                    break;
                case Role.Outlaw:
                    res = OutlawName;
                    break;
                case Role.Deputy:
                    res = DeputyName;
                    break;
                case Role.Renegade:
                    res = RenegadeName;
                    break;
            }
            return res;
        }

        public static Role[] GetRoles(int players)
        {
            Role[] result = new Role[players];
            switch (players)
            {
                case 8:
                    result[7] = Role.Renegade;
                    goto case 7;
                case 7:
                    result[6] = Role.Deputy;
                    goto case 6;
                case 6:
                    result[5] = Role.Outlaw;
                    goto case 5;
                case 5:
                    result[4] = Role.Deputy;
                    goto case 4;
                case 4:
                    result[3] = Role.Outlaw;
                    goto case 3;
                case 3:
                    result[2] = Role.Outlaw;
                    goto case 2;
                case 2:
                    result[1] = Role.Renegade;
                    result[0] = Role.Sheriff;
                    break;
            }
            return result;
        }

        public static Color GetColorFromRole(Role role)
        {
            Color res = new Color();
            switch (role)
            {
                case Role.Sheriff:
                    res = SheriffColor;
                    break;
                case Role.Outlaw:
                    res = OutlawColor;
                    break;
                case Role.Deputy:
                    res = DeputyColor;
                    break;
                case Role.Renegade:
                    res = RenegadeColor;
                    break;
            }
            return res;
        }
    }

    public struct CardStruct
    {
        public string name;
        public Suit suit;
        public Rank rank;
        public Color color;
    }

    public enum State
    {
        OutOfTurn,
        Play,
        Response,
        PimPamPum,
        Duel,
        Discard,
        Dying
    }

    public enum Event
    {
        Hit,
        Die,
        Play,
        Response,
        Duel,
        Panic,
        CatBalou,
        GeneralStore,
        DrawEffect
    }

    public enum Team
    {
        Law,
        Outlaw,
        Renegade
    }

    public enum Drop
    {
        Nothing,
        Trash,
        Hand,
        Properties,
        Weapon
    }

    public enum Decision
    {
        Pending,
        TakeHit,
        Die,
        Avoid,
        Barrel,
        Source
    }

    public enum Role
    {
        Sheriff,
        Deputy,
        Outlaw,
        Renegade
    }

    public enum CardType
    {
        PimPamPum,
        Barrel,
        Beer,
        Carabine,
        CatBalou,
        Colt45,
        Duel,
        Dynamite,
        Gatling,
        GeneralStore,
        Indians,
        Jail,
        Missed,
        Mustang,
        Panic,
        Remington,
        Saloon,
        Schofield,
        Scope,
        Stagecoach,
        Volcanic,
        WellsFargo,
        Winchester
    }

    public enum Suit
    {
        Null,
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }

    public enum Rank
    {
        Null,
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

}
