using UnityEngine;

namespace Bang
{

    public static class Number
    {
        public const int Bangs = 25;
        public const int Misseds = 12;
        public const int Beers = 6;
        public const int Panics = 4;
        public const int CatBalous = 4;
        public const int Duels = 3;
        public const int Jails = 3;
        public const int Schofields = 3;
        public const int GeneralStores = 2;
        public const int Mustangs = 2;
        public const int Barrels = 2;
        public const int Volcanics = 2;
        public const int Stagecoaches = 2;
        public const int Indians = 2;
    }

    public class Roles
    {

        public const string SheriffName = "SHERIFF";
        private const string DeputyName = "DEPUTY";
        private const string OutlawName = "OUTLAW";
        private const string RenegadeName = "RENEGADE";

        public readonly static Color SheriffColor = Color.yellow;
        private readonly static Color DeputyColor = Color.green;
        private readonly static Color OutlawColor = Color.red;
        private readonly static Color RenegadeColor = Color.blue;

        public static string GetNameFromRole(Role role)
        {
            switch (role)
            {
                case Role.Outlaw:
                    return OutlawName;
                case Role.Deputy:
                    return DeputyName;
                case Role.Renegade:
                    return RenegadeName;
            }
            return null;
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
            switch (role)
            {
                case Role.Outlaw:
                    return OutlawColor;
                case Role.Deputy:
                    return DeputyColor;
                case Role.Renegade:
                    return RenegadeColor;
            }
            return new Color();
        }

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
        Duel,
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
        Bang,
        Barrel,
        Beer,
        Scope,
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
